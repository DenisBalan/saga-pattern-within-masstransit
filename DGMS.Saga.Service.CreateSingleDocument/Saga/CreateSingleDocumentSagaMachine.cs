using Automatonymous;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using DGMS.CrossCutting.Configuration;
using DGMS.CrossCutting.Messages;
using DGMS.CrossCutting.Messages.Contract.SingleDocument;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DGMS.Saga.CreateSingleDocument.Saga
{
    public class CreateSingleDocumentSagaMachine : MassTransitStateMachine<SingleDocumentSaga>
    {
        private readonly ContextualLogging contextualLogging;

        public CreateSingleDocumentSagaMachine(ContextualLogging contextualLogging)
        {
            this.contextualLogging = contextualLogging;
            InstanceState(s => s.CurrentState);
            DefineFaultedEvents();
            DefineStateBehaviour();
        }

        public Event<ICreateSingleDocumentSagaRequest> StartupSagaEvent { get; set; }
        public Event<ICollateDataCompleted> CollateDataCompletedEvent { get; set; }
        public Event<IConvertToHtmlCompleted> ConvertToHtmlCompletedEvent { get; set; }
        public Event<IHtmlToPdfCompleted> HtmlToPdfCompletedEvent { get; set; }
        public Event<IUploadToStorageCompleted> UploadToStorageCompletedEvent { get; set; }
        public Event<Fault<ICollateDataRequest>> CollateDataFaultEvent { get; set; }
        public Event<Fault<ICollateDataCompleted>> ConvertToHtmlFaultEvent { get; set; }
        public Event<Fault<IConvertToHtmlCompleted>> HtmlToPdfFaultEvent { get; set; }
        public Event<Fault<IHtmlToPdfCompleted>> UploadToStorageFaultEvent { get; set; }

        public State CollatingDataState { get; set; }
        public State CreatingHtmlState { get; set; }
        public State CreatingPdfState { get; set; }
        public State UploadingToStorageState { get; set; }

        public virtual void DefineFaultedEvents()
        {

            Event(() => CollateDataFaultEvent, configureEventCorrelation => configureEventCorrelation.CorrelateById(context => context.Message.Message.CorrelationId));

            Event(() => ConvertToHtmlFaultEvent, configureEventCorrelation => configureEventCorrelation.CorrelateById(context => context.Message.Message.CorrelationId));

            Event(() => HtmlToPdfFaultEvent, configureEventCorrelation => configureEventCorrelation.CorrelateById(context => context.Message.Message.CorrelationId));

            Event(() => UploadToStorageFaultEvent, configureEventCorrelation => configureEventCorrelation.CorrelateById(context => context.Message.Message.CorrelationId));
        }

        public virtual void DefineStateBehaviour()
        {
            Initially(When(StartupSagaEvent)
                .ThenAsync(HydrateInstance)
                .Then(contextualLogging.LogIncomingMessage)
                .TransitionTo(CollatingDataState)
                .ThenAsync(StartCollatingData)
                );

            During(CollatingDataState, When(CollateDataCompletedEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .TransitionTo(CreatingHtmlState)
                .ThenAsync(StartCreatingHtml)
                );

            During(CollatingDataState, When(CollateDataFaultEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
                .ThenAsync(OnDocumentError)
                );

            During(CreatingHtmlState, When(ConvertToHtmlCompletedEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .TransitionTo(CreatingPdfState)
                .ThenAsync(StartCreatingPdf)
                );

            During(CreatingHtmlState, When(ConvertToHtmlFaultEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
                .ThenAsync(OnDocumentError)
                );

            During(CreatingPdfState, When(HtmlToPdfCompletedEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .TransitionTo(UploadingToStorageState)
                .ThenAsync(StartUploadingToStorage)
                );

            During(CreatingPdfState, When(HtmlToPdfFaultEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
                .ThenAsync(OnDocumentError)
                );

            During(UploadingToStorageState, When(UploadToStorageCompletedEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
                .ThenAsync(SendResultToParentSaga)
                );

            During(UploadingToStorageState, When(UploadToStorageFaultEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
                .ThenAsync(OnDocumentError)
                );

            States.ToList().ForEach(c => Events.ToList().ForEach(k => During(c, When(k).Then(Configure.NOOP))));

            var generator = new StateMachineGraphvizGenerator(this.GetGraph());
            contextualLogging.LogInformation("Graph for SAGA is {graph}", generator.CreateDotFile());

        }

        private async Task SendResultToParentSaga(BehaviorContext<SingleDocumentSaga, IUploadToStorageCompleted> context)
        {
            await context.Publish<SingleDocumentSaga, ICreateSingleDocumentSagaCompleted>(new
            {
                CorrelationId = context.Instance.ParentSagaCorrelationId,
                ChildSagaCorrelationId = context.Instance.CorrelationId,
                context.Instance.DocumentMetadata.DocumentId,
                context.Data.Uri
            });
        }

        private async Task OnDocumentError<T>(BehaviorContext<SingleDocumentSaga, Fault<T>> context) where T : class
        {
            await context.Publish<SingleDocumentSaga, Fault<ICreateSingleDocumentSagaFaulted>>(new
            {
                Message = new
                {
                    CorrelationId = context.Instance.ParentSagaCorrelationId,
                    ChildSagaCorrelationId = context.Instance.CorrelationId,
                    FaultedIn = GetType().Assembly.GetName().Name,
                    FaultDetails = context.Data.Message
                        .GetType().FullName + context.Data.Exceptions.Aggregate("", (a, b) => a + b.Message + b.StackTrace),
                   
                }
            });
        }

        private async Task StartUploadingToStorage(BehaviorContext<SingleDocumentSaga, IHtmlToPdfCompleted> context)
        {
            await BehaviorContextExtensions.Publish<SingleDocumentSaga, IHtmlToPdfCompleted>(context, (object)context.Data);
        }

        private async Task StartCreatingPdf(BehaviorContext<SingleDocumentSaga, IConvertToHtmlCompleted> context)
        {
            await BehaviorContextExtensions.Publish<SingleDocumentSaga, IConvertToHtmlCompleted>(context, (object)context.Data);
        }

        private async Task StartCollatingData(BehaviorContext<SingleDocumentSaga, ICreateSingleDocumentSagaRequest> context)
        {
            await context.Publish<SingleDocumentSaga, ICollateDataRequest>(new
            {
                context.Instance.CorrelationId,
                context.Instance.DocumentMetadata
            });
        }
        private async Task StartCreatingHtml(BehaviorContext<SingleDocumentSaga, ICollateDataCompleted> context)
        {
            await BehaviorContextExtensions.Publish<SingleDocumentSaga, ICollateDataCompleted>(context, (object)context.Data);
        }
        protected internal virtual async Task HydrateInstance(BehaviorContext<SingleDocumentSaga, ICreateSingleDocumentSagaRequest> context)
        {
            var message = context.Data;
            var instance = context.Instance;

            instance.CorrelationId = message.CorrelationId;
            instance.DocumentMetadata = message.DocumentMetadata;
            instance.ParentSagaCorrelationId = message.ParentSagaCorrelationId;
         }
    }
}

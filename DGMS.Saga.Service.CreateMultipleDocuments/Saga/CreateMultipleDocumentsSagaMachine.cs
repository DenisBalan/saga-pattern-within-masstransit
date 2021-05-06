using Automatonymous;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using DGMS.CrossCutting.Configuration;
using DGMS.CrossCutting.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DGMS.Saga.CreateMultipleDocuments.Saga
{
    public class CreateMultipleDocumentsSagaMachine : MassTransitStateMachine<MultipleDocumentsSaga>
    {
        private readonly ContextualLogging contextualLogging;

        public CreateMultipleDocumentsSagaMachine(ContextualLogging contextualLogging)
        {
            this.contextualLogging = contextualLogging;
            InstanceState(s => s.CurrentState);
            DefineFaultedEvents();
            DefineStateBehaviour();
        }


        public Event<ICreateMultipleDocumentsSagaRequest> StartupSagaEvent { get; set; }
        public Event<IStartCreatingDocumentsRequest> StartCreatingDocumentsEvent { get; set; }
        public Event<ICreateSingleDocumentSagaCompleted> SingleDocumentSagaCompleteEvent { get; set; }
        public Event<IAllDocumentSagasCompleted> AllInnerDocumentSagasCompleteEvent { get; set; }

        public Event<Fault<ICreateSingleDocumentSagaFaulted>> SingleDocumentFaulted { get; set; }


        public State CreatingDocuments { get; set; }
        public State InitialState { get; set; }

        public virtual void DefineFaultedEvents()
        {

            Event(() => SingleDocumentFaulted, configureEventCorrelation => configureEventCorrelation.CorrelateById(context => context.Message.Message.CorrelationId));
        }

        protected virtual void DefineStateBehaviour()
        {
            Initially(When(StartupSagaEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .ThenAsync(HydrateInstance)
                .TransitionTo(InitialState)
                .ThenAsync(context => context.Publish<MultipleDocumentsSaga, IStartCreatingDocumentsRequest>(new { context.Instance.CorrelationId }))
                );

            During(InitialState, When(StartCreatingDocumentsEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Then(StartPublishingCreateDocumentSagaMessages)
                .TransitionTo(CreatingDocuments)
                );

            During(CreatingDocuments, When(SingleDocumentSagaCompleteEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Then(DocumentItemCompleted)
                .Then(CheckIfAnyDocumentsAreStillCreating)
                .TransitionTo(CreatingDocuments)
                );

            During(CreatingDocuments, When(SingleDocumentFaulted)
                .Then(contextualLogging.LogIncomingMessage)
                .Then(DocumentItemFaulted)
                .Then(MarkAllDocumentsAsCompleted)
                .Finalize()
               .Then(context => context.Publish<MultipleDocumentsSaga, ICreateMultipleDocumentsCompleted>(new
               {
                   context.Instance.CorrelationId,
                   Status = "error",
                   Uris = context.Instance.Documents.Select(c => string.IsNullOrEmpty(c.Uri) ? "(empty)" : c.Uri),
               }))
                );

            During(CreatingDocuments, When(AllInnerDocumentSagasCompleteEvent)
                .Then(contextualLogging.LogIncomingMessage)
                .Finalize()
               .Then(context => context.Publish<MultipleDocumentsSaga, ICreateMultipleDocumentsCompleted>(new
               {
                   context.Instance.CorrelationId,
                   Status = "finished",
                   Uris = context.Instance.Documents.Select(c => c.Uri)
               }))
                );

            States.ToList().ForEach(c => Events.ToList().ForEach(k => During(c, When(k).Then(Configure.NOOP))));

            var generator = new StateMachineGraphvizGenerator(this.GetGraph());
            contextualLogging.LogInformation("Graph for SAGA is {graph}", generator.CreateDotFile());
        }

        protected internal virtual void StartPublishingCreateDocumentSagaMessages(BehaviorContext<MultipleDocumentsSaga, IStartCreatingDocumentsRequest> context)
        {
            context.Instance.Documents.ToList()
                .ForEach(async document =>
                {

                    var createDocumentSagaMessage = new
                    {
                        document.CorrelationId,
                        DocumentMetadata = document,
                        ParentSagaCorrelationId = context.Instance.CorrelationId
                    };

                    await context.Publish<MultipleDocumentsSaga, ICreateSingleDocumentSagaRequest>(createDocumentSagaMessage)
                        .ConfigureAwait(false);
                });
        }
        protected internal virtual async Task HydrateInstance(BehaviorContext<MultipleDocumentsSaga, ICreateMultipleDocumentsSagaRequest> context)
        {
            var message = context.Data;
            var instance = context.Instance;

            instance.CorrelationId = message.CorrelationId;
            instance.ParentSagaCorrelationId = message.ParentSagaCorrelationId;
            instance.InitiatedBy = message.InitiatedBy;

            instance.Documents = message.Documents.Select(x => new DomainDocumentMetadata
            {

                CorrelationId = NewId.NextGuid(),
                Complete = default,

                // could use automapper
                DocumentId = x.DocumentId,
                ForAsset = x.ForAsset,
                FullName = x.FullName,
            });
        }
        protected internal virtual void DocumentItemCompleted(BehaviorContext<MultipleDocumentsSaga, ICreateSingleDocumentSagaCompleted> context)
        {
            var sagaId = context.Data.ChildSagaCorrelationId;

            var document = context.Instance.Documents.Single(d => d.CorrelationId == sagaId);
            document.Complete = true;
            document.Uri = context.Data.Uri;
        }
        protected internal virtual void DocumentItemFaulted(BehaviorContext<MultipleDocumentsSaga, Fault<ICreateSingleDocumentSagaFaulted>> context)
        {
            var sagaId = context.Data.Message.ChildSagaCorrelationId;

            var document = context.Instance.Documents.Single(d => d.CorrelationId == sagaId);
            document.Complete = true;
            document.Uri = context.Data.Message.FaultDetails; // could be improved
        }
        protected internal virtual void MarkAllDocumentsAsCompleted(BehaviorContext<MultipleDocumentsSaga, Fault<ICreateSingleDocumentSagaFaulted>> context)
        {
            foreach(var doc in context.Instance.Documents)
            {
                doc.Complete = true;
            }
        }

        protected internal virtual async void CheckIfAnyDocumentsAreStillCreating(BehaviorContext<MultipleDocumentsSaga, ICreateSingleDocumentSagaCompleted> context)
        {
            if (!context.Instance.Documents.All(d => d.Complete))
                return;

            await context.Publish<MultipleDocumentsSaga, IAllDocumentSagasCompleted>(new
            {
                context.Instance.CorrelationId
            });
        }
    }
}

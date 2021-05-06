using Automatonymous;
using DGMS.CrossCutting.Messages;
using MassTransit.Saga;
using System;
using System.Collections.Generic;

namespace DGMS.Saga.CreateSingleDocument
{
    public class SingleDocumentSaga : SagaStateMachineInstance, ISagaVersion
    {
        public DocumentMetadata DocumentMetadata { get; set; }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid? ParentSagaCorrelationId { get; set; }
        public int Version { get; set; } = 1;
    }
}

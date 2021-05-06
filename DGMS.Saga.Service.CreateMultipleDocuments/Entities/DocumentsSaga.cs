using Automatonymous;
using DGMS.CrossCutting.Messages;
using MassTransit.Saga;
using System;
using System.Collections.Generic;

namespace DGMS.Saga.CreateMultipleDocuments
{
    public class MultipleDocumentsSaga : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string InitiatedBy { get; set; }
        public virtual IEnumerable<DomainDocumentMetadata> Documents { get; set; }
        public Guid? ParentSagaCorrelationId { get; set; }
        public int Version { get; set; } = 1;
    }
}

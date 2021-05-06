using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages
{
    public interface ICreateSingleDocumentSagaCompleted : CorrelatedBy<Guid>
    {
        string DocumentId { get; set; }
        string Uri { get; set; }
        Guid ChildSagaCorrelationId { get; set; }
    }
}
using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages
{
    public interface ICreateSingleDocumentSagaFaulted : CorrelatedBy<Guid>
    {
        string FaultedIn { get; set; }
        string FaultDetails { get; set; }
        Guid ChildSagaCorrelationId { get; set; }
    }
}
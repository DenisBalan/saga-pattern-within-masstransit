using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages
{
    public interface ICreateSingleDocumentSagaRequest : CorrelatedBy<Guid>
    {
        Guid? ParentSagaCorrelationId { get; set; }
        DocumentMetadata DocumentMetadata { get; set; }
    }
}
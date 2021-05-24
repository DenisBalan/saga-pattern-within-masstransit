using MassTransit;
using System;
using System.Collections.Generic;

namespace DGMS.CrossCutting.Messages
{
    public interface ICreateMultipleDocumentsSagaRequest: CorrelatedBy<Guid>
    {
        Guid? ParentSagaCorrelationId { get; set; }
        string InitiatedBy { get; set; }
        IEnumerable<DocumentMetadata> Documents { get; set; }
    }
}
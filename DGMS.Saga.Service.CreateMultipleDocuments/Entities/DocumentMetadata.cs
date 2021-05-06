using DGMS.CrossCutting.Messages;
using System;

namespace DGMS.Saga.CreateMultipleDocuments
{
    public class DomainDocumentMetadata: DocumentMetadata
    {
        public bool Complete { get; set; }
        public string Uri { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
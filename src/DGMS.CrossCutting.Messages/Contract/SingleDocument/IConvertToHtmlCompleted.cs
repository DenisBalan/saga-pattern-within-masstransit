using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages.Contract.SingleDocument
{
    public interface IConvertToHtmlCompleted : CorrelatedBy<Guid>
    {
        string RenderedHtml { get; set; }
    }
}

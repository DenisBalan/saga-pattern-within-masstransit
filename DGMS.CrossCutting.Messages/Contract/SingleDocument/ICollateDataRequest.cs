using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGMS.CrossCutting.Messages.Contract.SingleDocument
{
    public interface ICollateDataRequest : CorrelatedBy<Guid>
    {
        DocumentMetadata DocumentMetadata { get; set; }
    }
}

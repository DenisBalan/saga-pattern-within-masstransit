using MassTransit;
using System;
using System.Collections.Generic;

namespace DGMS.CrossCutting.Messages
{
    public interface ICollateDataCompleted : CorrelatedBy<Guid>
    {
        DomainDocumentBindingValues DataBinding { get; set; }
    }
}
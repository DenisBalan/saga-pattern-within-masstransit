using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages
{
    public interface IStartCreatingDocumentsRequest : CorrelatedBy<Guid>
    {

    }
}
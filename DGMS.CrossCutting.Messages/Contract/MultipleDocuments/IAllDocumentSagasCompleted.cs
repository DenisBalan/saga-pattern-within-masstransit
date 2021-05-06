using MassTransit;
using System;

namespace DGMS.CrossCutting.Messages
{
    public interface IAllDocumentSagasCompleted : CorrelatedBy<Guid>
    {

    }
}
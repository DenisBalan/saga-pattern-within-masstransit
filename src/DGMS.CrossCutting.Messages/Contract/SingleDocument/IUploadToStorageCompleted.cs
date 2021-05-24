using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGMS.CrossCutting.Messages.Contract.SingleDocument
{
    public interface IUploadToStorageCompleted : CorrelatedBy<Guid>
    {
        string Uri { get; set; }
    }
}

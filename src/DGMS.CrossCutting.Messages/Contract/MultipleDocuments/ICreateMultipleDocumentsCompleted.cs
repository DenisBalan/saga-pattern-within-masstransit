using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGMS.CrossCutting.Messages
{
    public interface ICreateMultipleDocumentsCompleted: CorrelatedBy<Guid>
    {
        string Status { get; set; }
        string[] Uris { get; set; }
    }
}

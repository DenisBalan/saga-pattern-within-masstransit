using DGMS.CrossCutting.Messages;
using DGMS.CrossCutting.Messages.Contract.SingleDocument;
using MassTransit;
using Storage.Net.Blobs;
using System.Text;
using System.Threading.Tasks;

namespace DGMS.Service.UploadToStorage
{
    public class UploadToStorageConsumer : IConsumer<IHtmlToPdfCompleted>
    {
        private readonly IBlobStorage blobStorage;

        public UploadToStorageConsumer(IBlobStorage blobStorage)
        {
            this.blobStorage = blobStorage;
        }
        public async Task Consume(ConsumeContext<IHtmlToPdfCompleted> context)
        {
            string fullPath = $"documents/single-document-saga-{context.Message.CorrelationId}.pdf";
            await blobStorage.WriteAsync(fullPath, context.Message.PdfBytes);
            await context.Publish<IUploadToStorageCompleted>(new
            {
                context.Message.CorrelationId,
                Uri = fullPath
            });
        }
    }
}

using DGMS.CrossCutting.Messages;
using DGMS.CrossCutting.Messages.Contract.SingleDocument;
using MassTransit;
using System.Text;
using System.Threading.Tasks;
using NGenHtmlToPdf = PugPdf.Core.HtmlToPdf;

namespace DGMS.Service.HtmlToPdf
{
    public class HtmlToPdfConsumer : IConsumer<IConvertToHtmlCompleted>
    {
        private readonly NGenHtmlToPdf htmlToPdfConverter;

        public HtmlToPdfConsumer(NGenHtmlToPdf htmlToPdfConverter)
        {
            this.htmlToPdfConverter = htmlToPdfConverter;
        }
        public async Task Consume(ConsumeContext<IConvertToHtmlCompleted> context)
        {
            await context.Publish<IHtmlToPdfCompleted>(new
            {
                context.Message.CorrelationId,
                PdfBytes = (await htmlToPdfConverter.RenderHtmlAsPdfAsync(context.Message.RenderedHtml)).BinaryData
            });
        }
    }
}

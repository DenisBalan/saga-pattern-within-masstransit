using DGMS.CrossCutting.Messages;
using DGMS.CrossCutting.Messages.Contract.SingleDocument;
using MassTransit;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGMS.Service.ConvertToHtml
{
    public class ConvertToHtmlConsumer : IConsumer<ICollateDataCompleted>
    {
        static string Template = $@"
<html>
<img width=150px src='@Model.{nameof(DomainDocumentBindingValues.CompanyImageUrl)}' />
<img width=400px src='@Model.{nameof(DomainDocumentBindingValues.AssetImageUrl)}' />

<hr/>
quote from @Model.{nameof(DomainDocumentBindingValues.FromCompany)}
<hr/>
for asset @Model.{nameof(DomainDocumentBindingValues.ForAsset)} [ <span style=""font-size:28px""> @Model.{nameof(DomainDocumentBindingValues.FullName)} </span> ]
<hr/>
<table border=1>
<thead>
<tr>
    <th>{nameof(DomainDocumentBindingValues.Line.BrokerName)}</th>
    <th>{nameof(DomainDocumentBindingValues.Line.Details)}</th>
    <th>{nameof(DomainDocumentBindingValues.Line.PhoneNumber)}</th>
</tr>
</thead>
@foreach (var item in Model.{nameof(DomainDocumentBindingValues.QuoteDetails)})
{{
<tr>
    <td>
        <div>@item.{nameof(DomainDocumentBindingValues.Line.BrokerName)}</div>
    </td>
    <td>
        <div>@item.{nameof(DomainDocumentBindingValues.Line.Details)}</div>
    </td>
    <td>
        <div>@item.{nameof(DomainDocumentBindingValues.Line.PhoneNumber)}</div>
    </td>
</tr>
}}
</table>
<b>document id <i>@Model.{nameof(DomainDocumentBindingValues.DocumentId)}</i></b>
</html>
";
        private readonly IRazorEngineService razorEngineService;

        public ConvertToHtmlConsumer(IRazorEngineService razorEngineService)
        {
            this.razorEngineService = razorEngineService;
        }
        public async Task Consume(ConsumeContext<ICollateDataCompleted> context)
        {
            if (context.Message.DataBinding.DocumentId == "11")
            {
                //throw new Exception("FAKE EXCEPTION");
            }
            await context.Publish<IConvertToHtmlCompleted>(new
            {
                context.Message.CorrelationId,
                RenderedHtml = razorEngineService.RunCompile(templateSource: Template, name: nameof(ConvertToHtml), model: context.Message.DataBinding)
            });
        }
    }
}

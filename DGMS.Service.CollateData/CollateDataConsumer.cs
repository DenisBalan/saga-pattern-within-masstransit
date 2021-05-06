using DGMS.CrossCutting.Messages;
using DGMS.CrossCutting.Messages.Contract.SingleDocument;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGMS.Service.CollateData
{
    public class CollateDataConsumer : IConsumer<ICollateDataRequest>
    {
        public async Task Consume(ConsumeContext<ICollateDataRequest> context)
        {
            // retrive data from db
            var fromCompany = new[] { "rapidasig", "moldasig" }[DateTime.Now.Ticks % 2];
            await context.Publish<ICollateDataCompleted>(new
            {
                context.Message.CorrelationId,
                DataBinding = new DomainDocumentBindingValues
                {
                    DocumentId = context.Message.DocumentMetadata.DocumentId,
                    ForAsset = context.Message.DocumentMetadata.ForAsset,
                    AssetImageUrl = context.Message.DocumentMetadata.ForAsset.ToLowerInvariant() switch
                    {
                        "home" => "https://www.bhgre.com/bhgrelife/wp-content/uploads/2014/12/dreamhome.jpg",
                        "car" => "https://www.extremetech.com/wp-content/uploads/2019/12/SONATA-hero-option1-764A5360-edit.jpg",
                        _ => "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyMdxj_fiEcrlZ7Ii5YFMyfWOMqrrHDAl65A&usqp=CAU"
                    },
                    FullName = context.Message.DocumentMetadata.FullName,

                    FromCompany = fromCompany,

                    CompanyImageUrl = fromCompany.ToLowerInvariant() switch
                    {
                        "rapidasig" => "https://rapidasig.md/img/logo_fb.jpg",
                        "moldasig" => "https://www.ipn.md/storage/ckfinder/images/cover_images/2019_07_18/1066890__5d305a95ddece.jpg",
                        _ => "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyMdxj_fiEcrlZ7Ii5YFMyfWOMqrrHDAl65A&usqp=CAU"
                    },
                    QuoteDetails = new[]
                    {
                        new DomainDocumentBindingValues.Line
                        {
                            BrokerName = "Vasile",
                            Details = "buy today!",
                            PhoneNumber = "061-553-322",
                        },
                        new DomainDocumentBindingValues.Line
                        {
                            BrokerName = "Ion",
                            Details = "right now!",
                            PhoneNumber = "061-553-578",
                        },
                    }
                }
            });
        }
    }
}

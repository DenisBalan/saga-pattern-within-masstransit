namespace DGMS.CrossCutting.Messages
{
    public class DomainDocumentBindingValues : DocumentMetadata
    {
        public string CompanyImageUrl { get; set; }
        public string FromCompany { get; set; }
        public string AssetImageUrl { get; set; }
        public class Line
        {
            public string BrokerName { get; set; }
            public string Details { get; set; }
            public string PhoneNumber { get; set; }

        }
        public Line[] QuoteDetails { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace ParserLesegaisRu.DbWork
{
    public class Declaration
    {
        public Data Data;
    }


    public class Data
    {
        public SearchReportWoodDeal SearchReportWoodDeal;
    };


    public class SearchReportWoodDeal
    {
        public Deal[] Content;
    }


    [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
    public class Deal
    {
        public string DealNumber;
        public string SellerName;
        public string SellerInn;
        public decimal WoodVolumeSeller;
        public string BuyerName;
        public string BuyerInn;
        public decimal WoodVolumeBuyer;
        public string DealDate;
    }
}

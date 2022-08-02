using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLesegaisRu
{
    public class Declaration
    {

        //                    {[,]
        //    }
        //            {[, Физическое лицо]
        //}
        //{[, 2022 - 07 - 30]}
        //{[, 0025000000000000261507356715]}
        //{[, 261507356715]}
        //{[, ИП Остапенко Валерий Валерьевич]}
        //{[, 0]}
        //{[, 4.41]}

        public static string DealNumberSiteColumnName = "dealNumber";
        public static string SellerNameSiteColumnName= "sellerName";
        public static string SellerINNSiteColumnName = "sellerInn";
        public static string SellerVolumeSiteColumnName = "woodVolumeSeller";
        public static string BuyerNameSiteColumnName = "buyerName";
        public static string BuyerINNSiteColumnName = "buyerInn";
        public static string BuyerVolumeSiteColumnName = "woodVolumeBuyer";
        public static string DealDateSiteColumnName = "dealDate";


        public string DealNumber;
        public string DealDate;
        public string SellerName;
        public string SellerINN;
        public string SellerVolume;
        public string BuyerName;
        public string BuyerINN;
        public string BuyerVolume;

    }
}

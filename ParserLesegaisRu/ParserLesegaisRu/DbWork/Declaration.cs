using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
        public string DealNumber { get; set; }
        public string SellerName { get; set; }
        public string SellerInn { get; set; }
        public decimal WoodVolumeSeller { get; set; }
        public string BuyerName { get; set; }
        public string BuyerInn { get; set; }
        public decimal WoodVolumeBuyer { get; set; }
        public string DealDate { get; set; }

        public bool IsValid()
        {
            return (ValidateDealNumber(this.DealNumber)
                && ValidateDealInn(this.SellerInn)
                && ValidateDealInn(this.BuyerInn)
                && ValidateDealDate(this.DealDate));
        }

        //номер сделки не должен быть пустым и должен состоять из цифр
        private bool ValidateDealNumber(string dealNumber)
        {
            return (!string.IsNullOrWhiteSpace(dealNumber) && Regex.IsMatch(dealNumber, @"^\d+$"));
        }


        private bool ValidateDealInn(string Inn)
        {
            //возможная длина поля ИНН равна 10 или 12 символов
            const int InnPossibleLengthv1 = 10;
            const int InnPossibleLengthv2 = 12;

            if ((Inn != null))
            {
                if (string.IsNullOrWhiteSpace(Inn) ||
                    (Inn.Length == InnPossibleLengthv1 || Inn.Length == InnPossibleLengthv2) && (Regex.IsMatch(Inn, @"^\d+$")))
                {
                    return true;
                }
            }

            return false;
        }


        //возможно ли преобразовать строку к дате
        private bool ValidateDealDate(string dealDate)
        {
            return DateTime.TryParseExact(dealDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

    }
}


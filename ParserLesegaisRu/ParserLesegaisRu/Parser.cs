using System.Net.Http;
using System;
using System.Text.Json;
using System.Net.Http.Json;
using ParserLesegaisRu.DbWork;


namespace ParserLesegaisRu
{
    internal class Parser
    {
        private const string ContentType = @"application/json";
        private const string UserAgentTitle = @"User-Agent";
        private const string UserAgent = @"Web Agent 007";
        private const string Host = @"lesegais.ru";
        private const string RequestBodyStart = @"{""query"":""query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    content {\n      sellerName\n      sellerInn\n      buyerName\n      buyerInn\n      woodVolumeBuyer\n      woodVolumeSeller\n      dealDate\n      dealNumber\n  }\n    __typename\n  }\n}\n"",""variables"":{";
        private const string RequestBodyData = "\"size\":{0},\"number\":{1}";
        private const string RequestBodyEnd = @",""filter"":null,""orders"":null},""operationName"":""SearchReportWoodDeal""}";

        private readonly string _pageUrl;
        private readonly HttpClient _httpClient = new();

        public Parser(string pageUrl)
        {
            _pageUrl = pageUrl;
        }


        public bool TryRequestData(int pageNumber, int amountRecords, out Deal[] dealsDataList)
        {
            var isFetched = false;
            dealsDataList = null;

            try
            {
                var requestBody = RequestBodyStart + string.Format(RequestBodyData, amountRecords, pageNumber) + RequestBodyEnd;

                HttpContent httpContent = new StringContent(requestBody);

                httpContent.Headers.ContentType.MediaType = ContentType;
                _httpClient.DefaultRequestHeaders.Add(UserAgentTitle, UserAgent);
                _httpClient.DefaultRequestHeaders.Host = Host;

                HttpResponseMessage response = (_httpClient.PostAsync(_pageUrl, httpContent)).Result;

                response.EnsureSuccessStatusCode();

                var declarationData = (response.Content.ReadFromJsonAsync<Declaration>(new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                })).Result;

                dealsDataList = declarationData.Data.SearchReportWoodDeal.Content;

                isFetched = (dealsDataList.Length > 0);
            }
            catch (JsonException e) // Invalid JSON
            {
                Console.WriteLine("Invalid JSON." + e.Message);
                throw;
            }

            return isFetched;
        }
    }
}
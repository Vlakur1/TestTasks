using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ParserLesegaisRu
{
    sealed class Parser : IDisposable
    {
        private readonly ChromeDriver _chromeDriver;
        private readonly string _pageUrl;
        private bool _disposed = false;


        public Parser(string pageUrl)
        {
            _chromeDriver = CreateChromeDriver();
            _pageUrl = pageUrl;
            _chromeDriver.Navigate().GoToUrl(_pageUrl);
        }


        public ChromeDriver CreateChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
            chromeOptions.AddArguments("--headless");
            chromeOptions.AddArguments("--disable-logging");
            chromeOptions.AddArguments("--log-level=3");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--disable-crash-reporter");
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-in-process-stack-traces");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--output=/dev/null");

            return new ChromeDriver(chromeOptions);
        }


        public bool TryFetchPageRecords(int pageNumber, int amountRecords, out List<Declaration> dealsDataList)
        {
            
            var isFetched = false;
            dealsDataList = new List<Declaration>();

            var scriptText = @"return " +
                          @"fetch('/open-area/graphql'," +
                          @" { method: 'POST'," +
                          @" headers: {'Content-Type': 'application/json'}," +
                          @"  body:""{\""query\"":\""query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\\n    content {\\n      sellerName\\n      sellerInn\\n      buyerName\\n      buyerInn\\n      woodVolumeBuyer\\n      woodVolumeSeller\\n      dealDate\\n      dealNumber\\n      __typename\\n    }\\n    __typename\\n  }\\n}\\n\"",\""variables\"":{\""size\"":" + amountRecords + @",\""number\"":" + pageNumber + @",\""filter\"":null,\""orders\"":null},\""operationName\"":\""SearchReportWoodDeal\""}""})"
                          + ".then(response => response.json())";

            try
            {
                var fetchedData = _chromeDriver.ExecuteScript(scriptText);

                var dealsList = (ReadOnlyCollection<object>)((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)fetchedData)["data"])["searchReportWoodDeal"])["content"];

                var declarationData = new Declaration();

                foreach (Dictionary<string, object> deal in dealsList)
                {
                    declarationData = new Declaration()
                    {
                        DealNumber = deal[Declaration.DealNumberSiteColumnName]?.ToString() ?? "",
                        DealDate = deal[Declaration.DealDateSiteColumnName]?.ToString() ?? null,
                        SellerName = deal[Declaration.SellerNameSiteColumnName]?.ToString() ?? "",
                        SellerINN = deal[Declaration.SellerINNSiteColumnName]?.ToString() ?? "",
                        BuyerName = deal[Declaration.BuyerNameSiteColumnName]?.ToString() ?? "",
                        BuyerINN = deal[Declaration.BuyerINNSiteColumnName]?.ToString() ?? "",
                        SellerVolume = deal[Declaration.SellerVolumeSiteColumnName]?.ToString() ?? "",
                        BuyerVolume = deal[Declaration.BuyerVolumeSiteColumnName]?.ToString() ?? ""
                    };
                    dealsDataList.Add(declarationData);
                }
                isFetched = (dealsDataList.Count > 0);
            }
            catch
            {
                throw;
            }

            return isFetched;
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                if (_chromeDriver != null)
                {
                    _chromeDriver.Quit();
                    _chromeDriver.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
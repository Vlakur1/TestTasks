using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ParserLesegaisRu
{
    class Parser : IDisposable
    {
        private const int FirstPage = 1;
        private const int CountColumn = 7;
        private int _nextPage = FirstPage;
        private readonly ChromeDriver _chromeDriver;
        private readonly string _pageUrl;

        private readonly List<string> _listColumnId = new List<string>
                                                           { "dealNumber",
                                                            "sellerName",
                                                            "sellerInn",
                                                            "buyerName",
                                                            "buyerInn",
                                                            "dealDate",
                                                            "0" };

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

            return new ChromeDriver(chromeOptions);

        }

        private bool TryGetPaginationRecordsInfo(out int paganationFromRecord, out int paganationToRecord)
        {
            var selectorPaginationRecordsInfo = By.CssSelector("div.react-pagination-toolbar > span:nth-child(9)");
            paganationFromRecord = 0;
            paganationToRecord = 0;

            var PaginationString = _chromeDriver.FindElement(selectorPaginationRecordsInfo);
            if (PaginationString != null && !string.IsNullOrWhiteSpace(PaginationString?.Text))
            {
                var paginationData = PaginationString.Text.Split(' ');
                if (paginationData.Length == 8
                    && int.TryParse(paginationData[3], out paganationFromRecord)
                    && int.TryParse(paginationData[5].Substring(0, paginationData[5].Length - 1), out paganationToRecord))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryGetPaginationPageInfo(out int paganationCurrentPage, out int paganationLastPage)
        {
            const int CountWordsInPaginationPagesContent = 4;
            const int NumWordCurrentPage = 1;
            const int NumWordTotlalPage = 3;
            const string paginationPageSelectorText = "div.react-pagination-toolbar.x-toolbar > span:nth-child(4)";

            var paginationPageSelector = By.CssSelector(paginationPageSelectorText);
            paganationCurrentPage = 0;
            paganationLastPage = 0;

            var PaginationPagesContent = _chromeDriver.FindElement(paginationPageSelector);
            if (PaginationPagesContent != null && !string.IsNullOrWhiteSpace(PaginationPagesContent?.Text))
            {
                var paginationData = PaginationPagesContent.Text.Split(' ');
                if (paginationData.Length == CountWordsInPaginationPagesContent
                    && int.TryParse(paginationData[NumWordCurrentPage], out paganationCurrentPage)
                    && int.TryParse(paginationData[NumWordTotlalPage], out paganationLastPage))
                {
                    return true;
                }
            }
            return false;
        }



        private List<Declaration> GetPageData(ChromeDriver chromeDriver, string pageUrl)
        {
            Console.WriteLine(Environment.NewLine + "Ждем загрузку страницы: " + (_nextPage - 1) + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

            var wait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(100));
            var selectorDataCells = By.CssSelector("div.ag-cell");
            var dealsDataList = new List<Declaration>();

            var selectorPaginationRecordsInfo = By.CssSelector("div.react-pagination-toolbar > span:nth-child(9)");
            wait.Until(_ => TryGetPaginationRecordsInfo(out int paganationFromPage, out int paganationToPage) && paganationToPage > paganationFromPage);

            Console.WriteLine("Старт парсинга страницы: " + (_nextPage - 1) + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
           
            var declarationData = new Declaration();
            var dataCells = _chromeDriver.FindElements(selectorDataCells);
            for (var i = 0; i < dataCells.Count - 2 * CountColumn; i += CountColumn)
            {
                
                declarationData = new Declaration()
                {
                    DealNumber = dataCells[i + 0].Text,
                    SellerName = dataCells[i + 1].Text,
                    SellerINN = dataCells[i + 2].Text,
                    CustomerName = dataCells[i + 3].Text,
                    CustomerINN = dataCells[i + 4].Text,
                    DealDate = dataCells[i + 5].Text,
                    VolumeByReport = dataCells[i + 6].Text,
                };
                dealsDataList.Add(declarationData);
            }

            Console.WriteLine("Финиш парсинга страницы: " + (_nextPage - 1) + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + Environment.NewLine);

            return dealsDataList;
        }


        public bool GetNextPageData(out List<Declaration> dealsDataList)
        {
            bool result = false;
            dealsDataList = null;
            try
            {
                dealsDataList = GetPageData(_chromeDriver, _pageUrl);

                TryGetPaginationPageInfo(out int paganationCurrentPage, out int paganationLastPage);

                if (paganationCurrentPage != paganationLastPage)
                {
                    var selectorNextPageButton = By.CssSelector("span.x-btn-button>span.x-btn-icon-el.x-btn-icon-el-plain-toolbar-small.x-tbar-page-next");
                    var nextPageButton = _chromeDriver.FindElement(selectorNextPageButton);
                    nextPageButton.Click();

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exseption was thrown - " + e.Message + Environment.NewLine + e.Source + Environment.NewLine);
                return result;
            }
        }

        private List<Declaration> GetPageDataVer1(ChromeDriver chromeDriver, string pageUrl)
        {
            Console.WriteLine(Environment.NewLine + "Старт парсинга страницы: " + (_nextPage - 1) + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

            var dataRows = chromeDriver.FindElements(By.CssSelector("div.ag-body-container>div.ag-row"));

            var dealDataDictionary = new Dictionary<string, string>();
            var dealsDataList = new List<Declaration>();

            for (var i = 0; i < dataRows.Count - 2; i++)
            {
                var dataCells = dataRows[i].FindElements(By.CssSelector("div.ag-cell"));
                foreach (var cellValue in dataCells)
                {
                    var columnName = cellValue.GetAttribute("col-id");
                    if (_listColumnId.Contains(columnName))
                    {
                        dealDataDictionary.Add(columnName, cellValue.Text);
                        Console.WriteLine($"{cellValue.GetAttribute("col-id")} = {cellValue.Text}");
                    }
                }

                var declarationData = new Declaration()
                {
                    DealNumber = dealDataDictionary.TryGetValue(_listColumnId[0], out string dealNumber) ? dealNumber : null,
                    SellerName = dealDataDictionary.TryGetValue(_listColumnId[1], out string sellerName) ? sellerName : null,
                    SellerINN = dealDataDictionary.TryGetValue(_listColumnId[2], out string sellerINN) ? sellerINN : null,
                    CustomerName = dealDataDictionary.TryGetValue(_listColumnId[3], out string customerName) ? customerName : null,
                    CustomerINN = dealDataDictionary.TryGetValue(_listColumnId[4], out string customerINN) ? customerINN : null,
                    DealDate = dealDataDictionary.TryGetValue(_listColumnId[5], out string dealDate) ? dealDate : null,
                    VolumeByReport = dealDataDictionary.TryGetValue(_listColumnId[6], out string volumeByReport) ? volumeByReport : null,
                };
                dealsDataList.Add(declarationData);
                dealDataDictionary.Clear();
            }

            Console.WriteLine("Окончание парсинга страницы: " + (_nextPage - 1) + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + Environment.NewLine);

            return dealsDataList;
        }

        public bool GetNextPageDataVer1(out List<Declaration> dealsDataList)
        {
            bool result = false;
            dealsDataList = null;
            try
            {
                if (_nextPage == FirstPage)
                {
                    // var wait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(100000));
                    //_chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(200);
                    _chromeDriver.Navigate().GoToUrl(_pageUrl);

                    dealsDataList = GetPageData(_chromeDriver, _pageUrl);
                    _nextPage++;
                    result = true;
                }
                else
                {
                    //System.Threading.Thread.Sleep(4000);
                    var wait = new WebDriverWait(_chromeDriver, TimeSpan.FromSeconds(10));

                    var selectorNextPageButton = By.CssSelector("span.x-btn-button>span.x-btn-icon-el.x-btn-icon-el-plain-toolbar-small.x-tbar-page-next");
                    //var nextPageButton = _chromeDriver.FindElement(By.CssSelector("span.x-btn-button>span.x-btn-icon-el.x-btn-icon-el-plain-toolbar-small.x-tbar-page-next"));

                    var nextPageButton = wait.Until(drv => drv.FindElement(selectorNextPageButton));

                    //wait.Until(Exp s.elementToBeClickable(ById("element"))
                    nextPageButton.Click();
                    //wait.Until(drv => drv.Navigate());


                    //var cursorView = nextPageButton.GetCssValue("cursor");
                    //if (cursorView == "pointer")
                    //{

                    //var PaginationPageStr = _chromeDriver.FindElement(By.CssSelector("div.react-pagination-toolbar.x-toolbar > span:nth-child(4)"));

                    dealsDataList = GetPageData(_chromeDriver, _pageUrl);

                    var paginationPageSelector = By.CssSelector("div.react-pagination-toolbar.x-toolbar > span:nth-child(4)");
                    var PaginationPageStr = wait.Until(drv => drv.FindElement(paginationPageSelector));
                    var currentPage = int.Parse(PaginationPageStr.Text.Split(' ')[1]);

                    //int paganationCurrentPage;
                    //int paganationLastPage;
                    //wait.Until(_ => TryGetPaginationPageInfo(out paganationCurrentPage, out paganationLastPage));
                    TryGetPaginationPageInfo(out int paganationCurrentPage, out int paganationLastPage);

                    if (paganationCurrentPage != paganationLastPage)
                    {
                        _nextPage++;
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exseption was thrown - " + e.Message + Environment.NewLine + e.Source + Environment.NewLine);
                return result;
            }
        }



        public void Dispose()
        {
            _chromeDriver.Quit();
        }
    }
}
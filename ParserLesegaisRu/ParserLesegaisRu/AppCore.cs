using System;
using System.Collections.Generic;
using System.Threading;
using ParserLesegaisRu.DbWork;

namespace ParserLesegaisRu
{
    public class AppCore
    {
        private readonly DbWorker _dBWorker;
        private readonly Parser _parser;
        private const string UrlForParsing = @"https://www.lesegais.ru/open-area/deal";
        private const int PageSize = 10000;
        private const int SecondInMinute = 60;
        private const int WaitSecond = 600;
        private const int MilliseconsInSecond = 1000;

        private const string WelcomeMessage = "\n This App parses the Lesegais.Ru site, to exit - press any key\n";
        private const string WorkFailMessage = @"Program execution was terminated!";
        private const string WaitExecutionMessage = @"Waiting {0}  minutes {1} seconds until next site parsing, to exit - press any key.";
        private const string ElapsedTimeMessage = @"Elapsed - {0} sec";
        private const string CurentPageInformationTimeMessage = @"Page Num = {0}, Records: {1}-{2}";
        private const string StartWriteRecordsToDbMessage = @"Start adding rows to the database: ";
        private const string FinishWriteRecordsToDbMessage = @"Finish adding rows to the database: ";

        private int currentPage;
        private int counterOfSeconds;

        public AppCore()
        {
            _dBWorker = new DbWorker();
            _parser = new Parser(UrlForParsing);
        }


        public void StartApplication()
        {
            Console.WriteLine(WelcomeMessage);
            try
            {
                do
                {
                    currentPage = 0;

                    while (_parser.TryFetchPageRecords(currentPage, PageSize, out List<Declaration> dealsDataList) && Console.KeyAvailable == false)
                    {
                        Console.WriteLine(CurentPageInformationTimeMessage, currentPage+1, currentPage * PageSize, (currentPage+1) * PageSize);
                        Console.WriteLine(StartWriteRecordsToDbMessage + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

                        _dBWorker.AddDealRange(dealsDataList);
                        Console.WriteLine(FinishWriteRecordsToDbMessage + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + Environment.NewLine);

                        currentPage++;
                    }

                    Console.WriteLine(WaitExecutionMessage, WaitSecond / SecondInMinute, WaitSecond % SecondInMinute);
                    var consoleCursorTopPosition = Console.CursorTop;
                    counterOfSeconds = 0;

                    while (Console.KeyAvailable == false && counterOfSeconds < WaitSecond)
                    {
                        Console.SetCursorPosition(2, consoleCursorTopPosition);
                        Console.Write(ElapsedTimeMessage, ++counterOfSeconds);
                        Thread.Sleep(MilliseconsInSecond);
                    }
                    Console.WriteLine();

                } while (counterOfSeconds == WaitSecond && Console.KeyAvailable == false);
            }
            catch (Exception e)
            {
                Console.WriteLine(WorkFailMessage + Environment.NewLine);
                Console.WriteLine(e.Message + Environment.NewLine + e.Source + Environment.NewLine);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message + Environment.NewLine + innerException.Source);
                    Console.WriteLine();
                    innerException = innerException.InnerException;
                }
            }
            finally
            {
                _parser.Dispose();
            }
        }
    }
}


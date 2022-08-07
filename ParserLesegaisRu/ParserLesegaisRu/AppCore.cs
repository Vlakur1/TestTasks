using System;
using System.Threading;
using ParserLesegaisRu.DbWork;


namespace ParserLesegaisRu
{
    public class AppCore
    {
        private readonly DbWorker _dBWorker = new();
        private readonly Parser _parser = new(UrlForParsing);
        private const string UrlForParsing = @"https://www.lesegais.ru/open-area/graphql";
        private const int PageSize = 10000;
        private const int SecondsInMinute = 60;
        private const int WaitingInterval = 600;
        private const int MilliseconsInSecond = 1000;

        private const string WelcomeMessage = "\n This App parses the Lesegais.Ru site, to exit - press any key\n";
        private const string WorkFailMessage = @"Program execution was terminated!";
        private const string WaitExecutionMessage = @"Waiting {0}  minutes {1} seconds until next site parsing, to exit - press any key.";
        private const string ElapsedTimeMessage = @"Elapsed - {0} sec";
        private const string CurentPageInformationTimeMessage = @"Page Num = {0}, Records: {1}-{2}";
        private const string StartWriteRecordsToDbMessage = @"Start adding rows to the database: ";
        private const string FinishWriteRecordsToDbMessage = @"Finish adding rows to the database: ";

        public void StartApplication()
        {
            Console.WriteLine(WelcomeMessage);
            try
            {
                do
                {
                    ParseSite(_parser, _dBWorker);
                } while (Console.KeyAvailable == false && StartWaiting(WaitingInterval) == WaitingInterval);
            }
            catch (Exception e)
            {
                PrintExceptionMessage(e);
            }
        }


        private void ParseSite(Parser parser, DbWorker dbWorker)
        {
            var currentPage = 0;

            while (parser.TryRequestData(currentPage, PageSize, out Deal[] dealsDataList) && Console.KeyAvailable == false)
            {
                Console.WriteLine(CurentPageInformationTimeMessage, currentPage + 1, currentPage * PageSize, (currentPage + 1) * PageSize);
                Console.WriteLine(StartWriteRecordsToDbMessage + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

                dbWorker.AddDealRange(dealsDataList);

                Console.WriteLine(FinishWriteRecordsToDbMessage + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + Environment.NewLine);

                currentPage++;
            }
        }


        private int StartWaiting(int secondsToWait)
        {
            Console.WriteLine(WaitExecutionMessage, secondsToWait / SecondsInMinute, secondsToWait % SecondsInMinute);
            var consoleCursorTopPosition = Console.CursorTop;
            var counterOfSeconds = 0;

            while (Console.KeyAvailable == false && counterOfSeconds < secondsToWait)
            {
                Console.SetCursorPosition(2, consoleCursorTopPosition);
                Console.Write(ElapsedTimeMessage, ++counterOfSeconds);
                Thread.Sleep(MilliseconsInSecond);
            }
            return counterOfSeconds;
        }


        private void PrintExceptionMessage(Exception exception)
        {
            Console.WriteLine(WorkFailMessage + Environment.NewLine);
            Console.WriteLine(exception.Message + Environment.NewLine + exception.Source + Environment.NewLine);

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                Console.WriteLine(innerException.Message + Environment.NewLine + innerException.Source);
                Console.WriteLine();
                innerException = innerException.InnerException;
            }
        }
    }
}


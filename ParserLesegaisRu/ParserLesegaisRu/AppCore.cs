using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ParserLesegaisRu.DbWork;

namespace ParserLesegaisRu
{
    public class AppCore
    {
        private readonly DbWorker _dBWorker;
        private readonly Parser _parser;
        

        const string WelcomeMessage = @"This App parse site Lesegais.Ru";
        const string WorkFailMessage = @"Program execution was terminated!";
        const string UrlForParsing = @"https://www.lesegais.ru/open-area/deal";
        //const string UrlForParsing = @"c:\Users\Vladimir\Downloads\lesegaisru-page1.mhtml";

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

                while (_parser.GetNextPageData(out List<Declaration> dealsDataList))
                {
                    _dBWorker.AddDealRange(dealsDataList);
                }

                //var task = parser.LoadPageAsync(UrlForParsing);

                //Console.WriteLine("While site is beeing parsed let's draw asterisks");
                //var counter = 1;
                //var maxCounter = 20;
                //var strBilder = new StringBuilder(maxCounter * 2);

                //while (!task.IsCompleted)
                //{
                //    strBilder.Append(' ', maxCounter - counter);
                //    strBilder.Append('*', counter);
                //    Console.WriteLine(strBilder.ToString());
                //    counter = ((counter + 2) % 20);
                //    Thread.Sleep(100);
                //    strBilder.Clear();
                //}

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


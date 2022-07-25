using System;

namespace ParserLesegaisRu
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Начало работы приложения: " + DateTime.Now);
            new AppCore().StartApplication();
            Console.WriteLine("Окончание работы приложения: " + DateTime.Now);

        }
    }
}

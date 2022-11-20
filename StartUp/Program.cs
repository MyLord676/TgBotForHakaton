using StartUp.TelegramBot;

namespace TelegramBotExperiments
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("App start");
            var bot = new TgBot("5841458717:AAEeWaganrS_XYoxZNzOvJ1lZsdCyXiEOOk", 
                Directory.GetCurrentDirectory() + "/data.csv", 
                Directory.GetCurrentDirectory() + "/SendTo.csv",
                Directory.GetCurrentDirectory() + "/Analytics.csv");
            bot.InfinityPolling();
        }
    }
}
using StartUp.TelegramBot;

namespace TelegramBotExperiments
{
    class Program
    {
        static void Main()
        {
            var bot = new TgBot("5841458717:AAGygPGReGjD8T4EOq8BaMcRuYdQUx0kqqg");
            bot.InfinityPolling();
        }
    }
}
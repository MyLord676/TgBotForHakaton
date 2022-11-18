using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace StartUp
{
    public class TgBot
    {
        public ITelegramBotClient Bot;
        private string token;
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update == null) return;
            if (update.Type != UpdateType.Message) return;

            var message = update.Message;
            if(message != null)
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привет-привет!!");
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public void InfinityPolling()
        { 
            while (true)
            {
                var command = Console.ReadLine()?.ToLower();
                if (command == "stop")
                    return;
            }
        }

        public TgBot(string token)
        {
            this.token = token;
            Bot = new TelegramBotClient(this.token);

            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync
            );
        }
    }
}

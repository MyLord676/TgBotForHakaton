using CsvHelper;
using StartUp.Data;
using System.Diagnostics;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace StartUp.TelegramBot
{
    public class TgBot
    {
        public ITelegramBotClient Bot;
        private string token;
        private string configPath;
        private List<string[]> config;
        private List<int[]> sendTo;
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update == null) return;

            var message = update.Message;
            if (message == null) return;

            using (var db = new TgBotContext())
            {
                var docs = db.Documents.Where(doc => doc.TelegramID == message.Chat.Id).ToList();
                var sessions = db.Sessions.Where(ses => ses.TelegramID == message.Chat.Id).ToList();
                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "/start - создать новый документ\n");

                    Console.WriteLine("Новый doc");

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Документ создан");

                    if (docs.Count == 0)
                    {
                        var docum = new Domain.Document();
                        docum.Template = configPath;
                        docum.TelegramID = (int)message.Chat.Id;
                        db.Documents.Add(docum);
                        db.SaveChanges();
                        docs = db.Documents.Where(doc => doc.TelegramID == message.Chat.Id).ToList();
                    }
                    else
                    {
                        docs.Last().DateOfCreation = DateTime.Now;
                        if (docs.Last().QuestionAnswer != null)
                            docs.Last().QuestionAnswer.Clear();

                        docs.Last().Template = configPath;
                        db.SaveChanges();
                    }

                    if (sessions.Count == 0)
                    {
                        db.Sessions.Add(new Domain.Session() { IsActive = true, TelegramID = (int)message.Chat.Id });
                        db.SaveChanges();
                        sessions = db.Sessions.Where(ses => ses.TelegramID == message.Chat.Id).ToList();
                    }
                    else
                    {
                        if (!sessions.Last().IsActive)
                        { 
                            sessions.Last().IsActive = true;
                            db.SaveChanges();
                        }
                        sessions.Last().LastQuestion = null;
                    }
                }

                if (!sessions.Last().IsActive)
                    return;
                

                Domain.Document doc = docs.Last();

                var answeredQuestions = doc.QuestionAnswer;
                if (answeredQuestions == null)
                    answeredQuestions = new List<Domain.QuestionAndAnswer>();
                if (sessions.Last().LastQuestion != null)
                {
                    if (message.Photo != null)
                    {
                        var fileId = message.Photo.Last().FileId;
                        var fileInfo = await botClient.GetFileAsync(fileId);
                        var filePath = fileInfo.FilePath;

                        string destinationFilePath = Directory.GetCurrentDirectory() + "/Photo/downloaded"+ DateTime.Now.Ticks + ".png";
                        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                        await botClient.DownloadFileAsync(
                            filePath: filePath,
                            destination: fileStream);
                        fileStream.Close();

                        answeredQuestions.Add(new Domain.QuestionAndAnswer() { Question = sessions.Last().LastQuestion, Answer = "%Photo%" + destinationFilePath, DocumentID = doc.DocumentID });
                    }
                    else
                        answeredQuestions.Add(new Domain.QuestionAndAnswer() { Question = sessions.Last().LastQuestion, Answer = message.Text, DocumentID = doc.DocumentID });
                    db.SaveChanges();
                }


                List<string> a = new List<string>();
                if (answeredQuestions != null)
                    foreach (var answeredQuestion in answeredQuestions)
                        a.Add(answeredQuestion.Question);

                foreach (var question in config)
                {
                    if (!a.Contains(question[2]))
                    {
                        sessions.Last().LastQuestion = question[2];
                        db.SaveChanges();
                        await botClient.SendTextMessageAsync(message.Chat.Id, question[2]);
                        return;
                    }
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Больше нет вопросов");
                sessions.Last().IsActive = false;
                sessions.Last().LastQuestion = null;
                db.SaveChanges();

                //var docId = db.Documents.Where(q => q.TelegramID == message.Chat.Id).First().DocumentID;
                //var ques = db.QuestionsAndAnswers.Where(q => q.DocumentID == docId).ToList();

                //var questionName = new Dictionary<string, string>();
                //foreach (var item in ques)
                //{
                //    foreach (var t in config)
                //    {
                //        if (t.Contains(item.Question))
                //        {
                //            questionName.Add(t[0], item.Answer);
                //            break;
                //        }
                //    }
                //}
                //Console.WriteLine(questionName);

                //using (var stream = File.OpenRead("D:\\test.docx"))
                //{
                //    Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream);
                //    iof.FileName = "smth.docx";
                //    var send = await botClient.SendDocumentAsync(message.Chat.Id, iof, "Ваш документ");
                //}

                return;
            }
            
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

        private void ConvertConfig()
        {
            using (CsvReader csv = new CsvReader(new StreamReader(configPath), CultureInfo.InvariantCulture))
            {
                int fieldCount = csv.ColumnCount;
                config = new List<string[]>();
                string field;
                while (csv.Read())
                {
                    string[] row = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        csv.TryGetField(i, out field);
                        row[i] = field;
                        Console.WriteLine(field);
                    }
                    config.Add(row);
                }
            }
        }
        private void ConvertSendTo(string sendToPath)
        {
            using (CsvReader csv = new CsvReader(new StreamReader(sendToPath), CultureInfo.InvariantCulture))
            {
                int fieldCount = csv.ColumnCount;
                sendTo = new List<int[]>();
                int field;
                while (csv.Read())
                {
                    int[] row = new int[3];
                    for (int i = 0; i < 2; i++)
                    {
                        csv.TryGetField(i, out field);
                        row[i] = field;
                        Console.WriteLine(field);
                    }
                    row[2] = 0;
                    sendTo.Add(row);
                }
            }
        }

        public TgBot(string token, string configPath, string sendToPath)
        {
            this.token = token;
            Bot = new TelegramBotClient(this.token);

            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync
            );
            this.configPath = configPath;
            ConvertSendTo(sendToPath);
            ConvertConfig();
            Console.WriteLine("tg bot");
        }
    }
}
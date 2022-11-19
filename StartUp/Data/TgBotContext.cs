using Newtonsoft.Json.Linq;
using StartUp.Domain;
using System.Data.Entity;
using System.Reflection.Emit;

namespace StartUp.Data
{
    public class TgBotContext: DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<QuestionAndAnswer>? QuestionsAndAnswers { get; set; }
        public TgBotContext(): base("DBConnection")
        {

        }
    }
}

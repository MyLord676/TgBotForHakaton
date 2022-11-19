using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUp.Domain
{
    [Table("Document")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentID { get; set; }
        public int? TelegramID { get; set; }
        public virtual List<QuestionAndAnswer>? QuestionAnswer { get; set; }
        public string? Template { get; set; } = string.Empty;
        public DateTime? DateOfCreation { get; set; } = DateTime.Now;

    }
}

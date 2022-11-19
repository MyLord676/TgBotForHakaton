using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUp.Domain
{
    [Table("QuestionAndAnswer")]
    public class QuestionAndAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionAndAnswerID { get; set; }
        public string? Question { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
        public int? DocumentID { get; set; }
        [ForeignKey(nameof(DocumentID))]
        public Document? Document { get; set; }
    }
}

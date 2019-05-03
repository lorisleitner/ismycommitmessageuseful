using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ismycommitmessageuseful.Database
{
    [Table("commit")]
    public class Commit
    {
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("useful_count")]
        public int UsefulCount { get; set; }

        [Column("not_useful_count")]
        public int NotUsefulCount { get; set; }

        [Column("dont_know_count")]
        public int DontKnowCount { get; set; }
    }
}

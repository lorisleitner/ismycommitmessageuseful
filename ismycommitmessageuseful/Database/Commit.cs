using System.ComponentModel.DataAnnotations;

namespace ismycommitmessageuseful.Database
{
    public class Commit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        public int UsefulCount { get; set; }

        public int NotUsefulCount { get; set; }

        public int DontKnowCount { get; set; }
    }
}

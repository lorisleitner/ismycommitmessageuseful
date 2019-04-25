using System.ComponentModel.DataAnnotations;

namespace ismycommitmessageuseful.Models
{
    public class CommitInputDto
    {
        [Required]
        public string Message { get; set; }
    }
}

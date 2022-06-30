using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Data.Models
{
    public class TalkModel
    {
        [Required]
        public int TalkId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(400,MinimumLength =10)]
        public string Abstract { get; set; }
        [Range(100,300)]
        public int Level { get; set; }
        public SpeakerModel Speaker { get; set; }

    }
}

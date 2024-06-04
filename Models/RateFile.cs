using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class RateFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string? FilePath { get; set; }

        public string? FileType { get; set; } // "image" hoặc "video"

        public long RateId { get; set; }
        [JsonIgnore]
        public virtual Rate Rate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core_Perf.Entities
{
    [Table(nameof(RestingAreaFeature))]
    public class RestingAreaFeature
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

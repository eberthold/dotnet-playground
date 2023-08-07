using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core_Perf.Entities
{
    [Table("RestingAreas")]
    public class RestingAreaEntity
    {
        [Key]
        public int Id { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public bool IsBlocked { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        public bool Future { get; set; }

        [ForeignKey(nameof(Road))]
        public int RoadId { get; set; }

        public RoadEntity Road { get; set; } = null!;

        public List<RestingAreaFeature> Features { get; set; } = new List<RestingAreaFeature>();
    }
}

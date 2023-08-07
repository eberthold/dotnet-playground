using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core_Perf.Entities
{
    [Table("Roads")]
    public class RoadEntity
    { 
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        [InverseProperty(nameof(RestingAreaEntity.Road))]
        public List<RestingAreaEntity> RestingAreas { get; set; } = new List<RestingAreaEntity>();
    }
}

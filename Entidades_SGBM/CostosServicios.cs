using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades_SGBM
{
    public class CostosServicios
    {
        [Key]
        public int? IdCostoServicio { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Costo { get; set; }

        public int? Unidades { get; set; }

        [Column(TypeName = "decimal(12,2)"), DisplayName("Medida")]
        public decimal? CantidadMedida { get; set; }

        [ForeignKey("Productos")]
        public int? IdProducto { get; set; }

        [ForeignKey("Servicios")]
        public int IdServicio { get; set; }

        public string? Descripcion { get; set; }

        public Productos? Productos { get; set; }
        public Servicios? Servicios { get; set; }

        public override string ToString()
        {
            return Descripcion ?? "";
        }

    }
}

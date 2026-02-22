using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades_SGBM
{
    public class Servicios
    {
        [Key]
        public int? IdServicio { get; set; }

        [MaxLength(150)]
        public string NombreServicio { get; set; }

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Costos {  get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Margen { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Comision { get; set; }

        public int DuracionMinutos { get; set; }

        public int Puntaje { get; set; }

        [ForeignKey("Categorias")]
        public int IdCategoria { get; set; }

        [Column(TypeName = "bit")]
        public bool Activo {  get; set; }

        public Categorias? Categorias { get; set; }

        public override string ToString()
        {
            return NombreServicio;
        }
    }
}

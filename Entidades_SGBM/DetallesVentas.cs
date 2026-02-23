using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class DetallesVentas
    {
        [Key]
        public int? IdDetalleVenta { get; set; }

        public int Cantidad { get; set; }

        [MaxLength(200)]
        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? InteresDescuento { get; set; }

        [ForeignKey("Productos")]
        public int? IdProducto { get; set; }

        [ForeignKey("Servicios")]
        public int? IdServicio { get; set; }

        [ForeignKey("FondosMembresias")]
        public int? IdFondoMembresia { get; set; }

        [ForeignKey("Ventas")]
        public int IdVenta { get; set; }

        public Productos? Productos { get; set; }
        public Servicios? Servicios { get; set; }
        public Ventas? Ventas { get; set; }

    }
}

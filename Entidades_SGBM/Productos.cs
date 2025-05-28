using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Productos
    {
        [Key]
        public int? IdProducto { get; set; }

        [MaxLength(100)]
        public string CodProducto { get; set; }

        [MaxLength(100)]
        public string Descripcion { get; set; }

        public int CantidadVenta { get; set; }
        public int? CantidadMedida { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Costo { get; set; }

        [ForeignKey("UnidadesMedidas")]
        public int? IdUnidadMedida { get; set; }

        [ForeignKey("Proveedores")]
        public int? IdProveedor { get; set; }

        public UnidadesMedidas? UnidadesMedidas { get; set; }
        public Proveedores? Proveedores { get; set; }

    }
}

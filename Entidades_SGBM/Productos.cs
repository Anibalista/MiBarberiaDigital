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

        public int Stock { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? CantidadMedida { get; set; }
        public int? Medida { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Costo { get; set; }

        [ForeignKey("UnidadesMedidas")]
        public int? IdUnidadMedida { get; set; }

        [ForeignKey("Proveedores")]
        public int? IdProveedor { get; set; }

        [ForeignKey("Categorias")]
        public int? idCategoria { get; set; }

        [Column(TypeName ="bit")]
        public bool Activo { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal? Comision { get; set; }

        public UnidadesMedidas? UnidadesMedidas { get; set; }
        public Proveedores? Proveedores { get; set; }
        public Categorias? Categorias { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}

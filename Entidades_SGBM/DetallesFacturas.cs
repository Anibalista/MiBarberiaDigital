using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class DetallesFacturas
    {
        [Key]
        public int? IdDetalleFactura { get; set; }

        [ForeignKey("DetallesVentas")]
        public int IdDetalleVenta { get; set; }

        [ForeignKey("Facturas")]
        public int IdFactura { get; set; }

        public DetallesVentas? DetallesVentas { get; set; }
        public Facturas? Facturas { get; set; }

    }
}

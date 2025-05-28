using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Transacciones
    {
        [Key]
        public int? IdTransaccion { get; set; }

        public DateTime Hora { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? MontoIngreso { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? MontoEgreso { get; set; }

        [ForeignKey("TiposTransacciones")]
        public int IdTipoTransaccion { get; set; }

        [ForeignKey("Cajas")]
        public int IdCaja { get; set; }

        public TiposTransacciones? TiposTransacciones { get; set; }
        public Cajas? Cajas { get; set; }

    }
}

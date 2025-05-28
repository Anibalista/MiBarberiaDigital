using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class CuotasMembresias
    {
        [Key]
        public int? IdCuota { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime? FechaPago { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal Recargo { get; set; }

        [ForeignKey("Transacciones")]
        public int IdTransaccion { get; set; }

        [ForeignKey("FondosMembresias")]
        public int IdFondoMembresia { get; set; }

        public Transacciones? Transacciones { get; set; }
        public FondosMembresias? FondosMembresias { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class FondosMembresias
    {
        [Key]
        public int? IdFondoMembresia { get; set; }

        public DateTime? PeriodoDesde { get; set; }
        public DateTime PeriodoHasta { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoAcumulado { get; set; }

        public int PuntajeAcumulado { get; set; }
    }
}

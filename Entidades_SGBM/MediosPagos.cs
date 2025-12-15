using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class MediosPagos
    {
        [Key]
        public int? IdMedioPago { get; set; }

        [MaxLength(150)]
        public string Medio { get; set; }

        [MaxLength(200)]
        public string? Observaciones { get; set; }

        public override string ToString()
        {
            return Medio;
        }
    }
}

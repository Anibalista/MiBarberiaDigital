using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Niveles
    {
        [Key]
        public int? IdNivel { get; set; }

        [MaxLength(100)]
        public string Nivel { get; set; }

        public override string ToString()
        {
            return Nivel;
        }
    }
}

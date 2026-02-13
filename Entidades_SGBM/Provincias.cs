using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Provincias
    {
        [Key]
        public int IdProvincia { get; set; }

        [MaxLength(150)]
        public string Provincia { get; set; }

        public override string ToString()
        {
            return Provincia;
        }
    }
}

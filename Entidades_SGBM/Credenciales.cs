using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Credenciales
    {
        [Key]
        public int? IdCredencial { get; set; }

        [MaxLength(150)]
        public string Accesos { get; set; }

        [ForeignKey("Niveles")]
        public int IdNivel { get; set; }

        public Niveles? Niveles { get; set; }
    }
}

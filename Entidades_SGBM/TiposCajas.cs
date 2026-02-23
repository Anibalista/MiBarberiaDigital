using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class TiposCajas
    {
        [Key]
        public int IdTipo {  get; set; }

        public string Tipo { get; set; }

    }
}

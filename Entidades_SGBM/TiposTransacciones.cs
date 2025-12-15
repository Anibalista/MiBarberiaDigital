using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class TiposTransacciones
    {
        [Key]
        public int? IdTipoTransaccion { get; set; }

        [MaxLength(100)]
        public string Tipo { get; set; }

        public override string ToString()
        {
            return Tipo;
        }
    }
}

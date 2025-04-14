using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class UnidadesMedidas
    {
        [Key]
        public int IdUnidadMedida { get; set; }

        [MaxLength(50)]
        public string Unidad { get; set; }
    }
}

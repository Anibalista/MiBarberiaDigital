using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
        public class Categorias
        {
            [Key]
            public int? IdCategoria { get; set; }

            [MaxLength(150)]
            public string Descripcion { get; set; }

            [MaxLength(50)]
            public string Indole { get; set; }

            public override string ToString()
            {
                return Descripcion ?? "";
            }
        }
}

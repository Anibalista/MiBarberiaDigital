using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class TiposMembresias
    {
        [Key]
        public int? IdTipo { get; set; }

        [MaxLength(50)]
        public string NombreTipo { get; set; }

        [MaxLength(150)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Precio { get; set; }

        public override string ToString()
        {
            return NombreTipo;
        }
    }
}

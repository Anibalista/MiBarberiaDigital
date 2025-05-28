using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Personas
    {
        [Key]
        public int? IdPersona { get; set; }

        [MaxLength(150)]
        public string Dni { get; set; }

        [MaxLength(150)]
        public string Nombres { get; set; }

        [MaxLength(150)]
        public string Apellidos { get; set; }

        public DateTime? FechaNac { get; set; }

        [ForeignKey("Domicilios")]
        public int? IdDomicilio { get; set; }

        public Domicilios? Domicilios { get; set; }
    }
}

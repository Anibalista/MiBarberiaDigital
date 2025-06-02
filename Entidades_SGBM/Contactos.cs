using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Contactos
    {
        [Key]
        public int? IdContacto { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(20)]
        public string? Whatsapp { get; set; }

        [MaxLength(150)]
        public string? Instagram { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(150)]
        public string? Facebook { get; set; }

        [Column(TypeName = "bit")]
        public bool ExtranjeroWhatsapp { get; set; }

        [ForeignKey("Personas")]
        public int? IdPersona { get; set; }

        [ForeignKey("Proveedores")]
        public int? IdProveedor { get; set; }

        public Personas? Personas { get; set; }

        public Proveedores? Proveedores { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Domicilios
    {
        [Key]
        public int? IdDomicilio { get; set; }

        [MaxLength(150)]
        public string? Calle { get; set; }

        [MaxLength(20)]
        public string? Altura { get; set; }

        [MaxLength(10)]
        public string? Piso { get; set; }

        [MaxLength(10)]
        public string? Depto { get; set; }

        [MaxLength(220)]
        public string? Barrio { get; set; }

        [ForeignKey("Localidades")]
        public int IdLocalidad { get; set; }

        [ForeignKey("Proveedores")]
        public int? IdProveedor { get; set; }

        public Localidades? Localidades { get; set; }
        public Proveedores? Proveedores { get; set; }
    }
}

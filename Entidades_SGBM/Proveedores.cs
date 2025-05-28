using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Proveedores
    {
        [Key]
        public int? IdProveedor { get; set; }

        [MaxLength(30)]
        public string? Cuit { get; set; }

        [MaxLength(150)]
        public string RazonSocial { get; set; }
    }
}

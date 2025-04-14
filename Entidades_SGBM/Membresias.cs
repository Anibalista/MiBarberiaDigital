using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Membresias
    {
        [Key]
        public int IdMembresia { get; set; }

        public DateTime FechaAlta { get; set; }

        [ForeignKey("TiposMembresias")]
        public int IdTipo { get; set; }

        [ForeignKey("Clientes")]
        public int IdCliente { get; set; }

        [ForeignKey("Empleados")]
        public int IdEmpleado { get; set; }

        [ForeignKey("Estados")]
        public int IdEstado { get; set; }

        public TiposMembresias? TiposMembresias { get; set; }
        public Clientes? Clientes { get; set; }
        public Empleados? Empleados { get; set; }
        public Estados? Estados { get; set; }

    }
}

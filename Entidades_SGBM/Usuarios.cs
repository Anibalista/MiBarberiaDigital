using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Usuarios
    {
        [Key]
        public int? IdUsuario { get; set; }

        [MaxLength(150)]
        public string NombreUsuario { get; set; }

        [MaxLength(150)]
        public string Clave { get; set; }

        [MaxLength(230)]
        public string? Imagen { get; set; }


        [Column(TypeName = "bit")]
        public bool Activo { get; set; }

        [ForeignKey("Niveles")]
        public int IdNivel { get; set; }

        [ForeignKey("Empleados")]
        public int IdEmpleado { get; set; }

        public Niveles? Niveles { get; set; }
        public Empleados? Empleados { get; set; }

        public override string ToString()
        {
            return NombreUsuario;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Clientes
    {
        [Key]
        public int? IdCliente { get; set; }

        [ForeignKey("Personas")]
        public int IdPersona { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }

        public Personas? Personas { get; set; }

        [NotMapped]
        public string? NombreCompleto
        {
            get
            {
                if (Personas != null)
                {
                    return $"{Personas.Apellidos} {Personas.Nombres}";
                }
                return null;
            }
        }
    }
}

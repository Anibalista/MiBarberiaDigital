using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Cajas
    {
        [Key]
        public int? IdCaja { get; set; }

        [Column(TypeName = "bit")]
        public bool Abierta { get; set; }

        public DateTime Fecha  { get; set; }

        public DateTime? HoraCierre { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalEfectivo { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalMP { get; set; }

        [ForeignKey("TiposCajas")]
        public int IdTipo { get; set; }

        [ForeignKey("Empleados")]
        public int? IdEmpleado { get; set; }

        public TiposCajas? TiposCajas { get; set;}

        public Empleados? Empleados { get; set; }

        public ICollection<Transacciones>? Transacciones { get; set; }

        [NotMapped]
        public string? Tipo
        {
            get
            {
                if (TiposCajas != null)
                    return TiposCajas.Tipo;
                return null;
            }
        }
    }
}

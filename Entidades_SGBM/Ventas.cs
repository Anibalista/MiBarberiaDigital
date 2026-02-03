using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Ventas
    {
        [Key]
        public int? IdVenta { get; set; }

        [MaxLength(30)]
        public string NroVenta { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public DateTime FechaVenta { get; set; }

        [ForeignKey("Clientes")]
        public int IdCliente { get; set; }

        [ForeignKey("Empleados")]
        public int IdEmpleado { get; set; }

        [ForeignKey("Estados")]
        public int IdEstado { get; set; }

        public Clientes? Clientes { get; set; }
        public Empleados? Empleados { get; set; }
        public Estados? Estados { get; set; }


        public override string ToString()
        {
            return $"{NroVenta}_{FechaVenta.ToString("dd/MM/yy")}";
        }

    }
}

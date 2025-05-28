using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Facturas
    {
        [Key]
        public int? IdFactura { get; set; }

        [MaxLength(10)]
        public string Tipo { get; set; }

        [MaxLength(100)]
        public string NroFactura { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAbonado { get; set; }

        [ForeignKey("Transacciones")]
        public int? IdTransaccion { get; set; }

        [ForeignKey("MediosPagos")]
        public int IdMedioPago { get; set; }

        [ForeignKey("Ventas")]
        public int IdVenta { get; set; }

        public Transacciones? Transacciones { get; set; }
        public MediosPagos? MediosPagos { get; set; }
        public Ventas? Ventas { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class ServiciosInsumos
    {
        [Key]
        public int IdServicioInsumo { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Costo { get; set; }

        public int? Unidades { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? CantidadMedida { get; set; }

        [ForeignKey("Insumos")]
        public int IdInsumo { get; set; }

        [ForeignKey("Servicios")]
        public int IdServicio { get; set; }

        public Insumos? Insumos { get; set; }
        public Servicios? Servicios { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Insumos
    {
        [Key]
        public int? IdInsumo { get; set; }

        public int? CantidadUnidades { get; set; }


        [Column(TypeName = "decimal(12,2)")]
        public decimal? CantidadMedida { get; set; }

        [ForeignKey("Productos")]
        public int IdProducto { get; set; }

        public Productos? Productos { get; set; }

    }
}

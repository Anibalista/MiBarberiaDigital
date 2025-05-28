using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class MembresiasServicios
    {
        [Key]
        public int? Id { get; set; }

        [ForeignKey("TiposMembresias")]
        public int IdTipoMembresia { get; set; }

        [ForeignKey("Servicios")]
        public int IdServicio { get; set; }
    }
}

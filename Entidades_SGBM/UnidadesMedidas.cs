using System.ComponentModel.DataAnnotations;

namespace Entidades_SGBM
{
    public class UnidadesMedidas
    {
        [Key]
        public int IdUnidadMedida { get; set; }

        [MaxLength(10)]
        public string Unidad { get; set; }

        [MaxLength(50)]
        public string? Descripcion { get; set; }

        public override string ToString()
        {
            return Unidad + (Descripcion == null ? "" : $" - {Descripcion}");
        }
    }
}

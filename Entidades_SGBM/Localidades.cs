using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Localidades
    {
        [Key]
        public int IdLocalidad { get; set; }

        [MaxLength(150)]
        public string Localidad { get; set; }

        [MaxLength(20)]
        public string? CodPostal { get; set; }

        [ForeignKey("Provincias")]
        public int IdProvincia { get; set; }

        public Provincias? Provincias { get; set; }

        [NotMapped]
        public string? localidadCompleta
        {
            get
            {
                if (Provincias == null)
                    return Localidad;
                return $"{Localidad}, {Provincias.Provincia}";
            } set
            {
                _localidadCompleta = value;
            }
        }
        private string? _localidadCompleta;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades_SGBM
{
    public class Personas
    {
        [Key]
        public int? IdPersona { get; set; }

        [MaxLength(150)]
        public string Dni { get; set; }

        [MaxLength(150)]
        public string Nombres { get; set; }

        [MaxLength(150)]
        public string Apellidos { get; set; }

        public DateTime? FechaNac { get; set; }

        [ForeignKey("Domicilios")]
        public int? IdDomicilio { get; set; }

        public Domicilios? Domicilios { get; set; }

        [NotMapped]
        public string? Direccion
        {
            get
            {
                if (Domicilios != null)
                {
                    string direccion = string.IsNullOrWhiteSpace(Domicilios.Barrio) ? "" : $"Barrio: {Domicilios.Barrio}, ";
                    direccion += string.IsNullOrWhiteSpace(Domicilios.Calle) ? "" : $"Calle: {Domicilios.Calle}";
                    direccion += string.IsNullOrWhiteSpace(Domicilios.Altura) ? "" : $" {Domicilios.Altura}, ";
                    direccion += string.IsNullOrWhiteSpace(Domicilios.Piso) ? "" : $"Piso: {Domicilios.Piso}, ";
                    direccion += string.IsNullOrWhiteSpace(Domicilios.Depto) ? "" : $"Depto: {Domicilios.Depto}, ";
                    return direccion.TrimEnd(',', ' ');
                }
                return null;
            }
            set { _direccion = value; }
        }
        private string? _direccion;

        [NotMapped]
        public string? Localidad
        {
            get
            {
                if (Domicilios != null && Domicilios.Localidades != null)
                {
                    return Domicilios.Localidades.localidadCompleta;
                }
                return null;
            }
            set { _localidad = value; }
        }
        private string? _localidad;
    }
}

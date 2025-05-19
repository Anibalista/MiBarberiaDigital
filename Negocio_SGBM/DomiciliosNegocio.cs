using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class DomiciliosNegocio
    {
        //Provincias------------------------------
        //Consultas
        public static List<Provincias>? getProvincias()
        {
            List<Provincias>? provincias = DomiciliosDatos.getProvincias();
            return provincias;
        }

        //Localidades-----------------------------
        //Consultas
        public static List<Localidades>? getLocalidadesPorProvincia(Provincias? provincia)
        {
            List<Localidades>? localidades = DomiciliosDatos.getLocalidadesPorProvincia(provincia);
            return localidades;
        }

        //Domicilios------------------------------
        //Consultas
    }
}

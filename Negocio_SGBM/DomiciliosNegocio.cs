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

        //Registros
        public static int registrarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return -1;
            }
            if (domicilio.IdDomicilio > 0)
            {
                mensaje = "No es posible registrar un domicilio existente";
                return -1;
            }
            int idDomicilio = DomiciliosDatos.registrarDomicilio(domicilio, ref mensaje);
            return idDomicilio;
        }

        //Modificaciones
        public static bool modificarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return false;
            }
            if (domicilio.IdDomicilio < 1)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }
            int exito = DomiciliosDatos.modificarDomicilio(domicilio, ref mensaje);
            return exito > 0;
        }

        //Eliminaciones
        public static bool eliminarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return false;
            }
            if (domicilio.IdDomicilio == null)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }
            bool exito = DomiciliosDatos.eliminarDomicilioPorId((int)domicilio.IdDomicilio, ref mensaje);
            return exito;
        }

        public static bool eliminarDomicilio(int idDomicilio, ref string mensaje)
        {
            if (idDomicilio < 1)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }
            bool exito = DomiciliosDatos.eliminarDomicilioPorId(idDomicilio, ref mensaje);
            return exito;
        }
    }
}

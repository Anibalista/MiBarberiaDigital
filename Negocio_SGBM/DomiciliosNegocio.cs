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
        //Comprobaciones
        public static Localidades? getLocalidadGenerica(Localidades? localidad, ref string mensaje)
        {
            if (localidad != null)
            {
                if (localidad.IdLocalidad == null || localidad.IdProvincia < 1)
                {
                    mensaje = String.IsNullOrWhiteSpace(mensaje) ? "No Se podrá registrar el domicilio ingresado (problemas con localidad)" : "";
                }
                return localidad;
            }
            localidad ??= getLocalidadPorNombre("Gualeguaychú", ref mensaje) ?? new Localidades { Localidad = "Gualeguaychú" };
            if (localidad.IdProvincia > 0)
            {
                localidad.Provincias = null;
                return localidad;
            }
            localidad.Provincias = getProvincias()?.FirstOrDefault(p => p.Provincia == "Entre Ríos") ?? new Provincias { Provincia = "Entre Ríos" };
            
            if (localidad.Provincias.IdProvincia != null)
            {
                localidad.IdProvincia = (int)localidad.Provincias.IdProvincia;
                localidad.Provincias = null;
            }
            int id = registrarLocalidad(localidad, ref mensaje);
            if (id > 0)
            {
                localidad.IdLocalidad = id;
            }
            return localidad;

        }

        //Consultas
        public static List<Localidades>? getLocalidadesPorProvincia(Provincias? provincia)
        {
            List<Localidades>? localidades = DomiciliosDatos.getLocalidadesPorProvincia(provincia);
            return localidades;
        }

        public static List<Localidades>? getLocalidades(ref string mensaje)
        {
            List<Localidades>? localidades = DomiciliosDatos.getLocalidades(ref mensaje);
            return localidades;
        }

        public static Localidades? getLocalidadPorNombre(string? nombre, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "No viaja el nombre de la localidad a consultar (capa negocio)";
                return null;
            }
            Localidades? localidad = DomiciliosDatos.getLocalidadPorNombre(nombre, ref mensaje);
            return localidad;
        }

        //Registro
        public static int registrarLocalidad(Localidades? localidad, ref string mensaje)
        {
            if (localidad == null)
            {
                mensaje = "No llega información de la localidad a la capa negocio";
                return -1;
            }
            if (localidad.IdProvincia < 1 && localidad.Provincias == null)
            {
                mensaje = "No llega información de la provincia a la capa negocio";
                return -1;
            }
            int id = DomiciliosDatos.registrarLocalidad(localidad, ref mensaje);
            if (id == 0)
            {
                mensaje = "\nNo se pudo registrar la localidad para el domicilio";
            }
            return id;
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

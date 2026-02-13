using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de domicilios, localidades y provincias.
    /// </summary>
    public class DomiciliosNegocio
    {
        #region Provincias

        /// <summary>
        /// Obtiene todas las provincias disponibles.
        /// </summary>
        /// <returns>Lista de provincias o null si ocurre un error.</returns>
        public static List<Provincias>? GetProvincias()
        {
            List<Provincias>? provincias = DomiciliosDatos.GetProvincias();
            return provincias;
        }

        #endregion

        #region Localidades

        /// <summary>
        /// Obtiene una localidad genérica (por defecto Gualeguaychú).
        /// Si no existe, la registra junto con su provincia.
        /// </summary>
        /// <param name="localidad">Localidad a validar o registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Localidad válida o registrada.</returns>
        public static Localidades? GetLocalidadGenerica(Localidades? localidad, ref string mensaje)
        {
            if (localidad != null)
            {
                if (localidad.IdLocalidad < 1 || localidad.IdProvincia < 1)
                {
                    mensaje = String.IsNullOrWhiteSpace(mensaje) ?
                              "No se podrá registrar el domicilio ingresado (problemas con localidad)" : "";
                }
                return localidad;
            }

            // Si no llega localidad, se busca por nombre o se crea nueva
            localidad ??= GetLocalidadPorNombre("Gualeguaychú", ref mensaje)
                          ?? new Localidades { Localidad = "Gualeguaychú" };

            if (localidad.IdProvincia > 0)
            {
                localidad.Provincias = null;
                return localidad;
            }

            localidad.Provincias = GetProvincias()?.FirstOrDefault(p => p.Provincia == "Entre Ríos")
                                   ?? new Provincias { Provincia = "Entre Ríos" };

            if (localidad.Provincias.IdProvincia > 0)
            {
                localidad.IdProvincia = localidad.Provincias.IdProvincia;
                localidad.Provincias = null;
            }

            int id = RegistrarLocalidad(localidad, ref mensaje);
            if (id > 0)
            {
                localidad.IdLocalidad = id;
            }
            return localidad;
        }

        /// <summary>
        /// Obtiene las localidades de una provincia.
        /// </summary>
        public static List<Localidades>? GetLocalidadesPorProvincia(Provincias? provincia)
        {
            List<Localidades>? localidades = DomiciliosDatos.GetLocalidadesPorProvincia(provincia);
            return localidades;
        }

        /// <summary>
        /// Obtiene todas las localidades.
        /// </summary>
        public static List<Localidades>? GetLocalidades(ref string mensaje)
        {
            List<Localidades>? localidades = DomiciliosDatos.GetLocalidades(ref mensaje);
            return localidades;
        }

        /// <summary>
        /// Obtiene una localidad por nombre.
        /// </summary>
        public static Localidades? GetLocalidadPorNombre(string? nombre, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "No viaja el nombre de la localidad a consultar (capa negocio)";
                return null;
            }
            Localidades? localidad = DomiciliosDatos.GetLocalidadPorNombre(nombre, ref mensaje);
            return localidad;
        }

        /// <summary>
        /// Registra una nueva localidad.
        /// </summary>
        public static int RegistrarLocalidad(Localidades? localidad, ref string mensaje)
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
            int id = DomiciliosDatos.RegistrarLocalidad(localidad, ref mensaje);
            if (id < 1)
            {
                mensaje = "\nNo se pudo registrar la localidad para el domicilio";
            }
            return id;
        }

        #endregion

        #region Domicilios

        /// <summary>
        /// Registra un nuevo domicilio.
        /// </summary>
        public static int RegistrarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return -1;
            }
            if (domicilio?.IdDomicilio > 0)
            {
                mensaje = "No es posible registrar un domicilio existente";
                return -1;
            }
            int idDomicilio = DomiciliosDatos.RegistrarDomicilio(domicilio, ref mensaje);
            return idDomicilio;
        }

        /// <summary>
        /// Modifica un domicilio existente.
        /// </summary>
        public static bool ModificarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return false;
            }
            if (domicilio?.IdDomicilio < 1)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }
            return DomiciliosDatos.ModificarDomicilio(domicilio, ref mensaje);
        }

        /// <summary>
        /// Elimina un domicilio por entidad.
        /// </summary>
        public static bool EliminarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa de negocio";
                return false;
            }

            if (domicilio?.IdDomicilio == null)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }

            bool exito = DomiciliosDatos.EliminarDomicilioPorId((int)domicilio.IdDomicilio, ref mensaje);
            return exito;
        }

        /// <summary>
        /// Elimina un domicilio por Id.
        /// </summary>
        public static bool EliminarDomicilio(int idDomicilio, ref string mensaje)
        {
            if (idDomicilio < 1)
            {
                mensaje = "El id del domicilio no llega a la capa de negocio";
                return false;
            }
            bool exito = DomiciliosDatos.EliminarDomicilioPorId(idDomicilio, ref mensaje);
            return exito;
        }

        #endregion
    }
}

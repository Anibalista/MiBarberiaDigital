using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    public class DomiciliosDatos
    {

        /// <summary>
        /// Comprueba que las entidades principales del contexto estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si todas las entidades están disponibles, False en caso contrario.</returns>
        public static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobacion = new ComprobacionContexto(contexto);

            // Verifica que la entidad Domicilios esté accesible
            if (!comprobacion.ComprobarEntidad(contexto.Domicilios, ref mensaje))
                return false;

            // Verifica que la entidad Localidades esté accesible
            if (!comprobacion.ComprobarEntidad(contexto.Localidades, ref mensaje))
                return false;

            // Verifica que la entidad Provincias esté accesible
            if (!comprobacion.ComprobarEntidad(contexto.Provincias, ref mensaje))
                return false;

            return true;
        }

        #region Listados

        /// <summary>
        /// Obtiene todas las provincias desde la base de datos.
        /// </summary>
        /// <returns>Lista de provincias o null si ocurre un error.</returns>
        public static List<Provincias>? GetProvincias()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    string mensaje = "";
                    if (!ComprobarContexto(contexto, ref mensaje))
                    {
                        Logger.LogError(mensaje);
                        return null;
                    }
                    return contexto.Provincias.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener Provincias:\n{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene todas las localidades ordenadas por nombre.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de localidades o null si ocurre un error.</returns>
        public static List<Localidades>? GetLocalidades(ref string mensaje)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Localidades.OrderBy(l => l.Localidad).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener Localidades:\n{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene las localidades pertenecientes a una provincia específica.
        /// </summary>
        /// <param name="provincia">Provincia seleccionada.</param>
        /// <returns>Lista de localidades o null si ocurre un error.</returns>
        public static List<Localidades>? GetLocalidadesPorProvincia(Provincias? provincia)
        {
            if (provincia == null || provincia?.IdProvincia == 0)
                return null;

            try
            {
                using (var contexto = new Contexto())
                {
                    string mensaje = "";
                    if (!ComprobarContexto(contexto, ref mensaje))
                    {
                        Logger.LogError(mensaje);
                        return null;
                    }
                    return contexto.Localidades
                                   .Where(l => l.IdProvincia == provincia.IdProvincia)
                                   .OrderBy(l => l.Localidad)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener Localidades de la provincia {provincia?.Provincia ?? "Sin Nombre"}:\n{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene domicilios filtrados por calle, barrio o localidad.
        /// </summary>
        /// <param name="calle">Nombre de la calle.</param>
        /// <param name="barrio">Nombre del barrio.</param>
        /// <param name="localidad">Localidad asociada.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de domicilios o null si ocurre un error.</returns>
        public static List<Domicilios>? GetDomiciliosPorCampos(string? calle, string? barrio, Localidades? localidad, ref string mensaje)
        {
            // Validación inicial: al menos un criterio debe estar presente
            if (string.IsNullOrEmpty(calle) && string.IsNullOrEmpty(barrio) && localidad == null)
            {
                mensaje = "No llegan los datos de búsqueda de domicilios a la capa datos";
                return null;
            }

            int idLocalidad = 0;
            if (localidad != null && localidad?.IdLocalidad > 0)
            {
                idLocalidad = localidad.IdLocalidad;
            }

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    return contexto.Domicilios
                                   .Include(d => d.Localidades)
                                   .ThenInclude(l => l.Provincias)
                                   .Where(d => (!string.IsNullOrEmpty(d.Calle) && d.Calle.Contains(calle ?? "")) ||
                                               (!string.IsNullOrEmpty(barrio) && d.Barrio.Contains(barrio ?? "")) ||
                                               (d.IdLocalidad == idLocalidad))
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(GetDomiciliosPorCampos)";
                return null;
            }
        }

        #endregion

        #region Consultas

        /// <summary>
        /// Obtiene el siguiente IdProvincia disponible.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>El siguiente IdProvincia o -1 si ocurre un error.</returns>
        static int GetSiguienteIdProvincia(Contexto contexto)
        {
            try
            {
                int idMaximo = contexto.Provincias.DefaultIfEmpty().Max(p => p == null ? 0 : p.IdProvincia);
                return idMaximo == 0 ? -1 : idMaximo + 1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener IDs de Provincias:\n{ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Obtiene el siguiente IdLocalidad disponible.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>El siguiente IdLocalidad o -1 si ocurre un error.</returns>
        static int GetSiguienteIdLocalidad(Contexto contexto)
        {
            try
            {
                int idMaximo = contexto.Localidades.DefaultIfEmpty().Max(p => p == null ? 0 : p.IdLocalidad);
                return idMaximo == 0 ? -1 : idMaximo + 1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener IDs de Localidades:\n{ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Obtiene una localidad por su nombre.
        /// </summary>
        /// <param name="nombre">Nombre de la localidad.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Localidad encontrada o null si no existe.</returns>
        public static Localidades? GetLocalidadPorNombre(string? nombre, ref string mensaje)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                mensaje = "No viaja el nombre de la localidad a consultar (capa datos)";
                return null;
            }
            nombre = nombre.ToLower();
            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Localidades.FirstOrDefault(l => l.Localidad.ToLower() == nombre);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener la localidad con el nombre {nombre}:\n{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene el domicilio asociado a una persona.
        /// </summary>
        /// <param name="persona">Persona con IdDomicilio.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Domicilio encontrado o null si no existe.</returns>
        public static Domicilios? GetDomicilioPorPersona(Personas? persona, ref string mensaje)
        {
            if (persona?.IdDomicilio == null)
            {
                mensaje = "No llegan los datos de la persona a la capa datos";
                return null;
            }
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    return contexto.Domicilios
                                   .Include(d => d.Localidades)
                                   .ThenInclude(l => l.Provincias)
                                   .FirstOrDefault(d => d.IdDomicilio == persona.IdDomicilio);
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(GetDomicilioPorPersona)";
                return null;
            }
        }

        #endregion

        #region Interacciones

        /// <summary>
        /// Registra una nueva localidad en la base de datos.
        /// Si la provincia no existe, la crea antes de registrar la localidad.
        /// </summary>
        /// <param name="localidad">Localidad a registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Id de la localidad registrada, 0 si no se procesa, -1 si ocurre un error.</returns>
        public static int RegistrarLocalidad(Localidades? localidad, ref string mensaje)
        {
            if (localidad == null || string.IsNullOrWhiteSpace(localidad?.Localidad))
                return 0;

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return 0;

                    // Si no llega IdProvincia, se intenta resolver con la entidad Provincias
                    if (localidad.IdProvincia == 0)
                    {
                        if (localidad.Provincias == null)
                        {
                            mensaje += "La localidad llega sin provincia al registro";
                            return -1;
                        }
                        else if (localidad.Provincias.IdProvincia > 0)
                        {
                            localidad.IdProvincia = localidad.Provincias.IdProvincia;
                        }
                        else
                        {
                            localidad.Provincias.IdProvincia = GetSiguienteIdProvincia(contexto);
                            if (localidad.Provincias.IdProvincia < 1)
                            {
                                mensaje += "Problemas al obtener ids de provincia";
                                return -1;
                            }
                            contexto.Provincias.Add(localidad.Provincias);
                            contexto.SaveChanges();
                            localidad.IdProvincia = localidad.Provincias.IdProvincia;
                        }
                        if (localidad.IdProvincia > 0)
                            localidad.Provincias = null; // Se limpia la referencia para evitar duplicados
                    }

                    if (localidad.Provincias != null)
                    {
                        mensaje += "Error al registrar provincias";
                        return -1;
                    }

                    localidad.IdLocalidad = GetSiguienteIdLocalidad(contexto);
                    if (localidad.IdLocalidad < 1)
                    {
                        mensaje = "No se puede generar un Id nuevo a la localidad";
                        return -1;
                    }

                    contexto.Add(localidad);
                    contexto.SaveChanges();
                    return localidad.IdLocalidad;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " (RegistrarLocalidad)";
                return -1;
            }
        }

        /// <summary>
        /// Registra un nuevo domicilio en la base de datos.
        /// </summary>
        /// <param name="domicilio">Domicilio a registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Id del domicilio registrado o -1 si ocurre un error.</returns>
        public static int RegistrarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "El domicilio no llega a la capa datos";
                return -1;
            }

            domicilio.IdDomicilio = null;

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return -1;

                    contexto.Domicilios.Add(domicilio);
                    contexto.SaveChanges();
                    return domicilio.IdDomicilio ?? 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(RegistrarDomicilio)";
                return -1;
            }
        }

        /// <summary>
        /// Modifica un domicilio existente.
        /// </summary>
        /// <param name="domicilio">Domicilio con datos actualizados.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si se modifica correctamente, False en caso contrario.</returns>
        public static bool ModificarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio?.IdDomicilio == null)
            {
                mensaje = "El domicilio o el Id no llega a la capa datos";
                return false;
            }

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.Domicilios.Update(domicilio);
                    int exito = contexto.SaveChanges();
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(ModificarDomicilio)";
                return false;
            }
        }

        /// <summary>
        /// Elimina un domicilio por su Id.
        /// </summary>
        /// <param name="id">Id del domicilio a eliminar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si se elimina correctamente, False en caso contrario.</returns>
        public static bool EliminarDomicilioPorId(int id, ref string mensaje)
        {
            if (id < 1)
            {
                mensaje = "No llega el id del domicilio a la capa datos";
                return false;
            }

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    Domicilios? d = contexto.Domicilios.FirstOrDefault(d => d.IdDomicilio == id);
                    if (d == null)
                    {
                        mensaje = "No se encuentra el domicilio con el Id proporcionado";
                        return false;
                    }

                    contexto.Remove(d);
                    int exito = contexto.SaveChanges();
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(EliminarDomicilioPorId)";
                return false;
            }
        }

        #endregion
    }
}

using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Personas.
    /// Contiene métodos de validación de contexto, obtención de listas,
    /// búsquedas específicas y edición de registros.
    /// </summary>
    public class PersonasDatos
    {
        #region Validaciones (contexto)

        /// <summary>
        /// Valida que el contexto tenga disponibles las entidades necesarias
        /// para operar con Personas, Domicilios, Localidades y Provincias.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>true si el contexto es válido; false en caso contrario.</returns>
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);

            if (!comprobar.ComprobarEntidad(contexto.Personas, ref mensaje))
                return false;
            if (!comprobar.ComprobarEntidad(contexto.Domicilios, ref mensaje))
                return false;
            if (!comprobar.ComprobarEntidad(contexto.Localidades, ref mensaje))
                return false;
            if (!comprobar.ComprobarEntidad(contexto.Provincias, ref mensaje))
                return false;

            return true;
        }

        #endregion

        #region Listados

        /// <summary>
        /// Obtiene la lista completa de personas, incluyendo sus domicilios,
        /// localidades y provincias asociadas.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de personas o null si ocurre un error.</returns>
        public static List<Personas>? GetPersonas(ref string mensaje)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades.Provincias)
                        .OrderBy(p => p.Apellidos)
                        .OrderBy(p => p.Nombres)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = $"Error al obtener el registro de personas\n{ex.Message}";
                return null;
            }
        }

        #endregion

        #region Búsquedas
                
        /// <summary>
        /// Obtiene una persona a partir de su DNI, incluyendo domicilio,
        /// localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento de la persona.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Objeto Persona o null si ocurre un error.</returns>
        public static Personas? GetPersonaPorDni(string dni, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni no llega a la consulta";
                return null;
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades.Provincias)
                        .FirstOrDefault(p => p.Dni == dni);
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener a la persona con Dni: {dni}\n{ex.Message}";
                Logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene una lista de personas filtradas por DNI o nombres.
        /// Incluye domicilio, localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento parcial o completo.</param>
        /// <param name="nombres">Nombres parciales o completos.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de personas o null si ocurre un error.</returns>
        public static List<Personas>? GetPersonasPorDniNombres(string? dni, string? nombres, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni) && String.IsNullOrWhiteSpace(nombres))
            {
                mensaje = "No llegan los datos de búsqueda";
                return null;
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades.Provincias)
                        .Where(p => (dni == null || p.Dni.Contains(dni)) ||
                                    (nombres == null || p.Nombres.Contains(nombres)))
                        .OrderBy(p => p.Apellidos)
                        .ThenBy(p => p.Nombres)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener a la persona buscada\n{ex.Message}";
                Logger.LogError(ex.Message);
                return null;
            }
        }

        #endregion

        #region Edición (registro y modificación)

        /// <summary>
        /// Registra una nueva persona en la base de datos.
        /// </summary>
        /// <param name="persona">Objeto Persona a registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Id de la persona registrada, -1 si ocurre un error.</returns>
        public static int RegistrarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de la persona no llega a la capa datos";
                return -1;
            }

            persona.IdPersona = null;

            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return -1;

                    contexto.Add(persona);
                    contexto.SaveChanges();
                    return persona.IdPersona ?? 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar a la persona\n{ex.Message}";
                Logger.LogError(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Modifica los datos de una persona existente en la base de datos.
        /// </summary>
        /// <param name="persona">Objeto Persona con datos actualizados.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Número de registros afectados, -1 si ocurre un error.</returns>
        public static int ModificarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "Los datos personales llegan vacíos";
                return -1;
            }

            if (persona.IdPersona < 1)
            {
                mensaje = "No llega el Id de la persona a la capa datos";
                return -1;
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return -1;

                    contexto.Update(persona);
                    int exito = contexto.SaveChanges();
                    return exito;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar los datos de la persona\n{ex.Message}";
                Logger.LogError(ex.Message);
                return -1;
            }
        }

        #endregion
    }
}

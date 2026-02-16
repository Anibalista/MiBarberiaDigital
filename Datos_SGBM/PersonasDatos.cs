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
        /// Comprueba que el contexto y la entidad Personas estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la comprobación.
        /// </returns>
        public static Resultado<bool> ComprobarContexto(Contexto contexto)
        {
            ComprobacionContexto comprobacion = new ComprobacionContexto(contexto);
            var resultado = comprobacion.ComprobarEntidad(contexto.Personas);
            if (!resultado.Success)
                return Resultado<bool>.Fail(resultado.Mensaje);
            return Resultado<bool>.Ok(true);
        }

        #endregion

        #region Listados

        /// <summary>
        /// Obtiene la lista completa de personas, incluyendo sus domicilios,
        /// localidades y provincias asociadas.
        /// </summary>
        /// <returns>
        /// Resultado con la lista de personas o mensaje de error.
        /// </returns>
        public static Resultado<List<Personas>> GetPersonas()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Personas>>.Fail(resultadoContexto.Mensaje);

                    var query = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .OrderBy(p => p.Apellidos)
                        .ThenBy(p => p.Nombres);

                    return Resultado<List<Personas>>.Ok(query.ToList());
                }
            }
            catch (Exception ex)
            {
                return Resultado<List<Personas>>.Fail($"Error al obtener el registro de personas:\n{ex.ToString()}");
            }
        }

        #endregion

        #region Búsquedas

        /// <summary>
        /// Obtiene una persona a partir de su DNI, incluyendo domicilio,
        /// localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento de la persona.</param>
        /// <returns>
        /// Resultado con el objeto Persona encontrado o mensaje de error.
        /// </returns>
        public static Resultado<Personas?> GetPersonaPorDni(string dni)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(dni))
                return Resultado<Personas?>.Fail("El DNI no llega a la consulta.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<Personas?>.Fail(resultadoContexto.Mensaje);

                    var persona = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .FirstOrDefault(p => p.Dni == dni);

                    if (persona == null)
                        return Resultado<Personas?>.Fail($"No se encontró una persona con DNI: {dni}");

                    return Resultado<Personas?>.Ok(persona);
                }
            }
            catch (Exception ex)
            {
                return Resultado<Personas?>.Fail($"Error al obtener a la persona con DNI {dni}:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Obtiene una lista de personas filtradas por DNI o nombres.
        /// Incluye domicilio, localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento parcial o completo.</param>
        /// <param name="nombres">Nombres parciales o completos.</param>
        /// <returns>
        /// Resultado con la lista de personas encontradas o mensaje de error.
        /// </returns>
        public static Resultado<List<Personas>> GetPersonasPorDniNombres(string? dni, string? nombres)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(dni) && string.IsNullOrWhiteSpace(nombres))
                return Resultado<List<Personas>>.Fail("No llegan los datos de búsqueda.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Personas>>.Fail(resultadoContexto.Mensaje);

                    var query = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .Where(p => (!string.IsNullOrWhiteSpace(dni) && p.Dni.Contains(dni))
                                 || (!string.IsNullOrWhiteSpace(nombres) && p.Nombres.Contains(nombres)))
                        .OrderBy(p => p.Apellidos)
                        .ThenBy(p => p.Nombres);

                    var personas = query.ToList();

                    if (personas.Count == 0)
                        return Resultado<List<Personas>>.Fail("No se encontraron personas con los criterios de búsqueda.");

                    return Resultado<List<Personas>>.Ok(personas);
                }
            }
            catch (Exception ex)
            {
                return Resultado<List<Personas>>.Fail($"Error al obtener a la persona buscada:\n{ex.ToString()}");
            }
        }

        #endregion

        #region Edición (registro y modificación)

        /// <summary>
        /// Registra una nueva persona en la base de datos.
        /// </summary>
        /// <param name="persona">Objeto Persona a registrar.</param>
        /// <returns>
        /// Resultado con el Id de la persona registrada o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarPersona(Personas? persona)
        {
            // Validación previa
            if (persona == null)
                return Resultado<int>.Fail("La persona no puede ser nula.");

            if (string.IsNullOrWhiteSpace(persona.Nombres))
                return Resultado<int>.Fail("El campo Nombres es obligatorio.");

            if (string.IsNullOrWhiteSpace(persona.Apellidos))
                return Resultado<int>.Fail("El campo Apellidos es obligatorio.");

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<int>.Fail(resultadoContexto.Mensaje);

                    // Autoincremental → Id en null
                    persona.IdPersona = null;

                    contexto.Personas.Add(persona);
                    contexto.SaveChanges();

                    if (persona.IdPersona != null && persona.IdPersona > 0)
                        return Resultado<int>.Ok((int)persona.IdPersona);
                    else
                        return Resultado<int>.Fail("No se pudo obtener el Id de la persona registrada.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail($"Error al registrar la persona:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Actualiza los datos de una persona existente.
        /// </summary>
        /// <param name="persona">Objeto Persona con los datos actualizados.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo.
        /// </returns>
        public static Resultado<bool> ModificarPersona(Personas? persona)
        {
            if (persona == null || persona.IdPersona == null || persona.IdPersona <= 0)
                return Resultado<bool>.Fail("Debe indicar una persona válida con IdPersona.");

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<bool>.Fail(resultadoContexto.Mensaje);

                    contexto.Personas.Update(persona);
                    int exito = contexto.SaveChanges();
                    return exito > 0 ? Resultado<bool>.Ok(true) : Resultado<bool>.Fail("No se pudo Actualizar la información de la persona");
                }
            }
            catch (Exception ex)
            {
                return Resultado<bool>.Fail($"Error al actualizar la persona:\n{ex.ToString()}");
            }
        }

        #endregion
    }
}

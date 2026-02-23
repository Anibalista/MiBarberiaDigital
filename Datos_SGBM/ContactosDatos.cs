using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Contactos.
    /// 
    /// Responsabilidades:
    /// - Proveer operaciones CRUD y consultas específicas relacionadas con contactos.
    /// - Gestionar la persistencia y resolución de referencias a entidades relacionadas (Personas, Proveedores)
    ///   evitando inserciones o actualizaciones accidentales de navegaciones.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet"/> usados
    ///   mediante <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar
    ///   tanto los datos como mensajes de error y facilitar el manejo en capas superiores.
    ///
    /// Diseño y buenas prácticas:
    /// - No contiene reglas de negocio; solo validaciones técnicas (nulls, ids válidos, existencia de DbSet).
    /// - Evita exponer excepciones crudas: registra detalles técnicos con <c>Logger</c> y devuelve mensajes
    ///   claros en <c>Resultado&lt;T&gt;</c>.
    /// - Al modificar entidades, recupera la entidad existente y asigna solo los campos permitidos
    ///   para evitar sobrescribir relaciones o provocar inserciones accidentales.
    /// - Usa <see cref="ComprobacionContexto.ComprobarEntidad"/> o <see cref="ComprobacionContexto.ComprobarEntidades"/>
    ///   según corresponda, para centralizar la comprobación del contexto y mantener mensajes consistentes.
    /// - Cada método crea y dispone su propio <see cref="Contexto"/> salvo que se diseñe explícitamente
    ///   para recibir uno (por ejemplo, para transacciones coordinadas en la capa de negocio).
    ///
    /// Consideraciones operativas:
    /// - Las operaciones que requieren atomicidad entre varias tablas deben orquestarse en la capa de negocio
    ///   con transacciones explícitas.
    /// - Para búsquedas por texto, preferir la collation de la base de datos o normalizar con
    ///   <c>ToLowerInvariant()</c> solo cuando sea necesario y no afecte el uso de índices.
    /// - Validaciones de formato (teléfono, email) y control de concurrencia (RowVersion) se implementan
    ///   en la capa de negocio o en iteraciones posteriores según necesidad.
    /// </summary>
    public class ContactosDatos
    {

        #region Consultas y Listados

        /// <summary>
        /// Obtiene la lista de contactos asociados a una persona.
        /// </summary>
        /// <param name="persona">Objeto Persona con IdPersona válido.</param>
        /// <returns>
        /// Resultado con la lista de contactos encontrados o mensaje de error.
        /// </returns>
        public static Resultado<List<Contactos>> GetContactosPorPersona(Personas? persona)
        {
            // Validación previa al uso del contexto
            if (persona == null || persona.IdPersona == null || persona.IdPersona <= 0)
                return Resultado<List<Contactos>>.Fail("La información de persona no llega correctamente a la consulta de contactos.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Contactos, nameof(contexto.Contactos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Contactos>>.Fail(rc.Mensaje);
                    }

                    var contactos = contexto.Contactos
                        .Where(c => c.IdPersona == persona.IdPersona)
                        .ToList();

                    if (contactos == null || contactos.Count < 1)
                        return Resultado<List<Contactos>>.Fail($"No se encontraron contactos para la persona con Id {persona.IdPersona}.");

                    return Resultado<List<Contactos>>.Ok(contactos);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener contactos de la persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Contactos>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene la lista de contactos filtrados por número de teléfono fijo o WhatsApp.
        /// </summary>
        /// <param name="fijo">Número de teléfono fijo parcial o completo.</param>
        /// <param name="whatsapp">Número de WhatsApp parcial o completo.</param>
        /// <returns>
        /// Resultado con la lista de contactos encontrados o mensaje de error.
        /// </returns>
        public static Resultado<List<Contactos>> GetContactosPorNumero(string? fijo, string? whatsapp)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(fijo) && string.IsNullOrWhiteSpace(whatsapp))
                return Resultado<List<Contactos>>.Fail("No llegan los datos de búsqueda de contactos.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Contactos, nameof(contexto.Contactos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Contactos>>.Fail(rc.Mensaje);
                    }

                    var query = contexto.Contactos.AsQueryable();

                    // Aplicar filtros solo si vienen criterios
                    query = query.Where(c =>
                        (!string.IsNullOrWhiteSpace(fijo) && c.Telefono != null && c.Telefono.Contains(fijo!))
                        || (!string.IsNullOrWhiteSpace(whatsapp) && c.Whatsapp != null && c.Whatsapp.Contains(whatsapp!))
                    );

                    var contactos = query.ToList();

                    if (contactos == null || contactos.Count < 1)
                        return Resultado<List<Contactos>>.Fail("No se encontraron contactos con los criterios de búsqueda.");

                    return Resultado<List<Contactos>>.Ok(contactos);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener contactos por número:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Contactos>>.Fail(msg);
            }
        }

        #endregion

        #region Interacción con BD (registro, modificación, etc)

        /// <summary>
        /// Registra un nuevo contacto en la base de datos.
        /// </summary>
        /// <param name="contacto">Objeto Contacto a registrar.</param>
        /// <returns>
        /// Resultado con el Id del contacto registrado o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarContacto(Contactos? contacto)
        {
            if (contacto == null)
                return Resultado<int>.Fail("El contacto no puede ser nulo.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Contactos, nameof(contexto.Contactos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Evitamos referencias circulares al registrar
                    contacto.Personas = null;
                    contacto.Proveedores = null;

                    // Contactos tiene autoincremental → IdContacto en null
                    contacto.IdContacto = null;

                    contexto.Contactos.Add(contacto);
                    int exito = contexto.SaveChanges();

                    if (exito > 0 && contacto.IdContacto != null && contacto.IdContacto > 0)
                        return Resultado<int>.Ok(contacto.IdContacto.Value);

                    var msg = "Problemas desconocidos en el registro de contactos.";
                    Logger.LogError(msg);
                    return Resultado<int>.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar el contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica los datos de un contacto existente.
        /// </summary>
        /// <param name="contacto">Objeto Contacto con los datos actualizados.</param>
        /// <returns>Resultado indicando éxito o fallo en la modificación.</returns>
        public static Resultado<bool> ModificarContacto(Contactos? contacto)
        {
            if (contacto == null || contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("Debe indicar un contacto válido con IdContacto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Contactos, nameof(contexto.Contactos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    // Buscar el contacto existente
                    var cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                    if (cont == null)
                        return Resultado<bool>.Fail($"No se encontró el contacto con Id {contacto.IdContacto}.");

                    // Actualizar campos permitidos (evitar sobrescribir navegaciones completas)
                    cont.Whatsapp = contacto.Whatsapp;
                    cont.Telefono = contacto.Telefono;
                    cont.Email = contacto.Email;
                    cont.Facebook = contacto.Facebook;
                    cont.Instagram = contacto.Instagram;
                    cont.IdPersona = contacto.IdPersona;
                    cont.IdProveedor = contacto.IdProveedor;

                    int exito = contexto.SaveChanges();

                    return exito > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo actualizar la información del contacto.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar el contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un contacto existente de la base de datos.
        /// </summary>
        /// <param name="contacto">Objeto Contacto con IdContacto válido.</param>
        /// <returns>Resultado indicando éxito o fallo en la eliminación.</returns>
        public static Resultado<bool> EliminarContacto(Contactos? contacto)
        {
            if (contacto == null || contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("Debe indicar un contacto válido con IdContacto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Contactos, nameof(contexto.Contactos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    var cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                    if (cont == null)
                        return Resultado<bool>.Fail($"No se encontró el contacto con Id {contacto.IdContacto} para eliminar.");

                    contexto.Contactos.Remove(cont);
                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo eliminar el contacto.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al eliminar el contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion
    }
}

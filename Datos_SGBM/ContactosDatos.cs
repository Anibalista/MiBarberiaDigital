using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    public class ContactosDatos
    {
        /// <summary>
        /// Comprueba que el contexto y la entidad Contactos estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la comprobación.
        /// </returns>
        public static Resultado<bool> ComprobarContexto(Contexto contexto)
        {
            if (contexto == null)
                return Resultado<bool>.Fail("Problemas al conectar a la BD.");

            if (contexto.Contactos == null)
                return Resultado<bool>.Fail("Problemas al conectar a la BD (Contactos).");

            return Resultado<bool>.Ok(true);
        }

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
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Contactos>>.Fail(resultadoContexto.Mensaje);

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
                return Resultado<List<Contactos>>.Fail($"Error al obtener contactos de la persona:\n{ex.ToString()}");
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
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Contactos>>.Fail(resultadoContexto.Mensaje);

                    var contactos = contexto.Contactos
                        .Where(c =>
                            (!string.IsNullOrWhiteSpace(fijo) && c.Telefono != null && c.Telefono.Contains(fijo))
                            || (!string.IsNullOrWhiteSpace(whatsapp) && c.Whatsapp != null && c.Whatsapp.Contains(whatsapp))
                        )
                        .ToList();

                    if (contactos == null || contactos.Count < 1)
                        return Resultado<List<Contactos>>.Fail("No se encontraron contactos con los criterios de búsqueda.");

                    return Resultado<List<Contactos>>.Ok(contactos);
                }
            }
            catch (Exception ex)
            {
                return Resultado<List<Contactos>>.Fail($"Error al obtener contactos por número:\n{ex.ToString()}");
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
            // Validación previa al uso del contexto
            if (contacto == null)
                return Resultado<int>.Fail("El contacto no puede ser nulo.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<int>.Fail(resultadoContexto.Mensaje);

                    // Evitamos referencias circulares al registrar
                    contacto.Personas = null;
                    contacto.Proveedores = null;

                    // Contactos tiene autoincremental → IdContacto en null
                    contacto.IdContacto = null;

                    contexto.Contactos.Add(contacto);
                    int exito = contexto.SaveChanges();

                    if (exito > 0 && contacto.IdContacto != null && contacto.IdContacto > 0)
                        return Resultado<int>.Ok((int)contacto.IdContacto);

                    return Resultado<int>.Fail("Problemas desconocidos en el registro de contactos.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail($"Error al registrar el contacto:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Modifica los datos de un contacto existente.
        /// </summary>
        /// <param name="contacto">Objeto Contacto con los datos actualizados.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la modificación.
        /// </returns>
        public static Resultado<bool> ModificarContacto(Contactos? contacto)
        {
            // Validación previa al uso del contexto
            if (contacto == null || contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("Debe indicar un contacto válido con IdContacto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<bool>.Fail(resultadoContexto.Mensaje);

                    // Buscar el contacto existente
                    var cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                    if (cont == null)
                        return Resultado<bool>.Fail($"No se encontró el contacto con Id {contacto.IdContacto}.");

                    // Actualizar campos
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
                return Resultado<bool>.Fail($"Error al modificar el contacto:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Elimina un contacto existente de la base de datos.
        /// </summary>
        /// <param name="contacto">Objeto Contacto con IdContacto válido.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la eliminación.
        /// </returns>
        public static Resultado<bool> EliminarContacto(Contactos? contacto)
        {
            // Validación previa al uso del contexto
            if (contacto == null || contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("Debe indicar un contacto válido con IdContacto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<bool>.Fail(resultadoContexto.Mensaje);

                    var cont = contexto.Contactos.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);
                    if (cont == null)
                        return Resultado<bool>.Fail($"No se encontró el contacto con Id {contacto.IdContacto} para eliminar.");

                    contexto.Contactos.Remove(cont);
                    int exito = contexto.SaveChanges();

                    return exito > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo eliminar el contacto.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<bool>.Fail($"Error al eliminar el contacto:\n{ex.ToString()}");
            }
        }

        #endregion
    }
}

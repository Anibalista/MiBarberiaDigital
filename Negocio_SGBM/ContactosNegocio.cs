using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de contactos.
    /// Contiene validaciones y preparación de datos antes de acceder a la capa de datos.
    /// </summary>
    public class ContactosNegocio
    {
        /// <summary>
        /// Obtiene los contactos asociados a una persona.
        /// </summary>
        public static Resultado<List<Contactos>> GetContactosPorPersona(Personas? persona)
        {
            if (persona == null)
                return Resultado<List<Contactos>>.Fail("La información de la persona no llega a la capa negocio de contactos.");

            if (persona.IdPersona == null || persona.IdPersona <= 0)
                return Resultado<List<Contactos>>.Fail("El Id de la persona no es válido.");

            try
            {
                return ContactosDatos.GetContactosPorPersona(persona);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener contactos por persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Contactos>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene contactos filtrados por número fijo y/o WhatsApp.
        /// </summary>
        public static Resultado<List<Contactos>> GetContactosPorNumero(string? fijo, string? whatsapp)
        {
            if (string.IsNullOrWhiteSpace(fijo)) fijo = null;
            if (string.IsNullOrWhiteSpace(whatsapp)) whatsapp = null;

            if (fijo == null && whatsapp == null)
                return Resultado<List<Contactos>>.Fail("Debe especificar al menos un número para la búsqueda.");

            try
            {
                return ContactosDatos.GetContactosPorNumero(fijo, whatsapp);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener contactos por número:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Contactos>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo contacto en la base de datos.
        /// </summary>
        public static Resultado<int> RegistrarContacto(Contactos? contacto)
        {
            if (contacto == null)
                return Resultado<int>.Fail("La información de contacto no llega a la capa negocio.");

            try
            {
                return ContactosDatos.RegistrarContacto(contacto);
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un contacto existente en la base de datos.
        /// </summary>
        public static Resultado<bool> ModificarContacto(Contactos? contacto)
        {
            if (contacto == null)
                return Resultado<bool>.Fail("La información de contacto no llega a la capa negocio.");

            if (contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("El Id del contacto no es válido.");

            try
            {
                return ContactosDatos.ModificarContacto(contacto);
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un contacto existente en la base de datos.
        /// </summary>
        public static Resultado<bool> EliminarContacto(Contactos? contacto)
        {
            if (contacto == null)
                return Resultado<bool>.Fail("La información de contacto no llega a la capa negocio.");

            if (contacto.IdContacto == null || contacto.IdContacto <= 0)
                return Resultado<bool>.Fail("El Id del contacto no es válido.");

            try
            {
                return ContactosDatos.EliminarContacto(contacto);
            }
            catch (Exception ex)
            {
                var msg = $"Error al eliminar contacto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}

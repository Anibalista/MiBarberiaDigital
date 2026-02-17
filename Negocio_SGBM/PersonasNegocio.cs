using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Clase de capa negocio para la entidad Personas.
    /// Contiene métodos de validación, gestión de contactos,
    /// consultas específicas y edición de registros.
    /// </summary>

    public class PersonasNegocio
    {
        #region Validaciones

        /// <summary>
        /// Valida la información de una persona antes de operar en la capa negocio.
        /// Comprueba que los datos obligatorios estén presentes y ajusta las referencias
        /// de domicilio, localidad y provincia según corresponda.
        /// </summary>
        /// <param name="persona">Objeto Persona a comprobar.</param>
        /// <param name="idNull">Indica si el IdPersona debe inicializarse en null.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Objeto Persona validado o null si ocurre un error.</returns>
        static Personas? ComprobarPersona(Personas? persona, bool idNull, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de la persona no llega a la capa negocio";
                return null;
            }
            if (string.IsNullOrWhiteSpace(persona.Dni))
            {
                mensaje = "El Dni de la persona no llega a la capa negocio";
                return null;
            }
            if (string.IsNullOrWhiteSpace(persona.Nombres))
            {
                mensaje = "El Nombre/s de la persona no llega a la capa negocio";
                return null;
            }
            if (string.IsNullOrWhiteSpace(persona.Apellidos))
            {
                mensaje = "El Apellido/s de la persona no llega a la capa negocio";
                return null;
            }

            try
            {
                // Ajusta referencias de domicilio, localidad y provincia.
                
                if (persona.Domicilios != null)
                {
                    persona.Domicilios.IdDomicilio = null;
                    persona.IdDomicilio = null;
                    persona.Domicilios.IdLocalidad = persona.Domicilios.IdLocalidad > 0 
                        ? persona.Domicilios.IdLocalidad 
                        : (persona.Domicilios.Localidades?.IdLocalidad ?? 0);

                    if (persona.Domicilios.IdLocalidad > 0)
                        persona.Domicilios.Localidades = null;

                    if (persona.Domicilios.Localidades != null)
                    {
                        persona.Domicilios.Localidades.IdProvincia = persona.Domicilios.Localidades.Provincias?.IdProvincia ?? 0;
                        persona.Domicilios.Localidades.Provincias = persona.Domicilios.Localidades.IdProvincia > 0
                            ? null
                            : persona.Domicilios.Localidades.Provincias;
                    }
                }

                // Ajusta el IdPersona según el parámetro idNull.
                if (idNull)
                    persona.IdPersona = null;
                else if (persona.IdPersona == null || persona.IdPersona < 1)
                {
                    mensaje = "El id de registro de la persona no llega a la capa negocio";
                    return null;
                }

                return persona;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = "Error inesperado: " + ex.Message;
                return null;
            }
        }

        #endregion

        #region Métodos Mixtos
        /// <summary>
        /// Gestiona los contactos asociados a una persona.
        /// Método principal que orquesta las operaciones delegando en helpers más pequeños.
        /// </summary>
        public static bool GestionarContactosPorPersona(Personas? p, List<Contactos>? contactosEntrantes, ref string mensaje)
        {
            // Valida que la persona no sea nula.
            if (p == null)
            {
                mensaje += "\nNo se pudo gestionar los contactos por errores al obtener la información de la persona";
                return false;
            }

            // Obtiene la persona según Id o DNI.
            Personas? persona = p.IdPersona == null ? GetPersonaPorDni(p.Dni, ref mensaje) : p;

            // Valida que la persona exista.
            if (persona == null)
            {
                mensaje += "\nNo se pudo gestionar los contactos por errores al obtener la información de la persona";
                return false;
            }

            // Obtiene los contactos actuales de la persona.
            string errores = string.Empty;
            List<Contactos>? contactosPersona = null;
            try
            {
                contactosPersona = ContactosNegocio.getContactosPorPersona(persona, ref errores);
            }
            catch (Exception ex)
            {
                errores = ex.Message;
            }

            // Caso 1: no se reciben contactos entrantes -> eliminar todos los existentes.
            if (contactosEntrantes == null)
            {
                return EliminarTodosContactos(contactosPersona, ref mensaje, ref errores);
            }

            // Asigna el IdPersona a los contactos entrantes.
            foreach (Contactos c in contactosEntrantes)
                c.IdPersona = persona.IdPersona;

            // Caso 2: no hay contactos previos -> registrar todos los entrantes.
            if (contactosPersona == null)
            {
                return RegistrarTodosContactos(contactosEntrantes, ref mensaje, ref errores);
            }

            // Caso 3: existen contactos previos -> actualizar o registrar según corresponda,
            // y luego eliminar los que quedaron sin coincidencia.
            bool resultado = ActualizarRegistrarYEliminar(contactosPersona, contactosEntrantes, ref mensaje);
            return resultado;
        }

        #region Helpers privados para gestionarContactosPorPersona

        /// <summary>
        /// Elimina todos los contactos de la lista proporcionada.
        /// </summary>
        private static bool EliminarTodosContactos(List<Contactos>? contactosPersona, ref string mensaje, ref string errores)
        {
            // Valida que existan contactos a eliminar.
            if (contactosPersona == null)
            {
                mensaje += errores;
                return false;
            }

            int contadorExitos = 0;
            int contadorErrores = 0;

            // Itera y elimina cada contacto.
            foreach (Contactos c in contactosPersona)
            {
                if (ContactosNegocio.eliminarContacto(c, ref errores))
                    contadorExitos++;
                else
                    contadorErrores++;
            }

            // Construye el mensaje de resultado.
            mensaje += $"\nSe eliminaron {contadorExitos} contactos";
            if (contadorExitos != contactosPersona.Count)
            {
                mensaje += $"\nNo se pudieron eliminar {contadorErrores} contactos";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Registra todos los contactos entrantes cuando no existen contactos previos.
        /// </summary>
        private static bool RegistrarTodosContactos(List<Contactos> contactosEntrantes, ref string mensaje, ref string errores)
        {
            int contadorExitos = 0;
            int contadorErrores = 0;

            // Itera y registra cada contacto entrante.
            foreach (Contactos c in contactosEntrantes)
            {
                c.IdContacto = null;
                if (ContactosNegocio.registrarContacto(c, ref errores))
                    contadorExitos++;
                else
                    contadorErrores++;
            }

            // Construye el mensaje de resultado.
            mensaje = $"\nSe registraron {contadorExitos} contactos";
            if (contadorExitos != contactosEntrantes.Count)
            {
                mensaje += $"\nNo se pudieron registrar {contadorErrores} contactos";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Actualiza los contactos existentes, registra los nuevos y elimina los sobrantes.
        /// Devuelve true si la operación global fue exitosa.
        /// </summary>
        private static bool ActualizarRegistrarYEliminar(List<Contactos> contactosPersona, List<Contactos> contactosEntrantes, ref string mensaje)
        {
            string errores = string.Empty;
            int contadorExitosMod = 0;
            int contadorErroresMod = 0;
            int exitosRegistros = 0;
            int erroresRegistro = 0;

            // Recorre los entrantes y decide modificar o registrar.
            foreach (Contactos contacto in contactosEntrantes)
            {
                Contactos? cont = contactosPersona.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);

                if (cont != null)
                {
                    // Si existe, se elimina de la lista de previos y se intenta modificar.
                    contactosPersona.Remove(cont);
                    if (!ContactosNegocio.modificarContacto(contacto, ref errores))
                        contadorErroresMod++;
                    else
                        contadorExitosMod++;
                }
                else
                {
                    // Si no existe, se registra como nuevo.
                    contacto.IdContacto = null;
                    if (!ContactosNegocio.registrarContacto(contacto, ref errores))
                        erroresRegistro++;
                    else
                        exitosRegistros++;
                }

                // Loguea errores parciales sin interrumpir el flujo.
                if (!String.IsNullOrWhiteSpace(errores))
                {
                    Logger.LogError(errores);
                    errores = string.Empty;
                }
            }

            // Construye resumen de modificaciones y registros.
            string resumen = string.Empty;
            bool exito = true;

            if (contadorExitosMod > 0)
                resumen += $"\nSe modificaron {contadorExitosMod} contactos";
            if (contadorErroresMod > 0)
            {
                exito = false;
                resumen += $"\nNo se pudieron modificar {contadorErroresMod} contactos";
            }
            if (exitosRegistros > 0)
                resumen += $"\nSe registraron {exitosRegistros} contactos";
            if (erroresRegistro > 0)
            {
                exito = false;
                resumen += $"\nNo se pudieron registrar {erroresRegistro} contactos";
            }

            // Si quedaron contactos previos sin coincidencia, eliminarlos.
            int contadorExitosElim = 0;
            int contadorErroresElim = 0;
            if (contactosPersona.Count > 0)
            {
                foreach (Contactos contacto in contactosPersona)
                {
                    if (!ContactosNegocio.eliminarContacto(contacto, ref errores))
                        contadorErroresElim++;
                    else
                        contadorExitosElim++;

                    if (!String.IsNullOrWhiteSpace(errores))
                    {
                        Logger.LogError(errores);
                        errores = string.Empty;
                    }
                }
            }

            if (contadorExitosElim > 0)
                resumen += $"\nSe eliminaron {contadorExitosElim} contactos";
            if (contadorErroresElim > 0)
            {
                exito = false;
                resumen += $"\nNo se pudieron eliminar {contadorErroresElim} contactos";
            }

            // Agrega el resumen al mensaje final.
            if (!String.IsNullOrWhiteSpace(resumen))
                mensaje += resumen;

            return exito;
        }

        #endregion

        #endregion

        #region Consultas

        /// <summary>
        /// Obtiene una persona a partir de su DNI desde la capa negocio.
        /// </summary>
        /// <param name="dni">Número de documento de la persona.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Objeto Persona o null si ocurre un error.</returns>
        public static Personas? GetPersonaPorDni(string? dni, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni ingresado no llega a la consulta (Capa Negocio)";
                return null;
            }

            try
            {
                return PersonasDatos.GetPersonaPorDni(dni, ref mensaje);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = "Error inesperado: " + ex.Message;
                return null;
            }
        }

        #endregion

        #region Edición (registro y modificación)

        /// <summary>
        /// Registra una nueva persona en la base de datos desde la capa negocio.
        /// </summary>
        /// <param name="persona">Objeto Persona a registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Id de la persona registrada, -1 si ocurre un error.</returns>
        public static int RegistrarPersona(Personas? persona, ref string mensaje)
        {
            persona = ComprobarPersona(persona, true, ref mensaje);
            if (persona == null)
                return -1;

            try
            {
                return PersonasDatos.RegistrarPersona(persona, ref mensaje);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// Modifica los datos de una persona existente desde la capa negocio.
        /// </summary>
        /// <param name="persona">Objeto Persona con datos actualizados.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>true si la modificación fue exitosa; false en caso contrario.</returns>
        public static bool modificarPersona(Personas? persona, ref string mensaje)
        {
            persona = ComprobarPersona(persona, false, ref mensaje);

            if (persona == null)
                return false;

            if (persona.IdPersona < 1)
            {
                mensaje = "No llega el Id de la persona a la capa negocio";
                return false;
            }

            Personas? p = PersonasDatos.GetPersonaPorDni(persona.Dni, ref mensaje);
            if (p == null)
            {
                mensaje = "Problemas al obtener los datos de la persona";
                return false;
            }

            try
            {
                int modificada = PersonasDatos.ModificarPersona(persona, ref mensaje);

                if (modificada >= 0)
                    mensaje = string.Empty;

                if (p.IdDomicilio != null && persona.IdDomicilio == null)
                    return DomiciliosNegocio.EliminarDomicilio((int)p.IdDomicilio, ref mensaje);

                return modificada > 0;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        #endregion

    }
}

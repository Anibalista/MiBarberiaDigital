using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de Personas.
    /// 
    /// Responsabilidades:
    /// - Validar la información de las personas antes de operar en la capa de datos.
    /// - Ajustar referencias de entidades relacionadas (domicilios, localidades, provincias, contactos).
    /// - Delegar operaciones CRUD a <see cref="PersonasDatos"/> y coordinar con otras capas de negocio
    ///   como <see cref="DomiciliosNegocio"/> y <see cref="ContactosNegocio"/>.
    /// - Devolver resultados uniformes mediante <see cref="Resultado{T}"/> para simplificar el manejo de errores.
    /// 
    /// Buenas prácticas:
    /// - No usar parámetros por referencia para mensajes; todos los mensajes de error o éxito
    ///   se devuelven dentro de <see cref="Resultado{T}"/>.
    /// - Mantener validaciones simples en esta capa; reglas más complejas pueden residir en una
    ///   capa superior de servicios si el proyecto escala.
    /// - Registrar mensajes técnicos en <c>Logger</c> en la capa de datos; aquí solo se devuelven
    ///   mensajes amigables.
    /// 
    /// Métodos principales:
    /// - Registro y modificación de personas, incluyendo validación de datos obligatorios.
    /// - Consultas por DNI, Id y otros criterios.
    /// - Validación y normalización de referencias de domicilio, localidad y provincia.
    /// - Gestión de contactos asociados a una persona (registrar, modificar, eliminar).
    /// 
    /// Extensibilidad:
    /// - Permite añadir reglas de negocio adicionales (ej. auditoría, permisos, validaciones cruzadas).
    /// - Puede integrarse con servicios externos en el futuro (ej. verificación de identidad).
    /// </summary>
    public class PersonasNegocio
    {
        #region Validaciones

        /// <summary>
        /// Valida la información de una persona antes de operar en la capa negocio.
        /// Comprueba que los datos obligatorios estén presentes y ajusta las referencias
        /// de domicilio, localidad y provincia según corresponda.
        /// </summary>
        private static Resultado<Personas?> ComprobarPersona(Personas? persona, bool idNull)
        {
            if (persona == null)
                return Resultado<Personas?>.Fail("La información de la persona no llega a la capa negocio.");

            if (string.IsNullOrWhiteSpace(persona.Dni))
                return Resultado<Personas?>.Fail("El DNI de la persona no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(persona.Nombres))
                return Resultado<Personas?>.Fail("El nombre de la persona no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(persona.Apellidos))
                return Resultado<Personas?>.Fail("El apellido de la persona no puede estar vacío.");

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
                    return Resultado<Personas?>.Fail("El Id de la persona no es válido.");

                return Resultado<Personas?>.Ok(persona);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al comprobar persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Personas?>.Fail(msg);
            }
        }

        #endregion

        #region Métodos Mixtos

        /// <summary>
        /// Gestiona los contactos asociados a una persona.
        /// Método principal que orquesta las operaciones delegando en helpers más pequeños.
        /// </summary>
        public static Resultado<bool> GestionarContactosPorPersona(Personas? p, List<Contactos>? contactosEntrantes)
        {
            if (p == null)
                return Resultado<bool>.Fail("No se pudo gestionar los contactos: la persona no llega a la capa negocio.");

            // Obtiene la persona según Id o DNI
            Personas? persona = p.IdPersona == null
                ? GetPersonaPorDni(p.Dni).Data
                : p;

            if (persona == null)
                return Resultado<bool>.Fail("No se pudo gestionar los contactos: la persona no existe.");

            try
            {
                var resultadoContactos = ContactosNegocio.GetContactosPorPersona(persona);
                var contactosPersona = resultadoContactos.Data;

                // Caso 1: no se reciben contactos entrantes -> eliminar todos los existentes
                if (contactosEntrantes == null)
                    return EliminarTodosContactos(contactosPersona);

                // Asigna el IdPersona a los contactos entrantes
                foreach (var c in contactosEntrantes)
                    c.IdPersona = persona.IdPersona;

                // Caso 2: no hay contactos previos -> registrar todos los entrantes
                if (contactosPersona == null)
                    return RegistrarTodosContactos(contactosEntrantes);

                // Caso 3: existen contactos previos -> actualizar, registrar y eliminar sobrantes
                return ActualizarRegistrarYEliminar(contactosPersona, contactosEntrantes);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al gestionar contactos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #region Helpers privados

        private static Resultado<bool> EliminarTodosContactos(List<Contactos>? contactosPersona)
        {
            if (contactosPersona == null)
                return Resultado<bool>.Fail("No hay contactos para eliminar.");

            int eliminados = 0, errores = 0;

            foreach (var c in contactosPersona)
            {
                var resultado = ContactosNegocio.EliminarContacto(c);
                if (resultado.Success) eliminados++;
                else errores++;
            }

            if (errores > 0)
                return Resultado<bool>.Fail($"Se eliminaron {eliminados} contactos, pero {errores} no pudieron eliminarse.");

            return Resultado<bool>.Ok(true, $"Se eliminaron {eliminados} contactos.");
        }

        private static Resultado<bool> RegistrarTodosContactos(List<Contactos> contactosEntrantes)
        {
            int registrados = 0, errores = 0;

            foreach (var c in contactosEntrantes)
            {
                c.IdContacto = null;
                var resultado = ContactosNegocio.RegistrarContacto(c);
                if (resultado.Success) registrados++;
                else errores++;
            }

            if (errores > 0)
                return Resultado<bool>.Fail($"Se registraron {registrados} contactos, pero {errores} no pudieron registrarse.");

            return Resultado<bool>.Ok(true, $"Se registraron {registrados} contactos.");
        }

        private static Resultado<bool> ActualizarRegistrarYEliminar(List<Contactos> contactosPersona, List<Contactos> contactosEntrantes)
        {
            int modificados = 0, erroresMod = 0, registrados = 0, erroresReg = 0, eliminados = 0, erroresElim = 0;

            foreach (var contacto in contactosEntrantes)
            {
                var existente = contactosPersona.FirstOrDefault(c => c.IdContacto == contacto.IdContacto);

                if (existente != null)
                {
                    contactosPersona.Remove(existente);
                    var resultado = ContactosNegocio.ModificarContacto(contacto);
                    if (resultado.Success) modificados++;
                    else erroresMod++;
                }
                else
                {
                    contacto.IdContacto = null;
                    var resultado = ContactosNegocio.RegistrarContacto(contacto);
                    if (resultado.Success) registrados++;
                    else erroresReg++;
                }
            }

            foreach (var contacto in contactosPersona)
            {
                var resultado = ContactosNegocio.EliminarContacto(contacto);
                if (resultado.Success) eliminados++;
                else erroresElim++;
            }

            var resumen = $"Modificados: {modificados}, Registrados: {registrados}, Eliminados: {eliminados}";
            if (erroresMod + erroresReg + erroresElim > 0)
                return Resultado<bool>.Fail($"Errores al gestionar contactos. {resumen}");

            return Resultado<bool>.Ok(true, $"Gestión de contactos exitosa. {resumen}");
        }

        #endregion

        #endregion

        #region Consultas

        /// <summary>
        /// Obtiene una persona a partir de su DNI desde la capa negocio.
        /// </summary>
        /// <param name="dni">Número de documento de la persona.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la persona encontrada en caso de éxito,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<Personas?> GetPersonaPorDni(string? dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return Resultado<Personas?>.Fail("El DNI ingresado no llega a la consulta (capa negocio).");

            try
            {
                return PersonasDatos.GetPersonaPorDni(dni);
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al obtener persona por DNI:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Personas?>.Fail(msg);
            }
        }

        #endregion

        #region Edición (registro y modificación)

        /// <summary>
        /// Registra una nueva persona en la base de datos desde la capa negocio.
        /// </summary>
        /// <param name="persona">Objeto Persona a registrar.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el Id de la persona registrada en caso de éxito,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<int> RegistrarPersona(Personas? persona)
        {
            var validacion = ComprobarPersona(persona, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            try
            {
                return PersonasDatos.RegistrarPersona(persona!);
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica los datos de una persona existente desde la capa negocio.
        /// </summary>
        /// <param name="persona">Objeto Persona con datos actualizados.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con true si la modificación fue exitosa,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<bool> ModificarPersona(Personas? persona)
        {
            var validacion = ComprobarPersona(persona, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            if (persona!.IdPersona == null || persona.IdPersona < 1)
                return Resultado<bool>.Fail("No llega el Id de la persona a la capa negocio.");

            try
            {
                // Verificar existencia por DNI
                var resultadoExistente = PersonasDatos.GetPersonaPorDni(persona.Dni);
                if (!resultadoExistente.Success || resultadoExistente.Data == null)
                    return Resultado<bool>.Fail("Problemas al obtener los datos de la persona.");

                var existente = resultadoExistente.Data;

                // Modificar persona
                var resultadoModificar = PersonasDatos.ModificarPersona(persona);
                if (!resultadoModificar.Success)
                    return Resultado<bool>.Fail(resultadoModificar.Mensaje);

                // Si tenía domicilio y ahora se eliminó, borrarlo
                if (existente.IdDomicilio != null && persona.IdDomicilio == null)
                {
                    var resultadoEliminar = DomiciliosNegocio.EliminarDomicilio(existente.IdDomicilio.Value);
                    if (!resultadoEliminar.Success)
                        return Resultado<bool>.Fail(resultadoEliminar.Mensaje);
                }

                return Resultado<bool>.Ok(true, "Persona modificada correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

    }
}

using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Empleados.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de acceso y persistencia relacionadas con empleados
    ///   (búsqueda por persona, listados, registro, modificación, eliminación, etc.).
    /// - Incluir relaciones de lectura necesarias (por ejemplo <c>Personas</c>, <c>Estados</c>, domicilios y localidades)
    ///   únicamente cuando el método lo requiera para evitar consultas N+1 y cargas innecesarias.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Empleados}"/>
    ///   mediante la clase <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar datos y mensajes de error.
    ///
    /// Buenas prácticas y diseño:
    /// - Mantener la clase enfocada en acceso a datos; las reglas de negocio, validaciones complejas y políticas
    ///   de autorización deben residir en la capa de negocio.
    /// - Al actualizar, recuperar la entidad existente y asignar únicamente los campos escalares permitidos
    ///   para evitar modificaciones accidentales de relaciones de navegación.
    /// - Normalizar cadenas mínimas (por ejemplo <c>Trim()</c>) antes de persistir o comparar.
    /// - Registrar errores técnicos con <c>Logger</c> y devolver mensajes amigables dentro de <c>Resultado&lt;T&gt;</c>.
    /// - No usar parámetros por referencia para mensajes; devolver siempre el mensaje dentro del <see cref="Resultado{T}"/>.
    ///
    /// Consideraciones técnicas y operativas:
    /// - Empleados es autoincremental; dejar <c>IdEmpleado</c> en null al insertar para que la BD genere el valor.
    /// - Verificar la existencia de las entidades referenciadas (<c>Personas</c>, <c>Estados</c>) antes de insertar o actualizar.
    /// - Implementar paginación en consultas que puedan devolver muchos registros (aplicar <c>Skip/Take</c>).
    /// - Evitar aplicar funciones CLR sobre columnas en WHERE que impidan el uso de índices; preferir collations
    ///   case‑insensitive o usar <c>EF.Functions.Like</c> cuando proceda.
    /// - Al insertar, fijar campos de auditoría controlados por la capa de datos (por ejemplo <c>FechaIngreso</c>) según la política.
    ///
    /// Seguridad y robustez:
    /// - No exponer excepciones crudas; capturar excepciones, registrar detalles técnicos y devolver mensajes útiles
    ///   en <c>Resultado&lt;T&gt;</c>.
    /// - Validar entradas (nulls, longitudes máximas, rangos numéricos) antes de persistir.
    /// - Recuperar entidades desde el contexto antes de eliminarlas o modificarlas para evitar problemas con entidades desconectadas.
    ///
    /// Métodos esperados y su comportamiento:
    /// - <c>GetEmpleadoPorIdPersona(int, bool completo = false)</c>: devuelve un empleado por IdPersona; si <c>completo</c>
    ///   es true incluye <c>Personas</c> con domicilios y localidades y <c>Estados</c>; si es false devuelve una proyección más ligera.
    /// - <c>GetEmpleados()</c>: devuelve todos los empleados ordenados por apellido y nombre, incluyendo relaciones necesarias.
    /// - <c>RegistrarEmpleado(Empleados)</c>: inserta un nuevo empleado y devuelve el Id generado en <c>Resultado&lt;int&gt;</c>.
    /// - <c>ModificarEmpleado(Empleados)</c>: actualizar campos permitidos tras verificar existencia y consistencia referencial.
    /// - <c>EliminarEmpleado</c> (físico o lógico según política): eliminar o marcar como inactivo tras verificar existencia.
    ///
    /// Extensibilidad:
    /// - Centralizar la traducción de criterios de búsqueda y validaciones en helpers reutilizables para mantener consistencia.
    /// - Considerar el uso de DTOs o proyecciones para consultas que devuelvan grandes volúmenes de datos o para la UI.
    /// - Añadir pruebas unitarias e integración para cada método público y para los helpers de filtrado y generación de Id.
    /// </summary>
    public class EmpleadosDatos
    {
        /// <summary>
        /// Obtiene un Empleado a partir del ID de persona.
        /// </summary>
        /// <param name="idPersona">ID de la persona asociada al Empleado (debe ser mayor que 0).</param>
        /// <param name="completo">Bandera para traer todos los datos relacionados (Personas con domicilios y localidades). Por defecto false.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el <see cref="Empleados"/> encontrado o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida el parámetro de entrada y la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Empleados}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Si <paramref name="completo"/> es true incluye las relaciones Personas -> Domicilios -> Localidades y Estados;
        ///   si es false incluye únicamente Estados para una consulta más ligera.
        /// - Registra detalles técnicos con <c>Logger</c> y devuelve mensajes amigables dentro de <see cref="Resultado{T}"/>.
        /// </remarks>
        public static Resultado<Empleados?> GetEmpleadoPorIdPersona(int idPersona, bool completo = false)
        {
            if (idPersona < 1)
                return Resultado<Empleados?>.Fail("El ID de la persona no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Empleados, nameof(contexto.Empleados));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<Empleados?>.Fail(rc.Mensaje);
                }

                Empleados? empleado;

                if (completo)
                {
                    empleado = contexto.Empleados
                                       .Include(e => e.Estados)
                                       .Include(e => e.Personas)
                                           .ThenInclude(p => p.Domicilios)
                                               .ThenInclude(d => d.Localidades)
                                       .FirstOrDefault(e => e.IdPersona == idPersona);
                }
                else
                {
                    empleado = contexto.Empleados
                                       .Include(e => e.Estados)
                                       .FirstOrDefault(e => e.IdPersona == idPersona);
                }

                if (empleado == null)
                    return Resultado<Empleados?>.Fail($"No se encontró un empleado asociado a la persona con Id {idPersona}.");

                return Resultado<Empleados?>.Ok(empleado);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de empleado por IdPersona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Empleados?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene todos los Empleados ordenados por apellido y nombre.
        /// </summary>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="Empleados"/> incluyendo <see cref="Estados"/> y
        /// <see cref="Personas"/> (con sus domicilios y localidades), o un <see cref="Resultado{T}"/> con el mensaje de error.
        /// </returns>
        /// <remarks>
        /// - Valida la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Empleados}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/> antes de ejecutar la consulta.
        /// - Incluye relaciones necesarias (Estados, Personas -> Domicilios -> Localidades) para evitar N+1.
        /// - No usa parámetros por referencia para mensajes; todos los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// - Registra detalles técnicos con <c>Logger</c> y devuelve mensajes amigables para la capa superior.
        /// </remarks>
        public static Resultado<List<Empleados>> GetEmpleados()
        {
            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Empleados, nameof(contexto.Empleados));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<Empleados>>.Fail(rc.Mensaje);
                }

                // Incluir relaciones necesarias y ordenar por Apellidos, Nombres
                var lista = contexto.Empleados
                                    .Include(e => e.Estados)
                                    .Include(e => e.Personas)
                                        .ThenInclude(p => p.Domicilios)
                                            .ThenInclude(d => d.Localidades)
                                    .OrderBy(e => e.Personas.Apellidos)
                                    .ThenBy(e => e.Personas.Nombres)
                                    .ToList();

                if (lista == null || lista.Count == 0)
                    return Resultado<List<Empleados>>.Fail("No se encontraron empleados.");

                return Resultado<List<Empleados>>.Ok(lista);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener empleados (capa datos):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Empleados>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo Empleado en la base de datos.
        /// </summary>
        /// <param name="empleado">Objeto <see cref="Empleados"/> a registrar (no puede ser nulo).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el Id del empleado registrado en caso de éxito,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la entrada y la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet{T}"/>
        ///   relevantes mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Comprueba la existencia de las entidades referenciadas (<c>Personas</c>, <c>Estados</c>) antes de insertar.
        /// - Establece <c>IdEmpleado</c> en null para permitir que la base de datos genere el valor (autoincremental)
        ///   y fija <c>FechaIngreso</c> en el momento de la inserción.
        /// - No usa parámetros por referencia para mensajes; todos los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// </remarks>
        public static Resultado<int> RegistrarEmpleado(Empleados? empleado)
        {
            if (empleado == null)
                return Resultado<int>.Fail("El empleado no puede ser nulo.");

            // Validaciones básicas de integridad referencial
            if (empleado.IdPersona <= 0)
                return Resultado<int>.Fail("El Id de la persona asociada no es válido.");

            if (empleado.IdEstado <= 0)
                return Resultado<int>.Fail("El Id del estado asociado no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet Empleados mediante ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rcEmpleados = comprobacion.ComprobarEntidad(contexto.Empleados, nameof(contexto.Empleados));
                if (!rcEmpleados.Success)
                {
                    Logger.LogError(rcEmpleados.Mensaje);
                    return Resultado<int>.Fail(rcEmpleados.Mensaje);
                }

                // Verificar que la Persona exista
                var rcPersonas = comprobacion.ComprobarEntidad(contexto.Personas, nameof(contexto.Personas));
                if (!rcPersonas.Success)
                {
                    Logger.LogError(rcPersonas.Mensaje);
                    return Resultado<int>.Fail(rcPersonas.Mensaje);
                }

                var personaExistente = contexto.Personas.Find(empleado.IdPersona);
                if (personaExistente == null)
                    return Resultado<int>.Fail($"No se encontró la persona con Id {empleado.IdPersona}.");

                // Verificar que el Estado exista
                var rcEstados = comprobacion.ComprobarEntidad(contexto.Estados, nameof(contexto.Estados));
                if (!rcEstados.Success)
                {
                    Logger.LogError(rcEstados.Mensaje);
                    return Resultado<int>.Fail(rcEstados.Mensaje);
                }

                var estadoExistente = contexto.Estados.Find(empleado.IdEstado);
                if (estadoExistente == null)
                    return Resultado<int>.Fail($"No se encontró el estado con Id {empleado.IdEstado}.");

                // Preparar entidad para inserción (IdEmpleado es autoincremental)
                empleado.IdEmpleado = null;
                empleado.FechaIngreso = DateTime.Now;

                contexto.Empleados.Add(empleado);
                contexto.SaveChanges();

                if (empleado.IdEmpleado != null && empleado.IdEmpleado > 0)
                    return Resultado<int>.Ok(empleado.IdEmpleado.Value);

                var msg = "No se pudo obtener el Id del empleado registrado.";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
            catch (Exception ex)
            {
                var personaInfo = empleado?.IdPersona.ToString() ?? "<sin persona>";
                var msg = $"Error al registrar el empleado (IdPersona: {personaInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un Empleado existente en la base de datos.
        /// </summary>
        /// <param name="empleado">Empleado con los cambios a aplicar (debe incluir IdEmpleado).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con <c>true</c> si la modificación fue exitosa, o con el mensaje de error en caso contrario.
        /// </returns>
        public static Resultado<bool> ModificarEmpleado(Empleados? empleado)
        {
            if (empleado == null)
                return Resultado<bool>.Fail("No se recibió información del empleado.");

            if (empleado.IdEmpleado == null || empleado.IdEmpleado <= 0)
                return Resultado<bool>.Fail("El Id del empleado no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Empleados, nameof(contexto.Empleados));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<bool>.Fail(rc.Mensaje);
                }

                // Recuperar la entidad existente para aplicar cambios controlados
                var id = empleado.IdEmpleado.Value;
                var existente = contexto.Empleados
                                        .FirstOrDefault(e => e.IdEmpleado == id);

                if (existente == null)
                    return Resultado<bool>.Fail($"No se encontró el empleado con Id {id}.");

                // Validaciones básicas de integridad referencial
                if (empleado.IdPersona <= 0)
                    return Resultado<bool>.Fail("El Id de la persona asociada no es válido.");

                if (empleado.IdEstado <= 0)
                    return Resultado<bool>.Fail("El Id del estado asociado no es válido.");

                // Actualizar solo campos escalares permitidos
                existente.TipoEmpleado = empleado.TipoEmpleado?.Trim();
                existente.IdPersona = empleado.IdPersona;
                existente.IdEstado = empleado.IdEstado;

                // Actualizar FechaIngreso solo si el valor proporcionado es razonable (evitar sobrescribir con default)
                if (empleado.FechaIngreso != default && empleado.FechaIngreso != existente.FechaIngreso)
                    existente.FechaIngreso = empleado.FechaIngreso;

                // Guardar cambios
                var exito = contexto.SaveChanges();

                if (exito > 0)
                    return Resultado<bool>.Ok(true);

                var msg = "No se pudo modificar el empleado.";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
            catch (Exception ex)
            {
                var idInfo = empleado?.IdEmpleado?.ToString() ?? "<sin id>";
                var msg = $"Error al modificar empleado (Id: {idInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

    }
}

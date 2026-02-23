using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Clientes.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de acceso y persistencia relacionadas con clientes
    ///   (búsquedas por persona, listados, filtrado por DNI/nombres, registro, modificación, eliminación, etc.).
    /// - Incluir relaciones de lectura necesarias (por ejemplo <c>Personas</c>, <c>Estados</c>, domicilios y localidades)
    ///   únicamente cuando el método lo requiera para evitar consultas N+1 y cargas innecesarias.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Clientes}"/>
    ///   mediante la clase <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar datos y mensajes de error.
    /// </summary>
    /// 
    /// <remarks>
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
    /// - Clientes es autoincremental; dejar <c>IdCliente</c> en null al insertar para que la BD genere el valor.
    /// - Algunos catálogos relacionados (por ejemplo <c>Estados</c>) pueden no ser autoincrementales; si la aplicación
    ///   debe generar claves, usar un helper seguro que obtenga el siguiente Id dentro de una transacción.
    /// - Implementar paginación en consultas que puedan devolver muchos registros (aplicar <c>Skip/Take</c>).
    /// - Evitar aplicar funciones CLR sobre columnas en WHERE que impidan el uso de índices; preferir collations
    ///   case‑insensitive o usar <c>EF.Functions.Like</c> cuando proceda.
    ///
    /// Seguridad y robustez:
    /// - No exponer excepciones crudas; capturar excepciones, registrar detalles técnicos y devolver mensajes útiles
    ///   en <c>Resultado&lt;T&gt;</c>.
    /// - Validar entradas (nulls, longitudes máximas, rangos numéricos) antes de persistir.
    /// - Verificar la existencia de entidades referenciadas (Personas, Estados) antes de insertar o actualizar.
    /// - Recuperar entidades desde el contexto antes de eliminarlas o modificarlas para evitar problemas con entidades desconectadas.
    ///
    /// Métodos esperados y su comportamiento:
    /// - <c>GetClientePorIdPersona(int)</c>: devuelve un cliente por IdPersona incluyendo relaciones necesarias.
    /// - <c>GetClientes()</c>: devuelve todos los clientes ordenados por apellido y nombre.
    /// - <c>GetClientesPorDniNombres(string dni, string nombres)</c>: búsqueda flexible por DNI y/o nombres/apellidos.
    /// - <c>RegistrarCliente(Clientes)</c>: inserta un nuevo cliente (autoincremental) y devuelve el Id generado.
    /// - <c>ModificarCliente(Clientes)</c>: actualizar campos permitidos tras verificar existencia.
    /// - <c>EliminarCliente</c> (físico o lógico según política): eliminar o marcar como inactivo tras verificar existencia.
    ///
    /// Extensibilidad:
    /// - Centralizar la traducción de criterios de búsqueda y validaciones en helpers reutilizables para mantener consistencia.
    /// - Considerar el uso de DTOs o proyecciones para consultas que devuelvan grandes volúmenes de datos o para la UI.
    /// - Añadir pruebas unitarias e integración para cada método público y para los helpers de filtrado y generación de Id.
    /// </remarks>
    public class ClientesDatos
    {
        /// <summary>
        /// Obtiene un cliente a partir del ID de persona.
        /// </summary>
        /// <param name="idPersona">ID de la persona asociada al cliente (debe ser mayor que 0).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el <see cref="Clientes"/> encontrado o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida el parámetro de entrada y la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Clientes}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Incluye las relaciones necesarias (Personas -> Domicilios -> Localidades) para evitar consultas N+1 cuando la capa superior
        ///   necesite esos datos.
        /// - No utiliza parámetros por referencia para mensajes; todos los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// - Registra detalles técnicos con <c>Logger</c> y devuelve mensajes amigables para la capa superior.
        /// </remarks>
        public static Resultado<Clientes?> GetClientePorIdPersona(int idPersona)
        {
            if (idPersona < 1)
                return Resultado<Clientes?>.Fail("El ID de la persona no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<Clientes?>.Fail(rc.Mensaje);
                }

                // Incluir relaciones necesarias y buscar por IdPersona
                var cliente = contexto.Clientes
                                      .Include(c => c.Personas)
                                        .ThenInclude(p => p.Domicilios)
                                          .ThenInclude(d => d.Localidades)
                                      .FirstOrDefault(c => c.IdPersona == idPersona);

                if (cliente == null)
                    return Resultado<Clientes?>.Fail($"No se encontró un cliente asociado a la persona con Id {idPersona}.");

                return Resultado<Clientes?>.Ok(cliente);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de cliente por IdPersona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Clientes?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene todos los clientes ordenados por apellido y nombre.
        /// </summary>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="Clientes"/> incluyendo <see cref="Estados"/> y
        /// <see cref="Personas"/> (con sus domicilios y localidades), o un <see cref="Resultado{T}"/> con el mensaje de error.
        /// </returns>
        public static Resultado<List<Clientes>> GetClientes()
        {
            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<Clientes>>.Fail(rc.Mensaje);
                }

                // Incluir relaciones necesarias y ordenar por Apellidos, Nombres
                var lista = contexto.Clientes
                                    .Include(c => c.Estados)
                                    .Include(c => c.Personas)
                                        .ThenInclude(p => p.Domicilios)
                                            .ThenInclude(d => d.Localidades)
                                                .ThenInclude(l => l.Provincias) // incluir provincia si es necesario para la capa superior
                                    .OrderBy(c => c.Personas.Apellidos)
                                    .ThenBy(c => c.Personas.Nombres)
                                    .ToList();

                if (lista == null || lista.Count == 0)
                    return Resultado<List<Clientes>>.Fail("No se encontraron clientes.");

                return Resultado<List<Clientes>>.Ok(lista);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener clientes (capa datos):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Clientes>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene clientes filtrados por DNI y/o nombres.
        /// </summary>
        /// <param name="dni">DNI del cliente (opcional).</param>
        /// <param name="nombres">Nombres o apellidos del cliente (opcional).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="Clientes"/> que cumplen los criterios,
        /// o un <see cref="Resultado{T}"/> con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida que al menos un criterio de búsqueda llegue; no usa parámetros por referencia para mensajes.
        /// - Usa <see cref="ComprobacionContexto"/> para verificar la disponibilidad del <see cref="Contexto"/> y del DbSet.
        /// - Normaliza los criterios a minúsculas y aplica comparaciones case‑insensitive traducibles a SQL mediante <c>ToLower()</c>.
        /// - Incluye relaciones necesarias (Estados, Personas -> Domicilios -> Localidades) para evitar N+1.
        /// - Si no se encuentran resultados devuelve un <see cref="Resultado{T}"/> con mensaje informativo.
        /// </remarks>
        public static Resultado<List<Clientes>> GetClientesPorDniNombres(string? dni, string? nombres)
        {
            // Valida que al menos un dato de búsqueda llegue
            if (string.IsNullOrWhiteSpace(dni) && string.IsNullOrWhiteSpace(nombres))
                return Resultado<List<Clientes>>.Fail("No llegan los datos de búsqueda a la consulta.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<Clientes>>.Fail(rc.Mensaje);
                }

                // Normalizar criterios
                var dniTrimLower = dni?.Trim().ToLower();
                var nombresTrimLower = nombres?.Trim().ToLower();

                // Base query con includes
                var query = contexto.Clientes
                                    .Include(c => c.Estados)
                                    .Include(c => c.Personas)
                                        .ThenInclude(p => p.Domicilios)
                                            .ThenInclude(d => d.Localidades)
                                                .ThenInclude(l => l.Provincias)
                                    .AsQueryable();

                // Aplicar filtros según los parámetros recibidos
                if (!string.IsNullOrWhiteSpace(dniTrimLower) && !string.IsNullOrWhiteSpace(nombresTrimLower))
                {
                    // Ambos criterios: DNI y (Nombres o Apellidos)
                    query = query.Where(c =>
                        c.Personas != null &&
                        c.Personas.Dni != null &&
                        c.Personas.Dni.ToLower().Contains(dniTrimLower) &&
                        ((c.Personas.Nombres != null && c.Personas.Nombres.ToLower().Contains(nombresTrimLower)) ||
                         (c.Personas.Apellidos != null && c.Personas.Apellidos.ToLower().Contains(nombresTrimLower)))
                    );
                }
                else if (!string.IsNullOrWhiteSpace(dniTrimLower))
                {
                    // Solo DNI
                    query = query.Where(c =>
                        c.Personas != null &&
                        c.Personas.Dni != null &&
                        c.Personas.Dni.ToLower().Contains(dniTrimLower)
                    );
                }
                else // solo nombresTrimLower no nulo
                {
                    query = query.Where(c =>
                        c.Personas != null &&
                        ((c.Personas.Nombres != null && c.Personas.Nombres.ToLower().Contains(nombresTrimLower)) ||
                         (c.Personas.Apellidos != null && c.Personas.Apellidos.ToLower().Contains(nombresTrimLower)))
                    );
                }

                var lista = query
                            .OrderBy(c => c.Personas.Apellidos)
                            .ThenBy(c => c.Personas.Nombres)
                            .ToList();

                if (lista == null || lista.Count == 0)
                    return Resultado<List<Clientes>>.Fail("No se encontraron clientes que coincidan con los criterios de búsqueda.");

                return Resultado<List<Clientes>>.Ok(lista);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener clientes por DNI/nombres (capa datos):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Clientes>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto <see cref="Clientes"/> a registrar (no puede ser nulo).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el Id del cliente registrado o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la entrada y la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet{T}"/>
        ///   relevantes mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Comprueba que la <c>Persona</c> y el <c>Estado</c> referenciados existan antes de insertar el cliente.
        /// - Asume que <c>Clientes</c> es autoincremental (deja <c>IdCliente</c> en null para que la BD lo genere).
        /// - No usa parámetros por referencia para mensajes; todos los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// - Registra detalles técnicos con <c>Logger</c> y devuelve mensajes amigables para la capa superior.
        /// </remarks>
        public static Resultado<int> RegistrarCliente(Clientes? cliente)
        {
            if (cliente == null)
                return Resultado<int>.Fail("El cliente no puede ser nulo.");

            // Validaciones básicas de integridad referencial en la capa de datos
            if (cliente.IdPersona <= 0)
                return Resultado<int>.Fail("El Id de la persona asociada no es válido.");

            if (cliente.IdEstado <= 0)
                return Resultado<int>.Fail("El Id del estado asociado no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet Clientes mediante ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rcClientes = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                if (!rcClientes.Success)
                {
                    Logger.LogError(rcClientes.Mensaje);
                    return Resultado<int>.Fail(rcClientes.Mensaje);
                }

                // Verificar que la Persona exista
                var rcPersonas = comprobacion.ComprobarEntidad(contexto.Personas, nameof(contexto.Personas));
                if (!rcPersonas.Success)
                {
                    Logger.LogError(rcPersonas.Mensaje);
                    return Resultado<int>.Fail(rcPersonas.Mensaje);
                }

                var personaExistente = contexto.Personas.Find(cliente.IdPersona);
                if (personaExistente == null)
                    return Resultado<int>.Fail($"No se encontró la persona con Id {cliente.IdPersona}.");

                // Verificar que el Estado exista (recordar que Estados puede no ser autoincremental)
                var rcEstados = comprobacion.ComprobarEntidad(contexto.Estados, nameof(contexto.Estados));
                if (!rcEstados.Success)
                {
                    Logger.LogError(rcEstados.Mensaje);
                    return Resultado<int>.Fail(rcEstados.Mensaje);
                }

                var estadoExistente = contexto.Estados.Find(cliente.IdEstado);
                if (estadoExistente == null)
                    return Resultado<int>.Fail($"No se encontró el estado con Id {cliente.IdEstado}.");

                // Preparar entidad para inserción (IdCliente es autoincremental)
                cliente.IdCliente = null;

                // Normalizaciones mínimas
                // (no hay campos string directos en Clientes salvo propiedades navegacionales)
                contexto.Clientes.Add(cliente);
                contexto.SaveChanges();

                if (cliente.IdCliente != null && cliente.IdCliente > 0)
                    return Resultado<int>.Ok(cliente.IdCliente.Value);

                var msg = "No se pudo obtener el Id del cliente registrado.";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
            catch (Exception ex)
            {
                var personaInfo = cliente?.IdPersona.ToString() ?? "<sin persona>";
                var msg = $"Error al registrar el cliente (IdPersona: {personaInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un cliente existente en la base de datos.
        /// </summary>
        /// <param name="cliente">Cliente con los cambios a aplicar (debe incluir IdCliente).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con <c>true</c> si la modificación fue exitosa, o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la entrada y la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Clientes}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Recupera la entidad existente y actualiza solo los campos escalares permitidos para evitar modificaciones
        ///   accidentales de relaciones de navegación.
        /// - Registra errores técnicos con <c>Logger</c> y devuelve mensajes amigables dentro de <see cref="Resultado{T}"/>.
        /// </remarks>
        public static Resultado<bool> ModificarCliente(Clientes? cliente)
        {
            if (cliente == null)
                return Resultado<bool>.Fail("No se recibió información del cliente.");

            if (cliente.IdCliente == null || cliente.IdCliente <= 0)
                return Resultado<bool>.Fail("El Id del cliente no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<bool>.Fail(rc.Mensaje);
                }

                // Recuperar la entidad existente para aplicar cambios controlados
                var id = cliente.IdCliente.Value;
                var existente = contexto.Clientes
                                        .FirstOrDefault(c => c.IdCliente == id);

                if (existente == null)
                    return Resultado<bool>.Fail($"No se encontró el cliente con Id {id}.");

                // Validaciones básicas de integridad referencial
                if (cliente.IdPersona <= 0)
                    return Resultado<bool>.Fail("El Id de la persona asociada no es válido.");

                if (cliente.IdEstado <= 0)
                    return Resultado<bool>.Fail("El Id del estado asociado no es válido.");

                // Actualizar solo campos escalares permitidos
                existente.IdPersona = cliente.IdPersona;
                existente.IdEstado = cliente.IdEstado;
                existente.esMiembro = cliente.esMiembro;

                // Guardar cambios
                var exito = contexto.SaveChanges();

                if (exito > 0)
                    return Resultado<bool>.Ok(true);

                var msg = "No se pudo modificar el cliente.";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
            catch (Exception ex)
            {
                var idInfo = cliente?.IdCliente?.ToString() ?? "<sin id>";
                var msg = $"Error al modificar cliente (Id: {idInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}

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
    /// Seguridad y robustez:
    /// - No exponer excepciones crudas; capturar excepciones, registrar detalles técnicos y devolver mensajes útiles
    ///   en <c>Resultado&lt;T&gt;</c>.
    /// - Validar entradas (nulls, longitudes máximas, rangos numéricos) antes de persistir.
    /// - Verificar la existencia de entidades referenciadas (Personas, Estados) antes de insertar o actualizar.
    /// - Recuperar entidades desde el contexto antes de eliminarlas o modificarlas para evitar problemas con entidades desconectadas.
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
                var cliente = contexto.Clientes.FirstOrDefault(c => c.IdPersona == idPersona);

                if (cliente == null)
                    return Resultado<Clientes?>.Ok(new Clientes(), $"No se encontró un cliente asociado a la persona con Id {idPersona}.");

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
        public static Resultado<List<Clientes>> GetClientes(bool incluirAnulados = true)
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
                                    .Include(c => c.Personas)
                                        .ThenInclude(p => p.Domicilios)
                                            .ThenInclude(d => d.Localidades)
                                                .ThenInclude(l => l.Provincias) // incluir provincia si es necesario para la capa superior
                                    .Where(c => c.Activo || c.Activo != incluirAnulados)
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
        public static Resultado<List<Clientes>> GetClientesPorDniNombres(string? dni, string? nombres, bool incluirAnulados)
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

                var lista = query.Where(c => c.Activo || c.Activo != incluirAnulados)
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

                // Preparar entidad para inserción (IdCliente es autoincremental)
                cliente.IdCliente = null;

                //Ponemos fecha de alta
                cliente.FechaAlta = DateTime.Now;

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

                // Actualizar solo campos escalares permitidos
                existente.IdPersona = cliente.IdPersona;
                existente.Activo = cliente.Activo;

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

        /// <summary>
        /// Registra masivamente un lote de nuevos clientes, asegurando la correcta vinculación de sus contactos.
        /// </summary>
        public static Resultado<int> RegistrarLoteMasivo(List<(Clientes cliente, Contactos? contacto)> nuevos)
        {
            if (!nuevos.Any()) return Resultado<int>.Ok(0);

            try
            {
                using (var contexto = new Contexto())
                {
                    foreach (var item in nuevos)
                    {
                        // 1. Agregamos el cliente (EF Core agregará automáticamente a la Persona y Domicilio anidados)
                        contexto.Clientes.Add(item.cliente);

                        // 2. Si tiene contacto, le asignamos LA REFERENCIA de la persona.
                        // Así, cuando EF Core genere el IdPersona, lo insertará correctamente en el Contacto.
                        if (item.contacto != null)
                        {
                            item.contacto.Personas = item.cliente.Personas;
                            contexto.Contactos.Add(item.contacto);
                        }
                    }

                    // Un solo viaje a la BD para insertar todo el lote
                    int cambios = contexto.SaveChanges();
                    return Resultado<int>.Ok(nuevos.Count, $"Se registraron {nuevos.Count} clientes nuevos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en RegistrarLoteMasivo:\n{ex.ToString()}");
                return Resultado<int>.Fail("Ocurrió un error técnico al registrar el lote de nuevos clientes.");
            }
        }

        /// <summary>
        /// Modifica masivamente un lote de clientes existentes, trayéndolos previamente de la BD para no romper referencias.
        /// </summary>
        public static Resultado<int> ModificarLoteMasivo(List<(Clientes cliente, Contactos? contacto)> modificados)
        {
            if (!modificados.Any()) return Resultado<int>.Ok(0);

            try
            {
                using (var contexto = new Contexto())
                {
                    // 1. Obtenemos todos los DNIs que vienen en el excel para este lote
                    var dnis = modificados.Select(m => m.cliente.Personas!.Dni).ToList();

                    // 2. Traemos todos esos clientes existentes de la BD en UN SOLO VIAJE
                    var clientesBD = contexto.Clientes
                        .Include(c => c.Personas)
                            .ThenInclude(p => p.Domicilios)
                        .Where(c => dnis.Contains(c.Personas!.Dni))
                        .ToList();

                    // 3. Traemos los contactos de esos clientes en UN SOLO VIAJE
                    var idsPersonasBD = clientesBD.Select(c => c.IdPersona).ToList();
                    var contactosBD = contexto.Contactos.Where(c => idsPersonasBD.Contains(c.IdPersona.Value)).ToList();

                    int actualizados = 0;

                    // 4. Cruzamos datos en memoria y actualizamos
                    foreach (var item in modificados)
                    {
                        var clienteExistente = clientesBD.FirstOrDefault(c => c.Personas!.Dni == item.cliente.Personas!.Dni);
                        if (clienteExistente == null) continue;

                        var personaBD = clienteExistente.Personas!;
                        var personaExcel = item.cliente.Personas!;

                        // Actualizamos campos de la Persona
                        personaBD.Nombres = personaExcel.Nombres;
                        personaBD.Apellidos = personaExcel.Apellidos;
                        if (personaExcel.FechaNac != null) personaBD.FechaNac = personaExcel.FechaNac;

                        // Actualizamos Domicilio
                        if (personaExcel.Domicilios != null)
                        {
                            if (personaBD.Domicilios == null)
                                personaBD.Domicilios = new Domicilios(); // Si no tenía, le creamos la instancia

                            personaBD.Domicilios.Calle = personaExcel.Domicilios.Calle;
                            personaBD.Domicilios.IdLocalidad = personaExcel.Domicilios.IdLocalidad;
                        }

                        // Actualizamos Contactos
                        if (item.contacto != null)
                        {
                            var contactoBD = contactosBD.FirstOrDefault(c => c.IdPersona == personaBD.IdPersona);
                            if (contactoBD == null)
                            {
                                // Si no tenía contacto, lo agregamos
                                item.contacto.IdPersona = personaBD.IdPersona;
                                contexto.Contactos.Add(item.contacto);
                            }
                            else
                            {
                                // Si ya tenía, lo pisamos con lo nuevo
                                contactoBD.Telefono = item.contacto.Telefono ?? contactoBD.Telefono;
                                contactoBD.Whatsapp = item.contacto.Whatsapp ?? contactoBD.Whatsapp;
                                contactoBD.Email = item.contacto.Email ?? contactoBD.Email;
                            }
                        }
                        actualizados++;
                    }

                    // Guardamos todos los updates/inserts generados en memoria de un solo golpe
                    contexto.SaveChanges();

                    return Resultado<int>.Ok(actualizados, $"Se actualizaron {actualizados} clientes exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error en ModificarLoteMasivo:\n{ex.ToString()}");
                return Resultado<int>.Fail("Ocurrió un error al actualizar el lote de clientes modificados.");
            }
        }

        public static Resultado<Clientes> GetClienteGenerico()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Clientes, nameof(contexto.Clientes));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Clientes>.Fail(rc.Mensaje);
                    }
                    var clienteGenerico = contexto.Clientes.Include(c => c.Personas).FirstOrDefault(c => c.Personas != null && c.Personas.Dni == "00000000");
                    if (clienteGenerico != null)
                        return Resultado<Clientes>.Ok(clienteGenerico);

                    Personas? personaGenerica = contexto.Personas.FirstOrDefault(p => p.Dni == "00000000");
                    
                    personaGenerica ??= new Personas
                    {
                        IdPersona = null,
                        Dni = "00000000",
                        Nombres = "Sin Cliente",
                        Apellidos = "",
                    };

                    clienteGenerico = new Clientes
                    {
                        IdCliente = null,
                        IdPersona = personaGenerica?.IdPersona ?? 0, // Se asignará el IdPersona generado al insertar la persona
                        Activo = true,
                        FechaAlta = DateTime.Now,
                        Personas = personaGenerica?.IdPersona != null ? null : personaGenerica // Solo asignamos la referencia si la persona no existía
                    };

                    contexto.Add(clienteGenerico);
                    contexto.SaveChanges();
                    if (clienteGenerico.IdCliente == null || clienteGenerico.IdCliente < 1)
                        return Resultado<Clientes>.Fail("No se pudo registrar el cliente genérico.");

                    clienteGenerico.Personas = personaGenerica; // Asignamos la referencia a la persona genérica para devolverla completa
                    return Resultado<Clientes>.Ok(clienteGenerico);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return Resultado<Clientes>.Fail("Error al obtener cliente genérico");
            }
        }
    }
}

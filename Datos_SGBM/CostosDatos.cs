using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad CostosServicios.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de acceso y persistencia relacionadas con los costos/insumos de servicios
    ///   (listar por servicio, registrar, modificar, eliminar físicamente, etc.).
    /// - Incluir relaciones de lectura necesarias (por ejemplo <c>Productos</c>) únicamente cuando el método
    ///   lo requiera para evitar consultas N+1 y cargas innecesarias.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{CostosServicios}"/>
    ///   mediante la clase <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar datos y mensajes de error.
    ///
    /// Buenas prácticas y diseño:
    /// - Mantener la clase enfocada en acceso a datos; las reglas de negocio, validaciones complejas y políticas
    ///   de autorización deben residir en la capa de negocio.
    /// - Al actualizar, recuperar la entidad existente y asignar únicamente los campos escalares permitidos
    ///   para evitar inserciones o modificaciones accidentales de relaciones de navegación.
    /// - Normalizar cadenas mínimas (por ejemplo <c>Trim()</c>) antes de persistir o comparar.
    /// - Registrar errores técnicos con <c>Logger</c> y devolver mensajes amigables dentro de <c>Resultado&lt;T&gt;</c>.
    /// - No usar parámetros por referencia para mensajes; devolver siempre el mensaje dentro del <see cref="Resultado{T}"/>.
    ///
    /// Métodos esperados y su comportamiento:
    /// - <c>ObtenerCostosPorIdServicio(int id)</c>: devuelve lista de <see cref="CostosServicios"/> por IdServicio,
    ///   incluyendo <c>Productos</c> cuando sea necesario, ordenada por <c>Descripcion</c>.
    /// - <c>RegistrarCosto(CostosServicios)</c>: inserta un nuevo costo; si la tabla no tiene autoincremental,
    ///   usar el helper genérico para obtener el Id antes de insertar; devolver el Id generado en <c>Resultado&lt;int&gt;</c>.
    /// - <c>ModificarCosto(CostosServicios)</c>: recuperar la entidad existente y actualizar solo campos escalares.
    /// - <c>EliminarFisico(CostosServicios)</c>: eliminar físicamente el registro tras verificar su existencia.
    /// - Métodos auxiliares para búsquedas y proyecciones (DTOs) según necesidades de la UI o reportes.
    ///
    /// </summary>
    public class CostosDatos
    {
        /// <summary>
        /// Obtiene la lista de costos asociados a un servicio dado.
        /// </summary>
        /// <param name="id">Id del servicio cuyos costos se desean obtener.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="CostosServicios"/> ordenada por <c>Descripcion</c>,
        /// o un <see cref="Resultado{T}"/> con el mensaje de error en caso de fallo.
        /// </returns>
        /// <remarks>
        /// - Valida la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{CostosServicios}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/> antes de ejecutar la consulta.
        /// - Incluye la navegación a <c>Productos</c> para evitar consultas N+1 cuando la capa superior necesite esa relación.
        /// - No usa parámetros por referencia para mensajes; los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// - Si la política de negocio requiere devolver una lista vacía en lugar de un error cuando no hay resultados,
        ///   ajustar la lógica en la capa de negocio o aquí según corresponda.
        /// </remarks>
        public static Resultado<List<CostosServicios>> ObtenerCostosPorIdServicio(int id)
        {
            if (id <= 0)
                return Resultado<List<CostosServicios>>.Fail("El Id de servicio no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.CostosServicios, nameof(contexto.CostosServicios));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<CostosServicios>>.Fail(rc.Mensaje);
                }

                // Consulta: incluir Productos y filtrar por IdServicio
                var lista = contexto.CostosServicios
                                    .Where(c => c.IdServicio == id)
                                    .Include(c => c.Productos)
                                    .OrderBy(c => c.Descripcion)
                                    .ToList();

                if (lista == null || lista.Count == 0)
                    return Resultado<List<CostosServicios>>.Fail($"No se encontraron costos para el servicio con Id {id}.");

                return Resultado<List<CostosServicios>>.Ok(lista);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener insumos - costos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<CostosServicios>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo costo/insumo asociado a un servicio.
        /// </summary>
        /// <param name="costo">Entidad <see cref="CostosServicios"/> a registrar (no debe ser nula).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el Id generado del costo en caso de éxito,
        /// o con el mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{CostosServicios}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/> antes de intentar persistir.
        /// - No se usan parámetros por referencia para mensajes; los mensajes de error se devuelven dentro de <see cref="Resultado{T}"/>.
        /// - Se registran errores técnicos con <c>Logger</c> y se devuelve un mensaje amigable en el resultado.
        /// </remarks>
        public static Resultado<int> RegistrarCosto(CostosServicios? costo)
        {
            if (costo == null)
                return Resultado<int>.Fail("El costo no puede ser nulo.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.CostosServicios, nameof(contexto.CostosServicios));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<int>.Fail(rc.Mensaje);
                }

                // Normalizaciones mínimas
                costo.Descripcion = costo.Descripcion?.Trim();

                // Marcar como nuevo
                costo.IdCostoServicio = null;

                contexto.Add(costo);
                contexto.SaveChanges();

                if (costo.IdCostoServicio != null && costo.IdCostoServicio > 0)
                    return Resultado<int>.Ok(costo.IdCostoServicio.Value);

                var msg = "No se pudo obtener el Id del costo-insumo registrado.";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
            catch (Exception ex)
            {
                var descripcion = costo?.Descripcion ?? "<sin descripción>";
                var msg = $"Error al registrar costo-insumo {descripcion}:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un costo/insumo existente.
        /// </summary>
        /// <param name="costo">Entidad <see cref="CostosServicios"/> con los cambios a aplicar (debe incluir IdCostoServicio).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con <c>true</c> si la modificación fue exitosa, o <c>false</c> con mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la entrada y la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{CostosServicios}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Recupera la entidad existente y actualiza solo los campos escalares necesarios para evitar modificaciones
        ///   accidentales de relaciones de navegación o inserciones no deseadas.
        /// - Registra errores técnicos con <c>Logger</c> y devuelve mensajes amigables dentro de <see cref="Resultado{T}"/>.
        /// - Si la política de la aplicación lo permite, se puede cambiar a un enfoque de "attach + Entry(...).State = Modified"
        ///   cuando se controle explícitamente qué propiedades se actualizan.
        /// </remarks>
        public static Resultado<bool> ModificarCosto(CostosServicios? costo)
        {
            if (costo == null)
                return Resultado<bool>.Fail("El costo no puede ser nulo.");

            if (costo.IdCostoServicio == null || costo.IdCostoServicio <= 0)
                return Resultado<bool>.Fail("El Id del costo no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.CostosServicios, nameof(contexto.CostosServicios));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<bool>.Fail(rc.Mensaje);
                }

                // Recuperar la entidad existente para aplicar cambios controlados
                var id = costo.IdCostoServicio.Value;
                var existente = contexto.CostosServicios
                                        .FirstOrDefault(c => c.IdCostoServicio == id);

                if (existente == null)
                    return Resultado<bool>.Fail($"No se encontró el costo con Id {id}.");

                // Actualizar solo campos escalares permitidos (ejemplo)
                existente.Costo = costo.Costo;
                existente.Unidades = costo.Unidades;
                existente.CantidadMedida = costo.CantidadMedida;
                existente.IdProducto = costo.IdProducto;
                existente.IdServicio = costo.IdServicio;
                existente.Descripcion = costo.Descripcion?.Trim();

                // Guardar cambios
                int exito = contexto.SaveChanges();

                if (exito > 0)
                    return Resultado<bool>.Ok(true);

                var msg = "No se pudo modificar el costo-insumo.";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
            catch (Exception ex)
            {
                var idInfo = costo?.IdCostoServicio?.ToString() ?? "<sin id>";
                var msg = $"Error al modificar costo-insumo (Id: {idInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina físicamente un registro de <see cref="CostosServicios"/>.
        /// </summary>
        /// <param name="costo">Entidad <see cref="CostosServicios"/> a eliminar (debe contener IdCostoServicio).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con <c>true</c> si la eliminación fue exitosa, o <c>false</c> con mensaje de error en caso contrario.
        /// </returns>
        /// <remarks>
        /// - Valida la entrada y la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{CostosServicios}"/>
        ///   mediante la clase <see cref="ComprobacionContexto"/>.
        /// - Recupera la entidad existente desde la base de datos y la elimina para evitar problemas con entidades desconectadas.
        /// - Registra errores técnicos con <c>Logger</c> y devuelve mensajes amigables dentro de <see cref="Resultado{T}"/>.
        /// - Si la política de la aplicación prefiere eliminaciones lógicas, implementar un método separado que marque el registro como inactivo.
        /// </remarks>
        public static Resultado<bool> EliminarFisico(CostosServicios? costo)
        {
            if (costo == null)
                return Resultado<bool>.Fail("No se recibió información del costo a eliminar.");

            if (costo.IdCostoServicio == null || costo.IdCostoServicio <= 0)
                return Resultado<bool>.Fail("El Id del costo no es válido.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.CostosServicios, nameof(contexto.CostosServicios));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<bool>.Fail(rc.Mensaje);
                }

                // Recuperar la entidad existente desde la BD para evitar problemas con entidades desconectadas
                var id = costo.IdCostoServicio.Value;
                var existente = contexto.CostosServicios.FirstOrDefault(c => c.IdCostoServicio == id);

                if (existente == null)
                    return Resultado<bool>.Fail($"No se encontró el costo-insumo con Id {id}.");

                contexto.CostosServicios.Remove(existente);
                int exito = contexto.SaveChanges();

                if (exito > 0)
                    return Resultado<bool>.Ok(true);

                var msg = "No se pudo eliminar el costo-insumo.";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
            catch (Exception ex)
            {
                var idInfo = costo?.IdCostoServicio?.ToString() ?? "<sin id>";
                var msg = $"Error al eliminar costo-insumo (Id: {idInfo}):\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}

using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Estados.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de acceso y persistencia relacionadas con estados (listar, buscar por índole,
    ///   obtener por combinación índole/descripcion, registrar, etc.).
    /// - Validar la disponibilidad del <see cref="Contexto"/> y del <see cref="DbSet{Estados}"/> mediante
    ///   <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar datos y mensajes de error.
    ///
    /// Buenas prácticas y diseño:
    /// - Mantener la clase enfocada en acceso a datos; las reglas de negocio, validaciones complejas y políticas
    ///   de autorización deben residir en la capa de negocio.
    /// - Incluir relaciones solo cuando el método lo requiera para evitar cargas innecesarias y consultas N+1.
    /// - Normalizar y validar entradas en la capa de negocio; en la capa de datos aplicar validaciones técnicas
    ///   (nulls, disponibilidad del DbSet) y registrar errores con <c>Logger</c>.
    /// - Evitar pasar mensajes por referencia; devolver mensajes de error dentro de <c>Resultado&lt;T&gt;</c>.
    ///
    /// Consideraciones operativas:
    /// - Para entidades sin autoincremental, si se necesita generar claves en la aplicación, usar un helper
    ///   seguro que obtenga el siguiente Id dentro de una transacción para evitar condiciones de carrera.
    /// - Implementar paginación en consultas que puedan devolver muchos registros (Skip/Take).
    /// - Capturar excepciones, registrar detalles técnicos y devolver mensajes amigables; no propagar excepciones crudas.
    ///
    /// Métodos esperados:
    /// - <c>GetEstadosPorIndole(string)</c>: devuelve lista de estados por índole.
    /// - <c>GetEstado(string indole, string descripcion)</c>: devuelve un estado por índole y descripción.
    /// - <c>RegistrarEstado(Estados)</c>: inserta un nuevo estado (puede usar helper para generar Id si la BD no lo hace).
    /// - Otros métodos de consulta y mantenimiento según necesidades del dominio.
    ///
    /// Extensibilidad:
    /// - Centralizar la lógica de filtrado y normalización en helpers reutilizables (por ejemplo, filtros y validaciones).
    /// - Añadir pruebas unitarias e integración para cada método público.
    /// </summary>
    public class EstadosDatos
    {
        #region Consultas y listados

        /// <summary>
        /// Obtiene un estado a partir de su índole y descripción.
        /// </summary>
        /// <param name="indole">Texto de índole del estado (no nulo ni vacío).</param>
        /// <param name="descripcion">Descripción del estado (no nula ni vacía).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con el objeto <see cref="Estados"/> encontrado o mensaje de error.
        /// </returns>
        /// <remarks>
        /// - Valida parámetros en la capa de datos y usa <see cref="ComprobacionContexto"/> para verificar el DbSet.
        /// - Normaliza los valores de búsqueda con Trim() y ToLowerInvariant() para búsquedas case-insensitive.
        /// - Evita lanzar excepciones hacia capas superiores: registra detalles técnicos con <c>Logger</c> y devuelve mensajes amigables.
        /// - Si se requiere una política distinta (por ejemplo, usar collations SQL case-insensitive o EF.Functions.Like),
        ///   mover esa decisión a la capa de negocio o ajustar según la configuración de la base de datos.
        /// </remarks>
        public static Resultado<Estados?> GetEstado(string? indole, string? descripcion)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(indole) || string.IsNullOrWhiteSpace(descripcion))
                return Resultado<Estados?>.Fail("No llegan los datos de búsqueda de estados.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Estados, nameof(contexto.Estados));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<Estados?>.Fail(rc.Mensaje);
                }

                // Normalizar criterios de búsqueda
                var indoleTrimLower = indole.Trim().ToLower();
                var descripcionTrimLower = descripcion.Trim().ToLower();

                // Búsqueda segura frente a nulls en las columnas
                var estado = contexto.Estados
                                     .FirstOrDefault(e =>
                                         e.Indole != null && e.Estado != null &&
                                         e.Indole.ToLower() == indoleTrimLower &&
                                         e.Estado.ToLower() == descripcionTrimLower);

                if (estado == null)
                    return Resultado<Estados?>.Fail($"No se encontró un estado con índole '{indoleTrimLower}' y descripción '{descripcionTrimLower}'.");

                return Resultado<Estados?>.Ok(estado);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener el estado:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Estados?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene una lista de estados filtrados por índole.
        /// </summary>
        /// <param name="indole">Texto de índole para filtrar (no nulo ni vacío).</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> con la lista de <see cref="Estados"/> encontrados o mensaje de error.
        /// </returns>
        public static Resultado<List<Estados>> GetEstadosPorIndole(string? indole)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(indole))
                return Resultado<List<Estados>>.Fail("No llega la información de índole a la capa de datos.");

            try
            {
                using var contexto = new Contexto();

                // Validar disponibilidad del DbSet mediante la clase ComprobacionContexto
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Estados, nameof(contexto.Estados));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<Estados>>.Fail(rc.Mensaje);
                }

                // Normalizar el valor de búsqueda
                var indoleTrim = indole.Trim().ToLower();

                // Búsqueda case-insensitive y segura frente a nulls en la columna Indole
                var estados = contexto.Estados
                                      .Where(e => e.Indole != null && e.Indole.ToLower() == indoleTrim)
                                      .ToList();

                if (estados == null || estados.Count < 1)
                    return Resultado<List<Estados>>.Fail($"No se encontraron estados con índole '{indoleTrim}'.");

                return Resultado<List<Estados>>.Ok(estados);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener estados por índole:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Estados>>.Fail(msg);
            }
        }

        #endregion

        /// <summary>
        /// Registra un nuevo estado en la base de datos.
        /// </summary>
        /// <param name="estado">Objeto Estado a registrar.</param>
        /// <returns>
        /// Resultado con el Id del estado registrado o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarEstado(Estados? estado)
        {
            if (estado == null)
                return Resultado<int>.Fail("El estado no puede ser nulo.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }


                    // Iniciar transacción para que la obtención del Id y la inserción sean atómicas
                    using var tx = contexto.Database.BeginTransaction();
                    try
                    {
                        estado.IdEstado = IdentificadorHelper.ObtenerSiguienteIdSeguro<Estados>(contexto);

                        contexto.Estados.Add(estado);
                        contexto.SaveChanges();

                        tx.Commit();
                        return Resultado<int>.Ok(estado.IdEstado);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        Logger.LogError(ex.ToString());
                        return Resultado<int>.Fail($"Error al registrar el estado:\n{ex.Message}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return Resultado<int>.Fail($"Error al registrar el estado:\n{ex.Message}");
            }
        }

    }
}

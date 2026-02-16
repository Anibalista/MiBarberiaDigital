using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    public class EstadosDatos
    {

        /// <summary>
        /// Comprueba que el contexto y la entidad Estados estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <returns>
        /// Resultado indicando éxito o fallo en la comprobación.
        /// </returns>
        public static Resultado<bool> ComprobarContexto(Contexto contexto)
        {
            if (contexto == null)
                return Resultado<bool>.Fail("No se conecta a la BD.");

            if (contexto.Estados == null)
                return Resultado<bool>.Fail("No se conecta a la BD (Estados).");

            return Resultado<bool>.Ok(true);
        }

        /// <summary>
        /// Obtiene un estado a partir de su índole y descripción.
        /// </summary>
        /// <param name="indole">Texto de índole del estado.</param>
        /// <param name="descripcion">Descripción del estado.</param>
        /// <returns>
        /// Resultado con el objeto Estado encontrado o mensaje de error.
        /// </returns>
        public static Resultado<Estados?> GetEstado(string? indole, string? descripcion)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(indole) || string.IsNullOrWhiteSpace(descripcion))
                return Resultado<Estados?>.Fail("No llegan los datos de búsqueda de estados.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<Estados?>.Fail(resultadoContexto.Mensaje);

                    var estado = contexto.Estados
                        .FirstOrDefault(e => e.Indole.Equals(indole) && e.Estado.Equals(descripcion));

                    if (estado == null)
                        return Resultado<Estados?>.Fail($"No se encontró un estado con índole '{indole}' y descripción '{descripcion}'.");

                    return Resultado<Estados?>.Ok(estado);
                }
            }
            catch (Exception ex)
            {
                return Resultado<Estados?>.Fail($"Error al obtener el estado:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Obtiene una lista de estados filtrados por índole.
        /// </summary>
        /// <param name="indole">Texto de índole para filtrar.</param>
        /// <returns>
        /// Resultado con la lista de estados encontrados o mensaje de error.
        /// </returns>
        public static Resultado<List<Estados>> GetEstadosPorIndole(string? indole)
        {
            // Validación previa al uso del contexto
            if (string.IsNullOrWhiteSpace(indole))
                return Resultado<List<Estados>>.Fail("No llega la información de índole a la capa de datos.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Estados>>.Fail(resultadoContexto.Mensaje);

                    var estados = contexto.Estados
                        .Where(e => e.Indole.Equals(indole))
                        .ToList();

                    if (estados == null || estados.Count < 1)
                        return Resultado<List<Estados>>.Fail($"No se encontraron estados con índole '{indole}'.");

                    return Resultado<List<Estados>>.Ok(estados);
                }
            }
            catch (Exception ex)
            {
                return Resultado<List<Estados>>.Fail($"Error al obtener estados por índole:\n{ex.ToString()}");
            }
        }

        //Registros
        /// <summary>
        /// Registra un nuevo estado en la base de datos.
        /// </summary>
        /// <param name="estado">Objeto Estado a registrar.</param>
        /// <returns>
        /// Resultado con el Id del estado registrado o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarEstado(Estados? estado)
        {
            // Validación previa al uso del contexto
            if (estado == null)
                return Resultado<int>.Fail("El estado no puede ser nulo.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<int>.Fail(resultadoContexto.Mensaje);

                    // Estados es entidad sin autoincremental → IdEstado = 0 en registros nuevos
                    estado.IdEstado = 0;

                    contexto.Estados.Add(estado);
                    int exito = contexto.SaveChanges();

                    if (exito > 0 && estado.IdEstado > 0)
                        return Resultado<int>.Ok(estado.IdEstado);

                    return Resultado<int>.Fail("Problemas desconocidos en el registro de estados.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail($"Error al registrar el estado:\n{ex.ToString()}");
            }
        }
    }
}

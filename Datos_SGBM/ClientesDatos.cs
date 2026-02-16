using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Clientes.
    /// Contiene métodos de comprobación de contexto y operaciones CRUD.
    /// </summary>
    public class ClientesDatos
    {
        /// <summary>
        /// Comprueba que las entidades necesarias del contexto estén disponibles.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <param name="mensaje">Mensaje de error si la validación falla.</param>
        /// <returns>True si todas las entidades están disponibles, False en caso contrario.</returns>
        public static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);

            // Verifica la entidad Estados
            if (!comprobar.ComprobarEntidad(contexto.Estados, ref mensaje))
                return false;

            // Verifica la entidad Clientes
            if (!comprobar.ComprobarEntidad(contexto.Clientes, ref mensaje))
                return false;

            // Verifica la entidad Personas
            return comprobar.ComprobarEntidad(contexto.Personas, ref mensaje);
        }

        /// <summary>
        /// Obtiene un cliente a partir del ID de persona.
        /// </summary>
        /// <param name="idPersona">ID de la persona asociada al cliente.</param>
        /// <param name="mensaje">Mensaje de error si la consulta falla.</param>
        /// <returns>Cliente encontrado o null si no existe.</returns>
        public static Clientes? GetClientePorIdPersona(int idPersona, ref string mensaje)
        {
            // Valida que el ID sea mayor a cero
            if (idPersona < 1)
            {
                mensaje = "El ID de la persona no llega a la consulta";
                return null;
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprueba que el contexto esté correcto
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Incluye relaciones: Personas -> Domicilios -> Localidades
                    return contexto.Clientes.Include(c => c.Personas)
                                            .ThenInclude(p => p.Domicilios)
                                            .ThenInclude(d => d.Localidades)
                                            .FirstOrDefault(c => c.IdPersona == idPersona);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = "Error en la búsqueda de clientes (capa datos)";
                return null;
            }
        }

        /// <summary>
        /// Obtiene todos los clientes ordenados por apellido y nombre.
        /// </summary>
        /// <param name="mensaje">Mensaje de error si la consulta falla.</param>
        /// <returns>Lista de clientes o null si ocurre un error.</returns>
        public static List<Clientes>? GetClientes(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Incluye Estados, Personas y sus relaciones
                    return contexto.Clientes.Include(c => c.Estados)
                                            .Include(c => c.Personas)
                                                .ThenInclude(p => p.Domicilios)
                                                    .ThenInclude(d => d.Localidades)
                                            .OrderBy(c => c.Personas.Apellidos)
                                            .ThenBy(c => c.Personas.Nombres)
                                            .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = ex.Message + "(Capa Datos)";
                return null;
            }
        }

        /// <summary>
        /// Obtiene clientes filtrados por DNI y/o nombres.
        /// </summary>
        /// <param name="dni">DNI del cliente.</param>
        /// <param name="nombres">Nombres o apellidos del cliente.</param>
        /// <param name="mensaje">Mensaje de error si la consulta falla.</param>
        /// <returns>Lista de clientes filtrados o null si ocurre un error.</returns>
        public static List<Clientes>? GetClientesPorDniNombres(string? dni, string? nombres, ref string mensaje)
        {
            // Valida que al menos un dato de búsqueda llegue
            if (String.IsNullOrWhiteSpace(dni) && String.IsNullOrWhiteSpace(nombres))
            {
                mensaje = "No llegan los datos de búsqueda a la consulta";
                return null;
            }

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Filtra clientes por coincidencia en DNI y nombres/apellidos
                    return contexto.Clientes.Include(c => c.Estados)
                                            .Include(c => c.Personas)
                                                .ThenInclude(p => p.Domicilios)
                                                    .ThenInclude(d => d.Localidades)
                                            .Where(c => c.Personas != null &&
                                                        (c.Personas.Dni.Contains(dni ?? "") &&
                                                         (c.Personas.Nombres.Contains(nombres ?? "") ||
                                                          c.Personas.Apellidos.Contains(nombres ?? ""))))
                                            .OrderBy(c => c.Personas.Apellidos)
                                            .ThenBy(c => c.Personas.Nombres)
                                            .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = ex.Message + " (Capa Datos)";
                return null;
            }
        }

        /// <summary>
        /// Registra un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto Cliente a registrar.</param>
        /// <returns>
        /// Resultado con el Id del cliente registrado o mensaje de error.
        /// </returns>
        public static Resultado<int> RegistrarCliente(Clientes? cliente)
        {
            // Validación previa al uso del contexto
            if (cliente == null)
                return Resultado<int>.Fail("El cliente no puede ser nulo.");

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    var resultadoContexto = ComprobarContexto(contexto);
                    if (!resultadoContexto.Success)
                        return Resultado<int>.Fail(resultadoContexto.Mensaje);

                    // En Clientes seguimos con autoincremental → Id en null
                    cliente.IdCliente = null;

                    contexto.Clientes.Add(cliente);
                    contexto.SaveChanges();

                    if (cliente.IdCliente != null && cliente.IdCliente > 0)
                        return Resultado<int>.Ok((int)cliente.IdCliente);
                    else
                        return Resultado<int>.Fail("No se pudo obtener el Id del cliente registrado.");
                }
            }
            catch (Exception ex)
            {
                return Resultado<int>.Fail($"Error al registrar el cliente:\n{ex.ToString()}");
            }
        }

        /// <summary>
        /// Modifica un cliente existente en la base de datos.
        /// </summary>
        /// <param name="cliente">Cliente a modificar.</param>
        /// <param name="mensaje">Mensaje de error si la operación falla.</param>
        /// <returns>True si la modificación fue exitosa, False en caso contrario.</returns>
        public static bool ModificarCliente(Clientes cliente, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.Update(cliente);
                    int exito = contexto.SaveChanges();

                    // Retorna true si se modificó al menos un registro
                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = ex.Message + " (Capa Datos)";
                return false;
            }
        }
    }
}

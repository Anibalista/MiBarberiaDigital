using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    public class EmpleadosDatos
    {
       
        /// <summary>
        /// Comprueba que el contexto y sus entidades estén disponibles y correctas.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si todas las entidades están disponibles, False en caso contrario.</returns>
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);

            // Verifica que la entidad Estados esté disponible
            if (!comprobar.ComprobarEntidad(contexto.Estados, ref mensaje))
                return false;

            // Verifica que la entidad Personas esté disponible
            if (!comprobar.ComprobarEntidad(contexto.Personas, ref mensaje))
                return false;

            // Verifica que la entidad Empleados esté disponible
            return comprobar.ComprobarEntidad(contexto.Empleados, ref mensaje);
        }

        /// <summary>
        /// Obtiene un Empleado a partir del ID de persona.
        /// </summary>
        /// <param name="idPersona">ID de la persona asociada al Empleado.</param>
        /// <param name="mensaje">Mensaje de error si la consulta falla.</param>
        /// <param name="completo">Bandera para traer todos los datos o solo el cliente (Por defecto false)</param>
        /// <returns>Empleado encontrado o null si no existe.</returns>
        public static Empleados? GetEmpleadoPorIdPersona(int idPersona, ref string mensaje, bool completo = false)
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

                    //Si se piden todos los datos con "completo" se incluyen las entidades
                    if (completo)
                    {
                        // Incluye relaciones: Personas -> Domicilios -> Localidades
                        return contexto.Empleados.Include(e => e.Estados).Include(e => e.Personas)
                                                .ThenInclude(p => p.Domicilios)
                                                    .ThenInclude(d => d.Localidades)
                                                .FirstOrDefault(e => e.IdPersona == idPersona);
                    } else
                        return contexto.Empleados.Include(e => e.Estados).FirstOrDefault(e => e.IdPersona == idPersona);
                    
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = "Error en la búsqueda de Empleados (capa datos)";
                return null;
            }
        }

        /// <summary>
        /// Obtiene todos los Empleados ordenados por apellido y nombre.
        /// </summary>
        /// <param name="mensaje">Mensaje de error si la consulta falla.</param>
        /// <returns>Lista de Empleados o null si ocurre un error.</returns>
        public static List<Empleados>? GetEmpleados(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Incluye Estados, Personas y sus relaciones
                    return contexto.Empleados.Include(e => e.Estados)
                                            .Include(e => e.Personas)
                                                .ThenInclude(p => p.Domicilios)
                                                    .ThenInclude(d => d.Localidades)
                                            .OrderBy(e => e.Personas.Apellidos)
                                            .ThenBy(e => e.Personas.Nombres)
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
        /// Registra un nuevo Empleado en la base de datos.
        /// </summary>
        /// <param name="Empleado">Empleado a registrar.</param>
        /// <param name="mensaje">Mensaje de error si la operación falla.</param>
        /// <returns>ID del Empleado registrado o -1 si ocurre un error.</returns>
        public static int RegistrarEmpleado(Empleados Empleado, ref string mensaje)
        {
            Empleado.IdEmpleado = null; // Reinicia el ID antes de registrar

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return -1;
                    Empleado.FechaIngreso = DateTime.Now;
                    contexto.Add(Empleado);
                    contexto.SaveChanges();

                    // Retorna el ID generado
                    return Empleado.IdEmpleado ?? 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                mensaje = ex.Message + " (Capa Datos)";
                return -1;
            }
        }

        /// <summary>
        /// Modifica un Empleado existente en la base de datos.
        /// </summary>
        /// <param name="Empleado">Empleado a modificar.</param>
        /// <param name="mensaje">Mensaje de error si la operación falla.</param>
        /// <returns>True si la modificación fue exitosa, False en caso contrario.</returns>
        public static bool ModificarEmpleado(Empleados Empleado, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.Update(Empleado);
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

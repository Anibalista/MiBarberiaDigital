using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;

namespace Datos_SGBM
{
    public class ServiciosDatos
    {
        static bool ComprobarContexto(Contexto contexto, ref string mensaje, bool insumos = false)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.Comprobar(ref mensaje))
                return false;
            if (!comprobar.ComprobarServicios(ref mensaje))
                return false;
            if (insumos)
            {
                if (!comprobar.ComprobarServiciosInsumos(ref mensaje))
                    return false;
                if (!comprobar.ComprobarProductos(ref mensaje))
                    return false;
            }
            return true;
        }

        public static List<Servicios>? ListaServicios (ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Servicios
                                   .OrderBy(s => s.NombreServicio)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener servicios:\n{ex.Message}";
                return null;
            }
        }

        public static int RegistrarServicio(Servicios? servicio, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return 0;
                    servicio.IdServicio = null;
                    contexto.Servicios.Add(servicio);
                    contexto.SaveChanges();
                    if (servicio.IdServicio != null)
                        return (int)servicio.IdServicio;
                    else
                    {
                        mensaje = "No se pudo obtener el Id del servicio registrado.";
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar servicio:\n{ex.Message}";
                return 0;
            }
        }

        public static bool ModificarServicio(Servicios? servicio, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.Servicios.Update(servicio);
                    contexto.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar servicio:\n{ex.Message}";
                return false;
            }
        }

        public static Servicios? ObtenerServicioPorId(int idServicio, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    Servicios? servicio = contexto.Servicios
                                                  .FirstOrDefault(s => s.IdServicio == idServicio);
                    if (servicio == null)
                    {
                        mensaje = "No se encontró el servicio con el Id proporcionado.";
                        return null;
                    }
                    return servicio;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener servicio por Id:\n{ex.Message}";
                return null;
            }
        }

        public static Servicios? ObtenerServicioPorNombre(string nombreServicio, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    Servicios? servicio = contexto.Servicios
                                                  .FirstOrDefault(s => string.Equals(s.NombreServicio, nombreServicio, StringComparison.OrdinalIgnoreCase));
                    if (servicio == null)
                    {
                        mensaje = "No se encontró el servicio con el nombre proporcionado.";
                        return null;
                    }
                    return servicio;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener servicio por nombre:\n{ex.Message}";
                return null;
            }
        }

        public static List<CostosServicios>? ObtenerInsumosPorIdServicio(int id, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje, true))
                        return null;

                    return contexto.ServiciosInsumos.Where(s => s.IdServicio == id)
                                   .Include(s => s.Productos)
                                   .OrderBy(s => s.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener insumos - costos:\n{ex.Message}";
                return null;
            }
        }
    }
}

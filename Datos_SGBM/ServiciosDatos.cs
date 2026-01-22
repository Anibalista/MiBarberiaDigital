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
            return comprobar.ComprobarEntidad(contexto.Servicios, ref mensaje);
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
                    int exito = contexto.SaveChanges();
                    mensaje = exito > 0 ? "" : "No se pudo modificar el servicio";
                    return exito > 0;
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

        public static List<Servicios>? BuscarAvanzado(string campo, string criterio, string valor, int idCategoria, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    string campoLower = campo.ToLower();

                    // Caso especial: Nombre Costo -> partimos de CostosServicios
                    if (campoLower.Contains("nombre costo"))
                    {
                        // IDs de servicios que tienen al menos un costo que contiene el texto
                        var idsExcluir = contexto.CostosServicios
                                                 .Where(c => c.Descripcion != null && c.Descripcion.Contains(valor))
                                                 .Select(c => c.IdServicio)
                                                 .Distinct()
                                                 .ToList();

                        var queryCostos = contexto.CostosServicios
                                                  .Include(c => c.Servicios)
                                                  .Where(c => c.Servicios != null);

                        // Filtro por categoría si corresponde
                        if (idCategoria > 0)
                            queryCostos = queryCostos.Where(c => c.Servicios!.IdCategoria == idCategoria);

                        if (criterio.ToLower() == "no contiene")
                        {
                            // Excluir servicios que tengan algún costo con el texto
                            return contexto.Servicios
                                           .Where(s => !idsExcluir.Contains(s.IdServicio!.Value))
                                           .OrderBy(s => s.NombreServicio)
                                           .ToList();
                        }
                        else
                        {
                            // Para los demás criterios, seguimos filtrando directamente sobre CostosServicios
                            queryCostos = FiltrosHelper.AplicarFiltroTexto(queryCostos, c => c.Descripcion!, criterio, valor);

                            return queryCostos.Select(c => c.Servicios!)
                                              .Distinct()
                                              .OrderBy(s => s.NombreServicio)
                                              .ToList();
                        }
                    }
                    else
                    {
                        IQueryable<Servicios> query = contexto.Servicios.Include(s => s.Categorias);

                        // Filtro por categoría si corresponde
                        if (idCategoria > 0)
                            query = query.Where(s => s.IdCategoria == idCategoria);

                        // Filtros por campo
                        if (campoLower.Contains("descrip"))
                        {
                            query = FiltrosHelper.AplicarFiltroTexto(query, s => s.Descripcion!, criterio, valor);
                        }
                        else if (campoLower.Contains("precio"))
                        {
                            if (decimal.TryParse(valor, out var numero))
                                query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.PrecioVenta, criterio, numero);
                        }
                        else if (campoLower.Contains("puntaje"))
                        {
                            if (decimal.TryParse(valor, out var numero))
                                query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.Puntaje, criterio, numero);
                        }
                        else if (campoLower.Contains("duracion"))
                        {
                            if (decimal.TryParse(valor, out var numero))
                                query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.DuracionMinutos, criterio, numero);
                        }
                        else if (campoLower.Contains("costo total"))
                        {
                            if (decimal.TryParse(valor, out var numero))
                                query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.Costos, criterio, numero);
                        }

                        return query.OrderBy(s => s.NombreServicio).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error en búsqueda avanzada:\n{ex.Message}";
                return null;
            }
        }

    }
}

using Entidades_SGBM;
using Datos_SGBM;

namespace Negocio_SGBM
{
    public class ServiciosNegocio
    {
        static bool ComprobarServicio(Servicios? servicio, bool registro, ref string mensaje)
        {
            if (servicio == null)
            {
                mensaje = "La información del servicio no llega a la consulta";
                return false;
            }
            if (servicio.PrecioVenta < 0)
            {
                mensaje = "El precio del servicio no puede ser negativo";
                return false;
            }
            if (servicio.Costos < 0)
            {
                mensaje = "El costo del servicio no puede ser negativo";
                return false;
            }
            if (servicio.Margen < 0)
            {
                mensaje = "El margen del servicio no puede ser negativo";
                return false;
            }
            if (servicio.Comision < 0)
            {
                mensaje = "La comisión del servicio no puede ser negativa";
                return false;
            }
            if (servicio.DuracionMinutos < 0)
            {
                mensaje = "La duración del servicio no puede ser negativa";
                return false;
            }
            if (servicio.Puntaje < 0)
            {
                mensaje = "El puntaje del servicio no puede ser negativo";
                return false;
            }
            if (servicio.IdCategoria < 0)
            {
                if (servicio.Categorias == null)
                {
                    mensaje = "Error con el Id de la Categoría del Servicio";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(servicio.Categorias.Descripcion))
                {
                    mensaje = "Error con la Categoría del Servicio a registrar";
                    return false;
                }
            }
            if (!registro)
            {
                if (servicio.IdServicio == null || servicio.IdServicio < 1)
                {
                    mensaje = "Error con el Id del Servicio a manipular";
                    return false;
                }
                Servicios? existe = ServiciosDatos.ObtenerServicioPorId((int)servicio.IdServicio, ref mensaje);
                if (existe == null)
                {
                    mensaje = "El Servicio a modificar no existe en los registros";
                    return false;
                }
            }
            return true;
        }

        public static List<Servicios>? Listar(ref string mensaje)
        {
            try
            {
                List<Servicios>? lista = ServiciosDatos.ListaServicios(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Servicios:\n{ex.Message}";
                return null;
            }
        }

        public static Servicios? NombreExiste(string nombreServicio, ref string mensaje)
        {
            try
            {
                Servicios? servicio = ServiciosDatos.ObtenerServicioPorNombre(nombreServicio, ref mensaje);
                return servicio;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al verificar el nombre del servicio:\n{ex.Message}";
                return null;
            }
        }

        public static bool Registrar(Servicios? servicio, List<CostosServicios>? costos, ref string mensaje)
        {
            if (!ComprobarServicio(servicio, true, ref mensaje))
                return false;
            try
            {
                if (servicio.Categorias != null)
                {
                    if (servicio.Categorias.IdCategoria == null)
                        servicio.IdCategoria = 0;
                    else
                    {
                        servicio.IdCategoria = (int)servicio.Categorias.IdCategoria;
                        servicio.Categorias = null;
                    }
                }
                string mensajeDatos = "";
                int exito = ServiciosDatos.RegistrarServicio(servicio, ref mensajeDatos);
                if (exito <= 0)
                {
                    mensaje = $"Servicio No Registrado\n{mensajeDatos}";
                    return false;
                }
                bool errorRegistroCostos = true;
                if (costos != null)
                    errorRegistroCostos = !CostosNegocio.RegistrarListaCostos(costos, exito, ref mensaje);
                
                mensaje = errorRegistroCostos ? $"Servicio Registrado (errores en insumos-costos)\n{mensaje}" : "Servicio Registrado Correctamente";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar el servicio:\n{ex.Message}";
                return false;
            }
        }

        public static bool Modificar(Servicios? servicio, ref string mensaje)
        {
            if (!ComprobarServicio(servicio, false, ref mensaje))
                return false;
            try
            {
                string mensajeDatos = "";
                bool exito = ServiciosDatos.ModificarServicio(servicio, ref mensajeDatos);
                mensaje = exito ? "Servicio Modificado Correctamente" : $"Servicio No Modificado\n{mensajeDatos}";
                return exito;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar el servicio:\n{ex.Message}";
                return false;
            }
        }
        /*
        static bool gestionarCostosServicios(List<CostosServicios>? costos, int idServicio, ref string mensaje)
        {
            if (costos == null)
                return false;
            try
            {

            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado" + ex.Message;
                return false;
            }
        }
        */
    }
}

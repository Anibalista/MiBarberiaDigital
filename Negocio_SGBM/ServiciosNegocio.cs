using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

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
                bool errorRegistroCostos = false;
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

        /// <summary>
        /// Orquesta la búsqueda avanzada de servicios: valida parámetros, normaliza y delega a la capa de datos.
        /// Aquí se pueden añadir reglas de negocio (p. ej. límites, permisos, logging de auditoría).
        /// </summary>
        public static Resultado<List<Servicios>> BuscarServiciosAvanzado(string campo, string criterio, string valor, int idCategoria)
        {
            // Validaciones y normalizaciones de negocio
            if (string.IsNullOrWhiteSpace(campo))
                return Resultado<List<Servicios>>.Fail("El campo de búsqueda es obligatorio.");

            if (string.IsNullOrWhiteSpace(criterio))
                return Resultado<List<Servicios>>.Fail("El criterio de búsqueda es obligatorio.");

            // Normalizar criterio conocido
            criterio = criterio.Trim();

            // Normalizar valor según tipo de campo: si el campo es texto, aplicar Trim y ToLowerInvariant en la capa de datos si corresponde.
            valor = valor?.Trim() ?? string.Empty;

            // Reglas de negocio adicionales (ejemplos):
            // - Limitar longitud de valor para evitar consultas costosas
            if (valor.Length > 149)
                return Resultado<List<Servicios>>.Fail("El valor de búsqueda es demasiado largo.");

            // Delegar a la capa de datos
            var resultadoDatos = ServiciosDatos.BuscarAvanzado(campo, criterio, valor, idCategoria);
            if (!resultadoDatos.Success)
                return Resultado<List<Servicios>>.Fail(resultadoDatos.Mensaje);

            // Posible post-procesado de negocio (orden, filtrado adicional, mapeo DTO)
            var lista = resultadoDatos.Data ?? new List<Servicios>();

            return Resultado<List<Servicios>>.Ok(lista);
        }

    }
}

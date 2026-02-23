using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de Servicios.
    /// 
    /// Responsabilidades:
    /// - Validar datos de entrada antes de invocar la capa de datos.
    /// - Centralizar reglas mínimas de negocio (ej. precios, márgenes, duración).
    /// - Delegar operaciones CRUD a <see cref="ServiciosDatos"/>.
    /// - Devolver resultados uniformes mediante <see cref="Resultado{T}"/>.
    /// 
    /// Buenas prácticas:
    /// - No usar parámetros por referencia para mensajes; todos los mensajes de error o éxito
    ///   se devuelven dentro de <see cref="Resultado{T}"/>.
    /// - Mantener validaciones simples en esta capa; reglas más complejas pueden residir en una
    ///   capa superior de servicios si el proyecto escala.
    /// - Registrar mensajes técnicos en <c>Logger</c> en la capa de datos; aquí solo se devuelven
    ///   mensajes amigables.
    /// </summary>
    public class ServiciosNegocio
    {
        /// <summary>
        /// Valida un servicio antes de registrar o modificar.
        /// </summary>
        private static Resultado<Servicios> ComprobarServicio(Servicios? servicio, bool registro)
        {
            if (servicio == null)
                return Resultado<Servicios>.Fail("La información del servicio no llega a la consulta.");

            if (servicio.PrecioVenta < 0)
                return Resultado<Servicios>.Fail("El precio del servicio no puede ser negativo.");
            if (servicio.Costos < 0)
                return Resultado<Servicios>.Fail("El costo del servicio no puede ser negativo.");
            if (servicio.Margen < 0)
                return Resultado<Servicios>.Fail("El margen del servicio no puede ser negativo.");
            if (servicio.Comision < 0)
                return Resultado<Servicios>.Fail("La comisión del servicio no puede ser negativa.");
            if (servicio.DuracionMinutos < 0)
                return Resultado<Servicios>.Fail("La duración del servicio no puede ser negativa.");
            if (servicio.Puntaje < 0)
                return Resultado<Servicios>.Fail("El puntaje del servicio no puede ser negativo.");

            if (servicio.IdCategoria < 1) // El servicio no tiene un ID asignado directamente (es 0)
            {
                // Si no hay ID, sí o sí debe venir el objeto Categorias para crearlo o extraer su ID
                if (servicio.Categorias == null)
                    return Resultado<Servicios>.Fail("Debe asignar una categoría válida al servicio.");

                // Validamos la nueva categoría
                if (string.IsNullOrWhiteSpace(servicio.Categorias.Descripcion))
                    return Resultado<Servicios>.Fail("La categoría a registrar no tiene descripción.");

                if (servicio.Categorias.Descripcion.Length > 99)
                    return Resultado<Servicios>.Fail("La descripción de la categoría no puede exceder los 99 caracteres.");

                // Usamos .HasValue para verificar de forma segura si el int? tiene un número.
                // Si tiene valor y es mayor a 0, significa que la categoría YA EXISTE en la BD.
                if (servicio.Categorias.IdCategoria.HasValue && servicio.Categorias.IdCategoria.Value > 0)
                {
                    servicio.IdCategoria = servicio.Categorias.IdCategoria.Value;
                    servicio.Categorias = null; // Soltamos el objeto para que EF solo use el ID
                }
                else
                {
                    // Si llegamos aquí, IdCategoria es 'null' o '0'. 
                    // Es una categoría GENUINAMENTE NUEVA.
                    servicio.IdCategoria = 0;

                    // Forzamos el ID de la categoría a 0 (aunque venga en null) porque SQL Server 
                    // prefiere el 0 explícito para generar el Identity de las Foreign Keys al vuelo.
                    servicio.Categorias.IdCategoria = null;
                }
            }

            if (!registro && (servicio.IdServicio == null || servicio.IdServicio < 1))
                return Resultado<Servicios>.Fail("Error con el Id del Servicio a manipular.");

            var resultado = Resultado<Servicios>.Ok(servicio);


            return resultado;
        }

        /// <summary>
        /// Lista todos los servicios.
        /// </summary>
        public static Resultado<List<Servicios>> Listar()
        {
            try
            {
                return ServiciosDatos.ListaServicios();
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de Servicios:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Servicios>>.Fail(msg);
            }
        }

        /// <summary>
        /// Verifica si existe un servicio por nombre.
        /// </summary>
        public static Resultado<Servicios?> NombreExiste(string nombreServicio)
        {
            if (string.IsNullOrWhiteSpace(nombreServicio))
                return Resultado<Servicios?>.Fail("El nombre del servicio no puede estar vacío.");

            try
            {
                return ServiciosDatos.ObtenerServicioPorNombre(nombreServicio);
            }
            catch (Exception ex)
            {
                var msg = $"Error al verificar el nombre del servicio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Servicios?>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo servicio junto con sus costos asociados.
        /// </summary>
        public static Resultado<bool> Registrar(Servicios? servicio, List<CostosServicios>? costos)
        {
            var validacion = ComprobarServicio(servicio, true);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            servicio = validacion.Data;

            try
            {
                var resultadoDatos = ServiciosDatos.RegistrarServicio(servicio);
                if (!resultadoDatos.Success)
                    return Resultado<bool>.Fail($"Servicio no registrado.\n{resultadoDatos.Mensaje}");

                var idServicio = resultadoDatos.Data;
                bool errorRegistroCostos = false;

                if (costos != null)
                {
                    var resultadoCostos = CostosNegocio.RegistrarListaCostos(costos, idServicio);
                    if (!resultadoCostos.Success)
                        errorRegistroCostos = true;
                }

                if (errorRegistroCostos)
                    return Resultado<bool>.Ok(true, "Servicio registrado con errores en insumos-costos.");

                return Resultado<bool>.Ok(true, "Servicio registrado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar el servicio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un servicio existente.
        /// </summary>
        public static Resultado<bool> Modificar(Servicios? servicio)
        {
            var validacion = ComprobarServicio(servicio, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            servicio = validacion.Data;
            try
            {
                var resultadoDatos = ServiciosDatos.ModificarServicio(servicio!);
                if (!resultadoDatos.Success)
                    return Resultado<bool>.Fail($"Servicio no modificado.\n{resultadoDatos.Mensaje}");

                return Resultado<bool>.Ok(true, "Servicio modificado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar el servicio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Orquesta la búsqueda avanzada de servicios: valida parámetros, normaliza y delega a la capa de datos.
        /// Aquí se pueden añadir reglas de negocio (p. ej. límites, permisos, logging de auditoría).
        /// </summary>
        public static Resultado<List<Servicios>> BuscarServiciosAvanzado(string campo, string criterio, string valor, int idCategoria)
        {
            if (string.IsNullOrWhiteSpace(campo))
                return Resultado<List<Servicios>>.Fail("El campo de búsqueda es obligatorio.");

            if (string.IsNullOrWhiteSpace(criterio))
                return Resultado<List<Servicios>>.Fail("El criterio de búsqueda es obligatorio.");

            criterio = criterio.Trim();
            valor = valor?.Trim() ?? string.Empty;

            if (valor.Length > 149)
                return Resultado<List<Servicios>>.Fail("El valor de búsqueda es demasiado largo.");

            try
            {
                return ServiciosDatos.BuscarAvanzado(campo, criterio, valor, idCategoria);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda avanzada de servicios:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Servicios>>.Fail(msg);
            }
        }
    }
}
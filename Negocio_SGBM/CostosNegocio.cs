using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de Costos/Insumos de los Servicios.
    /// 
    /// Responsabilidades:
    /// - Validar datos de entrada antes de invocar la capa de datos.
    /// - Centralizar reglas mínimas de negocio (ej. monto positivo, descripción obligatoria).
    /// - Delegar operaciones CRUD a <see cref="CostosDatos"/>.
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
    public class CostosNegocio
    {
        /// <summary>
        /// Valida un costo antes de registrar o modificar.
        /// </summary>
        private static Resultado<CostosServicios> ComprobarCosto(CostosServicios? costo, bool registro)
        {
            if (costo == null)
                return Resultado<CostosServicios>.Fail("No llega el costo del servicio a la consulta.");

            if (costo.Costo <= 0)
                return Resultado<CostosServicios>.Fail("El monto del costo del servicio debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(costo.Descripcion))
                return Resultado<CostosServicios>.Fail("La descripción del costo del servicio no puede estar vacía.");

            if (costo.IdServicio < 1)
                return Resultado<CostosServicios>.Fail("El Id del servicio asociado al costo no es válido.");

            if (costo.IdProducto.HasValue && costo.IdProducto > 0)
            {
                costo.Productos = null; // Evitar inconsistencias al registrar/modificar con un producto asociado
            }

            if (costo.Productos != null)
            {
                if (costo.Productos.IdProducto.HasValue && costo.Productos.IdProducto > 0)
                {
                    costo.IdProducto = costo.Productos.IdProducto;
                    costo.Productos = null; // Evitar redundancia al tener el IdProducto
                } else
                {
                    costo.IdProducto = null; // Si el producto no tiene Id válido, no asociar
                    costo.Productos = null;
                }
                    
            }

            if (!registro && (costo.IdCostoServicio == null || costo.IdCostoServicio < 1))
                return Resultado<CostosServicios>.Fail("Error al ligar el costo al servicio.");

            var resutado = Resultado<CostosServicios>.Ok(costo);

            return resutado;
        }

        /// <summary>
        /// Obtiene los insumos/costos asociados a un servicio.
        /// </summary>
        public static Resultado<List<CostosServicios>> ObtenerInsumosPorIdServicio(int idServicio)
        {
            if (idServicio < 1)
                return Resultado<List<CostosServicios>>.Fail("El Id del servicio no es válido.");

            try
            {
                return CostosDatos.ObtenerCostosPorIdServicio(idServicio);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener los insumos-costos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<CostosServicios>>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra una lista de costos asociados a un servicio.
        /// </summary>
        public static Resultado<bool> RegistrarListaCostos(List<CostosServicios>? costos, int idServicio)
        {
            if (costos == null)
                return Resultado<bool>.Fail("No llegan los costos-insumos a registrar.");

            if (idServicio <= 0)
                return Resultado<bool>.Fail("El Id del servicio no es válido.");

            try
            {
                var errores = new List<string>();

                foreach (var c in costos)
                {
                    c.IdCostoServicio = null;
                    c.IdServicio = idServicio;

                    var validacion = ComprobarCosto(c, true);
                    if (!validacion.Success)
                    {
                        errores.Add(validacion.Mensaje);
                        continue;
                    }

                    CostosServicios costo = validacion.Data;

                    var resultadoDatos = CostosDatos.RegistrarCosto(costo);
                    if (!resultadoDatos.Success || resultadoDatos.Data <= 0)
                        errores.Add(resultadoDatos.Mensaje);
                }

                if (errores.Any())
                    return Resultado<bool>.Fail("Problemas al registrar los costos-insumos:\n" + string.Join("\n", errores));

                return Resultado<bool>.Ok(true, "Costos-insumos registrados correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al registrar costos-insumos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un costo existente.
        /// </summary>
        public static Resultado<bool> ModificarCosto(CostosServicios? costo)
        {
            var validacion = ComprobarCosto(costo, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            costo = validacion.Data;
            try
            {
                
                var resultadoDatos = CostosDatos.ModificarCosto(costo);
                if (!resultadoDatos.Success)
                    return Resultado<bool>.Fail($"Costo no modificado.\n{resultadoDatos.Mensaje}");

                return Resultado<bool>.Ok(true, "Costo modificado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al modificar costo-insumo:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un costo existente.
        /// </summary>
        public static Resultado<bool> EliminarCosto(CostosServicios? costo)
        {
            var validacion = ComprobarCosto(costo, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);
            costo = validacion.Data;
            try
            {
                var resultadoDatos = CostosDatos.EliminarFisico(costo);
                if (!resultadoDatos.Success)
                    return Resultado<bool>.Fail($"Costo no eliminado.\n{resultadoDatos.Mensaje}");

                return Resultado<bool>.Ok(true, "Costo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al eliminar costo-insumo:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Gestiona los costos de un servicio: registra nuevos, modifica existentes y elimina los que ya no están.
        /// </summary>
        public static Resultado<bool> GestionarCostosServicios(List<CostosServicios>? costos, int idServicio)
        {
            if (costos == null)
                return Resultado<bool>.Fail("No llegan los costos-insumos a gestionar.");

            if (idServicio <= 0)
                return Resultado<bool>.Fail("El Id del servicio no es válido.");

            try
            {
                var registrar = costos.Where(c => c.IdCostoServicio == null || c.IdCostoServicio <= 0).ToList();
                var anteriores = registrar.Count != costos.Count
                    ? ObtenerInsumosPorIdServicio(idServicio).Data
                    : null;

                var errores = new List<string>();
                var exito = true;

                if (anteriores != null)
                {
                    var ids = costos.Select(c => c.IdCostoServicio).ToList();

                    foreach (var costoAnterior in anteriores)
                    {
                        if (!ids.Contains(costoAnterior.IdCostoServicio))
                        {
                            var resultadoEliminar = EliminarCosto(costoAnterior);
                            exito = exito && resultadoEliminar.Success;
                            if (!resultadoEliminar.Success) errores.Add(resultadoEliminar.Mensaje);
                        }
                        else
                        {
                            var modificar = costos.FirstOrDefault(c => c.IdCostoServicio == costoAnterior.IdCostoServicio);
                            if (modificar != null)
                            {
                                modificar.IdServicio = idServicio;
                                var resultadoModificar = ModificarCosto(modificar);
                                exito = exito && resultadoModificar.Success;
                                if (!resultadoModificar.Success) errores.Add(resultadoModificar.Mensaje);
                            }
                        }
                    }
                }

                if (registrar.Any())
                {
                    var resultadoRegistrar = RegistrarListaCostos(registrar, idServicio);
                    exito = exito && resultadoRegistrar.Success;
                    if (!resultadoRegistrar.Success) errores.Add(resultadoRegistrar.Mensaje);
                }

                if (errores.Any())
                    return Resultado<bool>.Fail("Problemas al gestionar costos-insumos:\n" + string.Join("\n", errores));

                return Resultado<bool>.Ok(exito, "Costos-insumos gestionados correctamente.");
            }
            catch (Exception ex)
            {
                var msg = $"Error inesperado al gestionar costos-insumos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}

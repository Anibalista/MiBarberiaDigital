using EF_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase que contiene métodos relacionados con la obtención y validación de cajas.
    /// </summary>
    public class CajasDatos
    {
        /// <summary>
        /// Obtiene la lista de cajas filtradas por fecha.
        /// </summary>
        /// <param name="fecha">Fecha de las cajas a consultar. Si es nula, se devuelve un error.</param>
        /// <param name="incluirCerradas">
        /// Indica si se deben incluir cajas cerradas en la consulta.
        /// Si es false, solo se devuelven cajas abiertas.
        /// </param>
        /// <returns>
        /// Resultado con Success = true y la lista de cajas encontradas,
        /// o Success = false con un mensaje de error en caso de fallo.
        /// </returns>
        public static Resultado<List<Cajas>> GetCajasPorFecha(DateTime? fecha, bool incluirCerradas = false)
        {
            // Validación inicial: la fecha es obligatoria.
            if (fecha == null)
                return Resultado<List<Cajas>>.Fail("La fecha de las cajas no llega a la consulta");

            try
            {
                using (Contexto contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    var resultadoContexto = comprobacion.ComprobarEntidad(contexto.Cajas);
                    if (!resultadoContexto.Success)
                        return Resultado<List<Cajas>>.Fail(resultadoContexto.Mensaje);

                    // Construimos la consulta:
                    // - Filtramos por rango de fecha (>= fecha y < fecha + 1 día).
                    //   Esto evita problemas con la propiedad Date y aprovecha índices en SQL.
                    // - Filtramos por estado de apertura: si incluirCerradas es true,
                    //   no se aplica restricción; si es false, solo se devuelven abiertas.
                    var query = contexto.Cajas.Where(c =>
                        c.Fecha >= fecha.Value.Date &&
                        c.Fecha < fecha.Value.Date.AddDays(1) &&
                        (incluirCerradas || c.Abierta));

                    // Devolvemos éxito con la lista resultante.
                    return Resultado<List<Cajas>>.Ok(query.ToList());
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                // Se loguea el detalle completo con ex.ToString() para diagnóstico.
                Logger.LogError(ex.ToString());

                // Se devuelve un resultado de error genérico para la capa de negocio.
                return Resultado<List<Cajas>>.Fail("Error en la consulta de cajas en la base de datos");
            }
        }

        /// <summary>
        /// Obtiene los tipos de cajas que NO tienen una caja abierta actualmente.
        /// Evita que se abra dos veces el mismo tipo de caja.
        /// </summary>
        public static Resultado<List<TiposCajas>> GetTiposCajasDisponibles()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    // 1. Buscamos los Ids de los Tipos que ya están abiertos
                    var idsTiposAbiertos = contexto.Cajas
                                            .Where(c => c.Abierta == true && c.Fecha >= DateTime.Today)
                                            .Select(c => c.IdTipo)
                                            .ToList();

                    // 2. Traemos solo los TiposCajas que NO estén en esa lista
                    var tiposDisponibles = contexto.TiposCajas
                                            .Where(tc => !idsTiposAbiertos.Contains(tc.IdTipo))
                                            .ToList();

                    return Resultado<List<TiposCajas>>.Ok(tiposDisponibles);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error al obtener tipos de cajas disponibles: {ex.ToString()}");
                return Resultado<List<TiposCajas>>.Fail("Error técnico al verificar la disponibilidad de cajas.");
            }
        }


        ///<summary>
        /// Obtiene las cajas abiertas filtradas por un rango de fechas.
        /// Si un parámetro es null, no aplica límite en ese extremo.
        ///</summary>
        public static Resultado<List<Cajas>> GetCajasAbiertasPorRangoFechas(DateTime? desde, DateTime? hasta)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Cajas, nameof(contexto.Cajas));

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Cajas>>.Fail(rc.Mensaje);
                    }

                    // 1. CONSULTA BASE: Empezamos filtrando SOLO las cajas abiertas.
                    // NOTA: Ajusta "c.FechaCierre == null" a la propiedad real que uses 
                    // para saber si está abierta (ej. c.Estado == "Abierta")
                    IQueryable<Cajas> query = contexto.Cajas.Where(c => c.HoraCierre == null || c.Abierta);

                    // 2. CONDICIÓN DESDE (Límite inferior)
                    if (desde.HasValue)
                    {
                        // Solo la fecha (sin hora) para que empiece a buscar desde las 00:00:00
                        DateTime limiteDesde = desde.Value.Date;
                        query = query.Where(c => c.Fecha >= limiteDesde);
                    }

                    // 3. CONDICIÓN HASTA (Límite superior)
                    if (hasta.HasValue)
                    {
                        // Si el usuario selecciona "Hoy" (ej: 03/03/2026), 
                        // debemos abarcar hasta las 23:59:59 de ese día, sino las cajas 
                        // abiertas a las 14:00hs no saldrían en el reporte.
                        DateTime limiteHasta = hasta.Value.Date.AddDays(1).AddTicks(-1);
                        query = query.Where(c => c.Fecha <= limiteHasta);
                    }

                    // 4. EJECUCIÓN: Recién aquí EF Core traduce los Where a SQL y va a la BD
                    var cajasAbiertas = query.OrderBy(c => c.Fecha).ToList();

                    string mensaje = cajasAbiertas.Any()
                        ? $"Se encontraron {cajasAbiertas.Count} cajas abiertas en el rango indicado."
                        : "No se encontraron cajas abiertas para los filtros aplicados.";

                    return Resultado<List<Cajas>>.Ok(cajasAbiertas, mensaje);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al buscar cajas abiertas por rango de fechas:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Cajas>>.Fail("Ocurrió un error inesperado al consultar el registro de cajas.");
            }
        }


        ///<summary>
        ///Crea una nueva caja con los datos proporcionados y la guarda en la base de datos.
        ///</summary>
        public static Resultado<Cajas> RegistrarCaja(Cajas? nuevaCaja)
        {
            // Validación inicial: la nueva caja no puede ser nula.
            if (nuevaCaja == null)
                return Resultado<Cajas>.Fail("No se proporcionó una caja válida para crear.");
            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    var resultadoContexto = comprobacion.ComprobarEntidad(contexto.Cajas);
                    if (!resultadoContexto.Success)
                        return Resultado<Cajas>.Fail(resultadoContexto.Mensaje);

                    // Aseguramos la fecha de creación y el estado inicial de la caja.
                    nuevaCaja.Fecha = DateTime.Now;
                    nuevaCaja.Abierta = true;

                    // Nulificamos el Id para que EF Core lo genere automáticamente al agregarlo.
                    nuevaCaja.IdCaja = null;

                    // Agregamos la nueva caja al contexto y guardamos los cambios.
                    contexto.Cajas.Add(nuevaCaja);
                    int exitos = contexto.SaveChanges();

                    // Si hubo cambios en la BD Devolvemos éxito
                    // Con la caja recién creada, que ahora incluye su Id generado.
                    return exitos > 0
                           ? Resultado<Cajas>.Ok(nuevaCaja, "Caja abierta correctamente.")
                           : Resultado<Cajas>.Fail("No se pudo registrar la apertura de la caja.");
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                Logger.LogError(ex.ToString());
                // Devolvemos un resultado de error genérico para la capa de negocio.
                return Resultado<Cajas>.Fail("Error al crear la caja en la base de datos");
            }
        }

        /// <summary>
        /// Cambia el estado de una caja a cerrada, actualizando su hora de cierre y estado.
        /// </summary>
        public static Resultado<Cajas> CerrarCaja(Cajas cajaACerrar)
        {
            // Validación inicial: la caja a cerrar no puede ser nula.
            if (cajaACerrar?.IdCaja == null)
                return Resultado<Cajas>.Fail("No se proporcionó una caja válida para cerrar.");
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    var resultadoContexto = comprobacion.ComprobarEntidad(contexto.Cajas);
                    if (!resultadoContexto.Success)
                        return Resultado<Cajas>.Fail(resultadoContexto.Mensaje);
                    // Buscamos la caja en la base de datos por su Id.
                    var cajaEnDb = contexto.Cajas.FirstOrDefault(c => c.IdCaja == cajaACerrar.IdCaja);
                    if (cajaEnDb == null)
                        return Resultado<Cajas>.Fail("La caja a cerrar no se encontró en la base de datos.");
                    // Actualizamos el estado de la caja a cerrada y establecemos la hora de cierre.
                    cajaEnDb.Abierta = false;
                    cajaEnDb.HoraCierre = DateTime.Now;

                    // Cruzamos la información de totales desde la caja proporcionada
                    // calculado previamente en la capa de negocio.
                    cajaEnDb.TotalEfectivo = cajaACerrar.TotalEfectivo;
                    cajaEnDb.TotalMP = cajaACerrar.TotalMP;

                    // Guardamos los cambios en la base de datos.
                    contexto.SaveChanges();
                    // Devolvemos éxito con la caja actualizada.
                    return Resultado<Cajas>.Ok(cajaEnDb);
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                Logger.LogError(ex.ToString());
                // Devolvemos un resultado de error genérico para la capa de negocio.
                return Resultado<Cajas>.Fail("Error al cerrar la caja en la base de datos");
            }
        }

        /// <summary>
        /// Obtiene los tipos de cajas disponibles en la base de datos.
        /// </summary>
        public static Resultado<List<TiposCajas>> GetTiposCajas()
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    var resultadoContexto = comprobacion.ComprobarEntidad(contexto.TiposCajas);
                    if (!resultadoContexto.Success)
                        return Resultado<List<TiposCajas>>.Fail(resultadoContexto.Mensaje);
                    // Devolvemos éxito con la lista de tipos de cajas.
                    return Resultado<List<TiposCajas>>.Ok(contexto.TiposCajas.ToList());
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                Logger.LogError(ex.ToString());
                // Devolvemos un resultado de error genérico para la capa de negocio.
                return Resultado<List<TiposCajas>>.Fail("Error al obtener los tipos de cajas desde la base de datos");
            }
        }

        /// <summary>
        /// Obtiene un tipo de caja por descripción.
        /// </summary>
        public static Resultado<TiposCajas> GetTipoCajaPorDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return Resultado<TiposCajas>.Fail("La descripción del tipo de caja no puede estar vacía.");

            var descripcionNormalizada = descripcion.Trim().ToLower();
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    // Comprobamos que el contexto y sus entidades estén disponibles.
                    var comprobacion = new ComprobacionContexto(contexto);
                    var resultadoContexto = comprobacion.ComprobarEntidad(contexto.TiposCajas);
                    if (!resultadoContexto.Success)
                        return Resultado<TiposCajas>.Fail(resultadoContexto.Mensaje);
                    // Buscamos el tipo de caja por su descripción.
                    var tipoCaja = contexto.TiposCajas.FirstOrDefault(tc => tc.Tipo.ToLower() == descripcionNormalizada);
                    if (tipoCaja == null)
                        return Resultado<TiposCajas>.Fail("No se encontró un tipo de caja con la descripción proporcionada.");
                    // Devolvemos éxito con el tipo de caja encontrado.
                    return Resultado<TiposCajas>.Ok(tipoCaja);
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción inesperada.
                Logger.LogError(ex.ToString());
                // Devolvemos un resultado de error genérico para la capa de negocio.
                return Resultado<TiposCajas>.Fail("Error al obtener el tipo de caja desde la base de datos");
            }
        }
    }
}

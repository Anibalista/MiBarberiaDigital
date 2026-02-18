using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de domicilios, localidades y provincias.
    /// Contiene validaciones y preparación de datos antes de acceder a la capa de datos.
    /// </summary>
    public class DomiciliosNegocio
    {
        #region Provincias

        /// <summary>
        /// Obtiene todas las provincias disponibles.
        /// </summary>
        public static Resultado<List<Provincias>> GetProvincias()
        {
            try
            {
                return DomiciliosDatos.GetProvincias();
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener provincias:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Provincias>>.Fail(msg);
            }
        }

        #endregion

        #region Localidades

        /// <summary>
        /// Obtiene una localidad genérica (por defecto Gualeguaychú).
        /// Si no existe, la registra junto con su provincia.
        /// </summary>
        public static Resultado<Localidades> GetLocalidadGenerica(Localidades? localidad)
        {
            if (localidad != null)
            {
                if (localidad.IdLocalidad < 1 || localidad.IdProvincia < 1)
                    return Resultado<Localidades>.Fail("No se podrá registrar el domicilio ingresado (problemas con localidad).");

                return Resultado<Localidades>.Ok(localidad);
            }

            // Si no llega localidad, se busca por nombre o se crea nueva
            var resultadoLocalidad = GetLocalidadPorNombre("Gualeguaychú");
            localidad = resultadoLocalidad.Success
                ? resultadoLocalidad.Data
                : new Localidades { Localidad = "Gualeguaychú" };

            if (localidad.IdProvincia > 0)
            {
                localidad.Provincias = null;
                return Resultado<Localidades>.Ok(localidad);
            }

            var resultadoProvincias = GetProvincias();
            localidad.Provincias = resultadoProvincias.Data?.FirstOrDefault(p => p.Provincia == "Entre Ríos")
                                   ?? new Provincias { Provincia = "Entre Ríos" };

            if (localidad.Provincias.IdProvincia > 0)
            {
                localidad.IdProvincia = localidad.Provincias.IdProvincia;
                localidad.Provincias = null;
            }

            var resultadoRegistro = RegistrarLocalidad(localidad);
            if (!resultadoRegistro.Success)
                return Resultado<Localidades>.Fail(resultadoRegistro.Mensaje);

            localidad.IdLocalidad = resultadoRegistro.Data;
            return Resultado<Localidades>.Ok(localidad);
        }

        /// <summary>
        /// Obtiene las localidades de una provincia.
        /// </summary>
        public static Resultado<List<Localidades>> GetLocalidadesPorProvincia(Provincias? provincia)
        {
            if (provincia == null || provincia.IdProvincia <= 0)
                return Resultado<List<Localidades>>.Fail("La provincia no es válida.");

            try
            {
                return DomiciliosDatos.GetLocalidadesPorProvincia(provincia);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener localidades por provincia:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Localidades>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene todas las localidades.
        /// </summary>
        public static Resultado<List<Localidades>> GetLocalidades()
        {
            try
            {
                return DomiciliosDatos.GetLocalidades();
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener localidades:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Localidades>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene una localidad por nombre.
        /// </summary>
        public static Resultado<Localidades?> GetLocalidadPorNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Resultado<Localidades?>.Fail("El nombre de la localidad no puede estar vacío.");

            try
            {
                return DomiciliosDatos.GetLocalidadPorNombre(nombre);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener localidad por nombre:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Localidades?>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra una nueva localidad.
        /// </summary>
        public static Resultado<int> RegistrarLocalidad(Localidades? localidad)
        {
            if (localidad == null)
                return Resultado<int>.Fail("No llega información de la localidad a la capa negocio.");

            if (localidad.IdProvincia < 1 && localidad.Provincias == null)
                return Resultado<int>.Fail("No llega información de la provincia a la capa negocio.");

            try
            {
                return DomiciliosDatos.RegistrarLocalidad(localidad);
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar localidad:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        #endregion

        #region Domicilios

        /// <summary>
        /// Registra un nuevo domicilio.
        /// </summary>
        public static Resultado<int> RegistrarDomicilio(Domicilios? domicilio)
        {
            if (domicilio == null)
                return Resultado<int>.Fail("El domicilio no llega a la capa de negocio.");

            if (domicilio.IdDomicilio > 0)
                return Resultado<int>.Fail("No es posible registrar un domicilio existente.");

            try
            {
                return DomiciliosDatos.RegistrarDomicilio(domicilio);
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un domicilio existente.
        /// </summary>
        public static Resultado<bool> ModificarDomicilio(Domicilios? domicilio)
        {
            if (domicilio == null)
                return Resultado<bool>.Fail("El domicilio no llega a la capa de negocio.");

            if (domicilio.IdDomicilio == null || domicilio.IdDomicilio < 1)
                return Resultado<bool>.Fail("El Id del domicilio no es válido.");

            try
            {
                return DomiciliosDatos.ModificarDomicilio(domicilio);
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un domicilio por entidad.
        /// </summary>
        public static Resultado<bool> EliminarDomicilio(Domicilios? domicilio)
        {
            if (domicilio == null)
                return Resultado<bool>.Fail("El domicilio no llega a la capa de negocio.");

            if (domicilio.IdDomicilio == null || domicilio.IdDomicilio < 1)
                return Resultado<bool>.Fail("El Id del domicilio no es válido.");

            try
            {
                return DomiciliosDatos.EliminarDomicilioPorId(domicilio.IdDomicilio.Value);
            }
            catch (Exception ex)
            {
                var msg = $"Error al eliminar domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un domicilio por Id.
        /// </summary>
        public static Resultado<bool> EliminarDomicilio(int idDomicilio)
        {
            if (idDomicilio < 1)
                return Resultado<bool>.Fail("El Id del domicilio no es válido.");

            try
            {
                return DomiciliosDatos.EliminarDomicilioPorId(idDomicilio);
            }
            catch (Exception ex)
            {
                var msg = $"Error al eliminar domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion
    }
}

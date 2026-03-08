using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Negocio_SGBM
{
    public class CajasNegocios
    {
        ///<summary>
        /// Comprueba la integridad de los datos de una caja y devuelve
        /// la caja con los datos corregidos o una caja vacía si los datos no son válidos.
        /// </summary>
        private static Resultado<Cajas> ValidarCaja(Cajas? caja, bool registro)
        {
            if (caja == null)
                return Resultado<Cajas>.Fail("No llega la caja a la capa de negocio");

            if (caja.IdTipo > 0)
                caja.TiposCajas = null;
            else if (caja.TiposCajas?.IdTipo < 1)
                return Resultado<Cajas>.Fail("Error en el tipo de cajas");

            if (!registro && caja.IdCaja < 1)
                return Resultado<Cajas>.Fail("No llega el id de caja a la capa de negocio");

            if (registro)
                caja.IdCaja = null;
            
            if (caja.Fecha == DateTime.MinValue)
                caja.Fecha = DateTime.Now;

            if (caja.HoraCierre != null && caja.HoraCierre < caja.Fecha)
                return Resultado<Cajas>.Fail("La hora de cierre no puede ser anterior a la fecha de apertura");

            if (caja.TotalEfectivo < 0)
                return Resultado<Cajas>.Fail("El total de efectivo no puede ser negativo");


            return Resultado<Cajas>.Ok(caja);
        }

        /// <summary>
        /// Registra la apertura de la caja validando sus reglas previas.
        /// </summary>
        public static Resultado<Cajas> RegistrarApertura(Cajas nuevaCaja)
        {
            // Usamos tu método privado ValidarCaja
            var resCheck = ValidarCaja(nuevaCaja, true);
            if (!resCheck.Success || resCheck.Data == null)
                return Resultado<Cajas>.Fail(resCheck.Mensaje);

            return CajasDatos.RegistrarCaja(resCheck.Data);
        }

        /// <summary>
        /// Devuelve los tipos de caja que se pueden abrir en este momento.
        /// </summary>
        public static Resultado<List<TiposCajas>> GetTiposCajasDisponibles()
        {
            return CajasDatos.GetTiposCajasDisponibles();
        }
    }
}

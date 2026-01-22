using EF_SGBM;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ComprobacionContexto
    {
        private readonly Contexto contexto;

        public ComprobacionContexto(Contexto contexto)
        {
            this.contexto = contexto;
        }

        /// <summary>
        /// Comprueba si la entidad existe en el contexto y genera el mensaje automáticamente.
        /// </summary>
        public bool ComprobarEntidad<T>(
            T entidad,
            ref string mensaje,
            [CallerArgumentExpression("entidad")] string nombreEntidad = "")
        {
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return false;
            }

            if (entidad == null)
            {
                mensaje = $"No se conecta al registro de {nombreEntidad}";
                return false;
            }

            return true;
        }
    }
}

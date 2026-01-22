using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ProductosDatos
    {
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);
            if (!comprobar.ComprobarEntidad(contexto.UnidadesMedidas, ref mensaje))
                return false;
            if (!comprobar.ComprobarEntidad(contexto.Productos, ref mensaje))
                return false;
            return comprobar.ComprobarEntidad(contexto.Proveedores, ref mensaje);
        }

        public static List<Productos>? ListarSimple(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Productos
                                   .OrderBy(p => p.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener productos:\n{ex.Message}";
                return null;
            }
        }
    }
}

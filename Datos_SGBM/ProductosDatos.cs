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
            if (!comprobar.Comprobar(ref mensaje))
                return false;
            if (!comprobar.ComprobarUnidadesMedidas(ref mensaje))
                return false;
            if (!comprobar.ComprobarProveedores(ref mensaje))
                return false;
            return true;
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

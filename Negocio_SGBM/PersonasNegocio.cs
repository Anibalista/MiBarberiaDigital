using Datos_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class PersonasNegocio
    {
        //Consultas
        public static int getIdPersonaPorDni (string? dni, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni ingresado no llega a la consulta";
                return -1;
            }
            int id = 0;
            try
            {
                id = PersonasDatos.getIdPersonaPorDni(dni,ref mensaje);
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return -2;
            }
            return id;
        }

    }
}

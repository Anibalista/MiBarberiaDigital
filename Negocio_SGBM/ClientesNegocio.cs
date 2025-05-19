using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class ClientesNegocio
    {
        //Consultas
        public static Clientes? getClientePorDni(string? dni, ref string mensaje)
        {
            int idPersona = PersonasNegocio.getIdPersonaPorDni(dni, ref mensaje);
            if (idPersona < 1)
            {
                return null;
            }
            Clientes? cliente = ClientesDatos.getClientePorIdPersona(idPersona, ref mensaje);
            return cliente;
        }
    }
}

using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ClientesDatos
    {
        static Contexto contexto;

        //Consulta
        public static Clientes? getClientePorIdPersona(int idPersona, ref string mensaje)
        {
            if (idPersona < 1)
            {
                mensaje = "El ID de la persona no llega a la consulta";
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return null;
            }
            if (contexto.Personas == null || contexto.Clientes == null)
            {
                mensaje = "No se conecta a la BD (Personas o clientes)";
                return null;
            }
            Clientes? cliente = null;
            try
            {
                cliente = contexto.Clientes.Include("Personas").Where(c => c.IdPersona == idPersona).FirstOrDefault();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }

            return cliente;
        } 
    }
}

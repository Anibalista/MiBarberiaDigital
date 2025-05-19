using EF_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class PersonasDatos
    {
        static Contexto contexto;

        //Consultas
        public static int getIdPersonaPorDni(string dni, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni no llega a la consulta";
                return -1;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return -1;
            }
            if (contexto.Personas == null)
            {
                mensaje = "No se conecta a la BD (Personas)";
                return -1;
            }
            int id = 0;
            try
            {
                id = contexto.Personas.Where(p => p.Dni == dni).Select(p => p.IdPersona).FirstOrDefault();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return -2;
            }
            return id;          

        }
    }
}

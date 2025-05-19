using EF_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class DomiciliosDatos
    {
        static Contexto contexto;
        //Provincias-----------------------------------
        //Consultas
        public static List<Provincias>? getProvincias()
        {
            contexto = new Contexto();
            if (contexto == null)
            {
                return null;
            }
            if (contexto.Provincias == null)
            {
                return null;
            }
            List<Provincias>? provincias = contexto.Provincias.ToList();
            return provincias;
        }

        //Localidades------------------------------------
        //Consultas
        public static List<Localidades>? getLocalidadesPorProvincia(Provincias? provincia)
        {
            if (provincia == null)
            {
                return null;
            }
            if (provincia.IdProvincia == null)
            {
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                return null;
            }
            if (contexto.Localidades == null)
            {
                return null;
            }
            List<Localidades>? localidades = contexto.Localidades.Where(l => l.IdProvincia == provincia.IdProvincia).ToList();
            return localidades;

        }

        //Domicilios------------------------------------
        //Consultas
    }
}

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
        public static Domicilios? getDomicilioPorPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "No llegan los datos de la persona a la capa datos";
                return null;
            }
            if (persona.IdDomicilio == null)
            {
                mensaje = "No llegan los datos de la persona a la capa datos";
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas al conectar a la BD";
                return null;
            }
            if (contexto.Provincias == null || contexto.Localidades == null)
            {
                mensaje = "Problemas al conectar a la BD (Localidad, Provincia)";
                return null;
            }
            Domicilios? domicilio = null;
            try
            {
                domicilio = contexto.Domicilios.Include("Localidades.Provincias").Where(d => d.IdDomicilio == persona.IdDomicilio).FirstOrDefault();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            return domicilio;
        }


        //Registros
        public static int registrarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "No llegan los datos de domicilio a la capa datos";
                return -1;
            }
            domicilio.IdDomicilio = null;
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas al conectar a la BD";
                return -1;
            }
            if (contexto.Domicilios == null)
            {
                mensaje = "Problemas al conectar a la BD (domicilios)";
                return -1;
            }
            if (domicilio.IdLocalidad > 0)
            {
                domicilio.Localidades = null;
            }
            int exito = 0;
            try
            {
                contexto.Domicilios.Add(domicilio);
                exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            if (domicilio.IdDomicilio != null)
            {
                exito = (int)domicilio.IdDomicilio;
            }
            return exito;
        }

        //Modificaciones
        public static int modificarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "No llegan los datos de domicilio a la capa datos";
                return -1;
            }
            if (domicilio.IdDomicilio < 1)
            {
                mensaje = "No llegan los datos de domicilio a la capa datos";
                return -1;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas al conectar a la BD";
                return -1;
            }
            if (contexto.Domicilios == null)
            {
                mensaje = "Problemas al conectar a la BD (domicilios)";
                return -1;
            }
            Domicilios? d = null;
            try
            {
                d = contexto.Domicilios.Where(d => d.IdDomicilio == domicilio.IdDomicilio).FirstOrDefault();
            } catch(Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            if (d  == null)
            {
                mensaje = "No se encuentra el domicilio a modificar";
                return -1;
            }
            d.Calle = domicilio.Calle;
            d.Altura = domicilio.Altura;
            d.Piso = domicilio.Piso;
            d.Depto = domicilio.Depto;
            d.Barrio = domicilio.Barrio;
            d.IdLocalidad = domicilio.IdLocalidad;
            int modificado = 0;
            try
            {
                contexto.Domicilios.Update(d);
                modificado = contexto.SaveChanges();
            } catch(Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            if (modificado == 0 || d.IdDomicilio == null)
            {
                return 0;
            }
            return (int)d.IdDomicilio;
        }

        //Eliminaciones
        public static bool eliminarDomicilioPorId(int id, ref string mensaje)
        {
            if (id < 1)
            {
                mensaje = "No llega el id del domicilio a la capa datos";
                return false;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas al conectar a la BD";
                return false;
            }
            if (contexto.Domicilios == null)
            {
                mensaje = "Problemas al conectar a la BD (domicilios)";
                return false;
            }
            Domicilios? domicilio = null;
            try
            {
                domicilio = contexto.Domicilios.Where(d => d.IdDomicilio == id).FirstOrDefault();
            } catch(Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            if (domicilio == null)
            {
                mensaje = "No se pudo encontrar domicilio con el id proporcionado";
                return false;
            }
            int eliminado = 0;
            try
            {
                contexto.Domicilios.Remove(domicilio);
                eliminado = contexto.SaveChanges();
            } catch(Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }

            return eliminado > 0;
        }
    }
}

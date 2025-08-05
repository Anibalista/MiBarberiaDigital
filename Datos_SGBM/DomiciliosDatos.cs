using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Datos_SGBM
{
    public class DomiciliosDatos
    {
        static Contexto contexto;
        //Comprobaciones
        public static bool comprobarContexto(ref string mensaje)
        {
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
            if (contexto.Localidades == null || contexto.Provincias == null)
            {
                mensaje = "Problemas al conectar a la BD (Localidades o Provincias)";
                return false;
            }
            return true;
        }

        public static bool comprobarDomicilio(Domicilios? domicilio, bool registrando, ref string mensaje)
        {
            if (domicilio == null)
            {
                mensaje = "No llegan los datos de domicilio a la capa datos";
                return false;
            }
            if (!registrando)
            {
                if (domicilio.IdDomicilio == null)
                {
                    mensaje = "No llegan los datos de domicilio (Id) a la capa datos";
                    return false;
                }
            }
            bool exito = true;
            if (domicilio.IdLocalidad < 1)
            {

                exito = registrando && domicilio.Localidades != null;
                mensaje = exito ? "" : "Problemas con la localidad del domicilio";
            }
            return exito;
        }

        public static bool comprobarLocalidad(Localidades? localidad, bool registrando, ref string mensaje)
        {
            if (localidad == null)
            {
                mensaje = "No llega información de la localidad a la capa datos";
                return false;
            }
            if (!registrando)
            {
                if (localidad.IdLocalidad == null)
                {
                    mensaje = "No llegan los datos de localidad (Id) a la capa datos";
                    return false;
                }
            }
            bool exito = true;
            if (localidad.IdProvincia < 1)
            {
                exito = registrando && localidad.Provincias != null;
                mensaje = exito ? "" : "No llega información de la provincia a la capa datos";
            }
            return exito;

        }

        //Provincias-----------------------------------
        //Consultas
        public static List<Provincias>? getProvincias()
        {
            contexto = new Contexto();
            string error = string.Empty;
            if (!comprobarContexto(ref error))
            {
                Console.WriteLine(error);
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
            string error = string.Empty;
            if (!comprobarContexto(ref error))
            {
                Console.WriteLine(error);
                return null;
            }
            List<Localidades>? localidades = contexto.Localidades.Where(l => l.IdProvincia == provincia.IdProvincia).ToList();
            return localidades;

        }

        public static List<Localidades>? getLocalidades(ref string mensaje)
        {
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            List<Localidades>? localidades = null;
            try
            {
                localidades = contexto.Localidades.Include("Provincias").OrderBy(l => l.Localidad).ToList();
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            return localidades;
        }

        public static Localidades? getLocalidadPorNombre(string? nombre, ref string mensaje)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                mensaje = "No viaja el nombre de la localidad a consultar (capa datos)";
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            Localidades? localidad = null;
            try
            {
                localidad = contexto.Localidades.FirstOrDefault(l => l.Localidad == nombre);
            } catch (Exception ex)
            {
                mensaje = ex.Message + " (getLocalidadPorNombre)";
                return null;
            }
            return localidad;
        }

        public static List<Domicilios>? getDomiciliosPorCampos(string? calle, string? barrio, Localidades? localidad, ref string mensaje)
        {
            if (string.IsNullOrEmpty(calle) && string.IsNullOrEmpty(barrio) && localidad == null)
            {
                mensaje = "No llegan los datos de búsqueda de domicilios a la capa datos";
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            int idLocalidad = 0;
            if (localidad != null && localidad.IdLocalidad != null)
            {
                idLocalidad = (int)localidad.IdLocalidad;
            }
            List<Domicilios>? domicilios = null;
            try
            {
                domicilios = contexto.Domicilios.Include("Localidades.Provincias")
                    .Where(d => (!string.IsNullOrEmpty(d.Calle) 
                                && (d.Calle.Contains(calle ?? ""))) ||
                                (!string.IsNullOrEmpty(barrio) && d.Barrio.Contains(barrio ?? "")) ||
                                (d.IdLocalidad == idLocalidad))
                    .ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(getDomiciliosPorCampos)";
                return null;
            }
            return domicilios;

        }

        //Registros
        public static int registrarLocalidad(Localidades? localidad, ref string mensaje)
        {
            if (!comprobarLocalidad(localidad, true, ref mensaje))
            {
                return -1;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return -1;
            }
            int id = 0;
            if (localidad.IdProvincia > 0)
            {
                localidad.Provincias = null;
            }
            try
            {
                contexto.Localidades.Add(localidad);
                contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + " (registrarLocalidad)";
                return -1;
            }
            if (localidad.IdLocalidad != null)
            {
                id = (int)localidad.IdLocalidad;
            }
            return id;
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
                return null;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            Domicilios? domicilio = null;
            try
            {
                domicilio = contexto.Domicilios.Include("Localidades.Provincias").Where(d => d.IdDomicilio == persona.IdDomicilio).FirstOrDefault();
            } catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(getDomicilioPorPersona)";
                return null;
            }
            return domicilio;
        }


        //Registros
        public static int registrarDomicilio(Domicilios? domicilio, ref string mensaje)
        {
            if (!comprobarDomicilio(domicilio, true, ref mensaje))
            {
                return -1;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return -1;
            }
            domicilio.IdDomicilio = null;
            int exito = 0;
            try
            {
                contexto.Domicilios.Add(domicilio);
                exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(registrarDomicilio)";
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
            if (!comprobarDomicilio(domicilio, false, ref mensaje))
            {
                return -1;
            }
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return -1;
            }
            Domicilios? d = null;
            try
            {
                d = contexto.Domicilios.Where(d => d.IdDomicilio == domicilio.IdDomicilio).FirstOrDefault();
            } catch(Exception ex)
            {
                mensaje = ex.Message + " DomiciliosDatos(modificarDomicilio - Buscar)";
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
                mensaje = ex.Message + "DomiciliosDatos(modificarDomicilio - Guardar)";
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
            if (!comprobarContexto(ref mensaje))
            {
                return false;
            }
            Domicilios? domicilio = null;
            try
            {
                domicilio = contexto.Domicilios.Where(d => d.IdDomicilio == id).FirstOrDefault();
            } catch(Exception ex)
            {
                mensaje = ex.Message + "DomiciliosDatos(eliminarDomicilioPorId - Buscar)";
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
                mensaje = ex.Message + "DomiciliosDatos(eliminarDomicilioPorId - Guardar)";
                return false;
            }

            return eliminado > 0;
        }
    }
}

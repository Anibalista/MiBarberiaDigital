using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class PersonasDatos
    {
        static Contexto contexto;
        //Comprobaciones
        public static bool comprobarContexto(ref string mensaje)
        {
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return false;
            }
            if (contexto.Personas == null)
            {
                mensaje = "No se conecta a la BD (Personas)";
                return false;
            }
            return true;
        }


        //Consultas
        public static List<Personas>? getPersonas(ref string mensaje)
        {
            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }
            List<Personas>? personas = null;
            try
            {
                personas = contexto.Personas.Include("Domicilios").OrderBy(p => p.Apellidos).OrderBy(p => p.Nombres).ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return null;
            }
            return personas;

        }
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
            int? id = null;
            try
            {
                id = contexto.Personas.Where(p => p.Dni == dni).Select(p => p.IdPersona).FirstOrDefault();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return -2;
            }
            if (id == null)
            {
                return 0;
            }
            return (int)id;          

        }

        public static Personas? getPersonaPorDni(string dni, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni no llega a la consulta";
                return null;
            }
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return null;
            }
            if (contexto.Personas == null)
            {
                mensaje = "No se conecta a la BD (Personas)";
                return null;
            }
            Personas? persona = null;
            try
            {
                persona = contexto.Personas.Where(p => p.Dni == dni).FirstOrDefault();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return null;
            }
            mensaje = "";
            if (persona != null)
            {
                persona.Domicilios = DomiciliosDatos.getDomicilioPorPersona(persona, ref  mensaje);
            }
            return persona;
        }

        public static List<Personas>? getPersonasPorDniNombres(string? dni, string? nombres, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni) && String.IsNullOrWhiteSpace(nombres))
            {
                mensaje = "No llegan los datos de búsqueda";
                return null;
            }

            contexto = new Contexto();
            if (!comprobarContexto(ref mensaje))
            {
                return null;
            }

            List<Personas>? personas = null;
            try
            {
                personas = contexto.Personas
                            .Include(p => p.Domicilios)
                                .ThenInclude(d => d.Localidades)
                            .Where(p => (dni == null || p.Dni.Contains(dni)) ||
                                        (nombres == null || p.Nombres.Contains(nombres)))
                            .OrderBy(p => p.Apellidos)
                            .ThenBy(p => p.Nombres)
                            .ToList();
            }
            catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return null;
            }
            return personas;
        }

        //Registros
        public static int registrarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de la persona no llega a la capa datos";
                return -1;
            }
            persona.IdPersona = null;
            contexto = new Contexto();
            if (contexto == null)
            {
                mensaje = "Problemas de copnexión a la BD";
                return -1;
            }
            if (contexto.Personas == null)
            {
                mensaje = "Problemas de copnexión a la BD (Personas)";
                return -1;
            }
            if (persona.IdDomicilio != null)
            {
                persona.Domicilios = null;
            }
            if (persona.Domicilios != null)
            {
                persona.Domicilios.IdDomicilio = null;
                if (persona.Domicilios.IdLocalidad > 0)
                {
                    persona.Domicilios.Localidades = null;
                }
            }
            int exito = 0;
            try
            {
                contexto.Personas.Add(persona);
                exito = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return -1;
            }
            if (exito == 0)
            {
                mensaje = "No se pudo completar el registro de la persona";
                return 0;
            }
            if (persona.IdPersona == null)
            {
                mensaje = "No se pudo completar el registro de la persona";
                return 0;
            }
            return (int)persona.IdPersona;
        }


        //Modificaciones
        public static int modificarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "Los datos personales llegan vacíos";
                return -1;
            }
            if (persona.IdPersona < 1)
            {
                mensaje = "No llega el Id de la persona a la capa datos";
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
            Personas? p = null;
            try
            {
                p = contexto.Personas.Where(p => p.IdPersona == persona.IdPersona).FirstOrDefault();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return -1;
            }
            if (p == null)
            {
                mensaje = "No se puede encontrar a la persona a modificar";
                return -1;
            }
            p.Dni = persona.Dni;
            p.Nombres = persona.Nombres;
            p.Apellidos = persona.Apellidos;
            p.FechaNac = persona.FechaNac;
            p.IdDomicilio = persona.IdDomicilio;

            int modificado = 0;
            try
            {
                contexto.Personas.Update(p);
                modificado = contexto.SaveChanges();
            } catch (Exception ex)
            {
                mensaje = ex.Message + "PersonasDatos";
                return -1;
            }
            if (modificado == 0)
            {
                mensaje = "Problemas al modificar los registros de la persona";
            } else
            {
                mensaje = "";
            }
            
            return modificado;

        }
    }
}

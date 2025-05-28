using Datos_SGBM;
using Entidades_SGBM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_SGBM
{
    public class PersonasNegocio
    {
        //Métodos Mixtos
        public static bool gestionarContactosPorPersona(Personas? p, List<Contactos>? contactosEntrantes, ref string mensaje)
        {
            if (p == null)
            {
                mensaje += "\nNo se pudo gestionar los contactos por errores al obtener la información de la persona";
                return false;
            }
            Personas? persona = null;
            if (p.IdPersona == null)
            {
                persona = getPersonaPorDni(p.Dni, ref mensaje);
            } else
            {
                persona = p;
            }
            if (persona == null)
            {
                mensaje += "\nNo se pudo gestionar los contactos por errores al obtener la información de la persona";
                return false;
            }
            List<Contactos>? contactosPersona = null;
            string errores = "";
            try
            {
                contactosPersona = ContactosNegocio.getContactosPorPersona(persona, ref errores);
            } catch (Exception ex)
            {
                errores = ex.Message;
            }

            int contadorExitos = 0;
            int contadorErrores = 0;
            if (contactosEntrantes == null)
            {
                if (contactosPersona == null)
                {
                    mensaje += errores;
                    return false;
                }
                foreach (Contactos c in contactosPersona)
                {
                    if (ContactosNegocio.eliminarContacto(c, ref errores))
                        contadorExitos++;
                    else 
                        contadorErrores++;
                }
                mensaje += $"\nSe eliminaron {contadorExitos} contactos";
                if (contadorExitos != contactosPersona.Count)
                {
                    mensaje += $"\nNo se pudieron eliminar {contadorErrores} contactos";
                    return false;
                }
                return true;
            }

            foreach (Contactos c in contactosEntrantes)
            {
                c.IdPersona = persona.IdPersona;
            }
                errores = "";
            if (contactosPersona == null)
            {
                foreach (Contactos c in contactosEntrantes)
                {
                    c.IdContacto = null;
                    if (ContactosNegocio.registrarContacto(c, ref errores))
                        contadorExitos++;
                    else
                        contadorErrores++;
                }
                mensaje = $"\nSe registraron {contadorExitos} contactos";
                if (contadorExitos != contactosEntrantes.Count)
                {
                    mensaje += $"\nNo se pudieron registrar {contadorErrores} contactos";
                    return false;
                }
                return true;
            }
            
            errores = "";
            int erroresRegistro = 0;
            int exitosRegistros = 0;
            foreach (Contactos contacto in contactosEntrantes)
            {
                Contactos? cont = null;
                try
                {
                    cont = contactosPersona.Where(c => c.IdContacto == contacto.IdContacto).FirstOrDefault();
                } catch (Exception) { }
                
                if (cont != null)
                {
                    contactosPersona.Remove(cont);
                    if (!ContactosNegocio.modificarContacto(contacto, ref errores))
                        contadorErrores++;
                    else
                        contadorExitos++;
                } else
                {
                    contacto.IdContacto = null;
                    if (!ContactosNegocio.registrarContacto(contacto, ref errores))
                        erroresRegistro++;
                    else
                        exitosRegistros++;
                }
                if (!String.IsNullOrWhiteSpace(errores))
                {
                    Console.WriteLine(errores);
                } else
                {
                    errores = "";
                }
            }
            bool exito = true;
            if (contadorExitos > 0)
            {
                errores += $"\nSe modificaron {contadorExitos} contactos";
                contadorExitos = 0;
            }
            if (contadorErrores > 0)
            {
                exito = false;
                errores += $"\nNo se pudieron modificar {contadorErrores} contactos";
                contadorErrores = 0;
            }
            if (exitosRegistros > 0)
            {
                errores += $"\nSe registraron {exitosRegistros} contactos";
            }
            if (erroresRegistro > 0)
            {
                exito = false;
                errores += $"\nNo se pudieron registrar {erroresRegistro} contactos";
            }
            if (!String.IsNullOrWhiteSpace(errores))
            {
                mensaje += errores;
                errores = "";
            }

            if (contactosPersona.Count > 0)
            {
                foreach (Contactos contacto in contactosPersona)
                {
                    if (!ContactosNegocio.eliminarContacto(contacto, ref errores))
                        contadorErrores++;
                    else
                        contadorExitos++;
                    if (!String.IsNullOrWhiteSpace(errores))
                    {
                        Console.WriteLine(errores);
                        errores = "";
                    }
                }
            }
            if (contadorExitos > 0)
            {
                errores += $"\nSe eliminaron {contadorExitos} contactos";
            }
            if (contadorErrores > 0)
            {
                exito = false;
                errores += $"\nNo se pudieron modificar {contadorErrores} contactos";
            }
            mensaje += errores;
            return exito;
        }

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

        public static Personas? getPersonaPorDni(string? dni, ref string mensaje)
        {
            if (String.IsNullOrWhiteSpace(dni))
            {
                mensaje = "El Dni ingresado no llega a la consulta";
                return null;
            }
            Personas? persona = null;
            try
            {
                persona = PersonasDatos.getPersonaPorDni(dni, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return null;
            }
            return persona;
        }


        //Registros
        public static int registrarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "La información de la persona no llega a la capa negocio";
                return -1;
            }
            persona.IdPersona = null;
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
                    if (persona.Domicilios.Localidades != null)
                    {
                        if (persona.Domicilios.Localidades.IdProvincia > 0)
                        {
                            persona.Domicilios.Localidades.Provincias = null;
                        }
                    }
                }
            }
            int idPersona = 0;
            try
            {
                idPersona = PersonasDatos.registrarPersona(persona, ref mensaje);
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return -1;
            }
            return idPersona;
        }


        //Modificaciones
        public static bool modificarPersona(Personas? persona, ref string mensaje)
        {
            if (persona == null)
            {
                mensaje = "Los datos personales llegan vacíos";
                return false;
            }
            if (persona.IdPersona < 1)
            {
                mensaje = "No llega el Id de la persona a la capa negocio";
                return false;
            }
            Personas? p = null;
            p = PersonasDatos.getPersonaPorDni(persona.Dni, ref mensaje);
            if (p ==  null)
            {
                mensaje = "Problemas al obtener los datos de la persona";
                return false;
            }
            if (p.IdDomicilio != null)
            {
                if (persona.Domicilios != null)
                {
                    persona.Domicilios.IdDomicilio = (int)p.IdDomicilio;
                    if (!DomiciliosNegocio.modificarDomicilio(persona.Domicilios, ref mensaje))
                    {
                        return false;
                    }
                    persona.Domicilios = null;
                } else
                {
                    if (DomiciliosNegocio.eliminarDomicilio((int)p.IdDomicilio, ref mensaje))
                    {
                        persona.Domicilios = null;
                        persona.IdDomicilio = null;
                    } else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (persona.Domicilios != null)
                {
                    persona.IdDomicilio = DomiciliosNegocio.registrarDomicilio(persona.Domicilios, ref mensaje);
                    if (persona.IdDomicilio < 0)
                    {
                        return false;
                    } else
                    {
                        persona.Domicilios = null;
                        if (persona.IdDomicilio == 0)
                        {
                            persona.IdDomicilio = null;
                        }
                    }
                }
            }
            int modificada = 0;
            try
            {
                modificada = PersonasDatos.modificarPersona(persona, ref mensaje);
            } catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            if (modificada >= 0)
            {
                mensaje = "";
            }
            return modificada > 0;
        }
    }
}

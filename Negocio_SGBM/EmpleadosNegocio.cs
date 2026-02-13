

using Datos_SGBM;
using Entidades_SGBM;

namespace Negocio_SGBM
{
    public class EmpleadosNegocio
    {
        public static Empleados? ComprobarEmpleado(Empleados? Empleado, bool registro, ref string mensaje)
        {
            if (Empleado == null)
            {
                mensaje = "Problema al enviar datos de Empleado entre capas";
                return null;
            }
            if (Empleado.Personas == null)
            {
                mensaje = "Problema al enviar datos de la persona relacionada al Empleado entre capas";
                return null;
            }
            if (Empleado.IdEstado > 0)
                Empleado.Estados = null;
            if (Empleado.Estados != null)
                Empleado.IdEstado = Empleado.Estados.IdEstado ?? 0;
            if (Empleado.IdEstado < 1 && !registro)
            {
                mensaje = "Error al asignar un estado al Empleado en la capa negocio";
                return null;
            }
            if (!registro && Empleado.IdEmpleado == null)
            {
                mensaje = "Error al mover el Id del Empleado a la capa negocio";
                return null;
            }
            else if (registro)
                Empleado.IdEmpleado = null;

            return Empleado;
        }

        public static bool ImportarEmpleado(Empleados? Empleado, Contactos? contacto, ref string mensaje)
        {
            Empleado = ComprobarEmpleado(Empleado, true, ref mensaje);
            if (Empleado == null)
                return false;

            Personas? persona = PersonasNegocio.GetPersonaPorDni(Empleado.Personas.Dni, ref mensaje);
            if (persona == null)
                Empleado.IdPersona = PersonasNegocio.RegistrarPersona(Empleado.Personas, ref mensaje);
            else
            {
                Empleado.IdPersona = (int)persona.IdPersona;
                Empleado.Personas.IdPersona = persona.IdPersona;
                if (!PersonasNegocio.modificarPersona(Empleado.Personas, ref mensaje))
                    return false;

            }
            if (Empleado.IdPersona < 1)
                return false;

            Empleados? emp = null;
            emp = GetEmpleadoPorDni(Empleado.Personas.Dni, ref mensaje);
            Empleado.Personas = null;
            if (emp == null)
            {
                if (!RegistrarEmpleadoBasico(Empleado, ref mensaje))
                    return false;
            }
            if (contacto != null)
            {
                contacto.IdPersona = Empleado.IdPersona;
                if (!ContactosNegocio.registrarContacto(contacto, ref mensaje))
                    mensaje += "\nNo se pudo registrar contactos para el dni " + Empleado.Personas.Dni;
            }
            return true;
        }

        public static Empleados? GetEmpleadoPorDni(string? dni, ref string mensaje)
        {
            Personas? persona = null;
            persona = PersonasNegocio.GetPersonaPorDni(dni, ref mensaje);
            if (persona?.IdPersona == null)
                return null;
            
            Empleados? Empleado = EmpleadosDatos.GetEmpleadoPorIdPersona((int)persona.IdPersona, ref mensaje);
            if (Empleado == null)
                Empleado = new();
            Empleado.Personas = persona;
            Empleado.IdPersona = persona.IdPersona ?? 0;
            return Empleado;
        }

        public static List<Empleados>? GetEmpleadosPorDomicilio(string? calle, string? barrio, Localidades? localidad, bool incluirAnulados, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && localidad == null)
            {
                mensaje = "No se han enviado datos de búsqueda";
                return null;
            }
            List<Domicilios>? domicilios = DomiciliosDatos.GetDomiciliosPorCampos(calle, barrio, localidad, ref mensaje);
            if (domicilios == null)
            {
                return null;
            }
            List<int> IdDomicilios = domicilios.Select(d => (int)d.IdDomicilio).ToList();
            List<Empleados>? EmpleadosTodos = EmpleadosDatos.GetEmpleados(ref mensaje);
            if (EmpleadosTodos == null)
            {
                return null;
            }
            List<Empleados>? Empleados = EmpleadosTodos.Where(e => e.Personas != null && e.Personas.Domicilios != null &&
                                                        IdDomicilios.Contains((int)e.Personas.IdDomicilio)).ToList();
            return Empleados;
        }

        public static List<Empleados>? GetEmpleadosPorContactos(string? telefono, string? whatsapp, ref string mensaje)
        {
            if (string.IsNullOrWhiteSpace(telefono) && string.IsNullOrWhiteSpace(whatsapp))
            {
                mensaje = "No se han enviado datos de búsqueda";
                return null;
            }
            List<Contactos>? contactos = ContactosNegocio.getContactosPorNumero(telefono, whatsapp, ref mensaje);
            if (contactos == null)
            {
                return null;
            }
            List<int> IdPersonas = contactos.Where(e => e.IdPersona != null).Select(e => (int)e.IdPersona).ToList();
            List<Empleados>? EmpleadosTodos = EmpleadosDatos.GetEmpleados(ref mensaje);
            if (EmpleadosTodos == null)
            {
                return null;
            }
            List<Empleados>? Empleados = EmpleadosTodos.Where(e => e.Personas != null && IdPersonas.Contains((int)e.IdPersona)).ToList();
            return Empleados;
        }

        public static bool RegistrarEmpleado(Empleados? Empleado, List<Contactos>? contactos, ref string mensaje)
        {
            Empleado = ComprobarEmpleado(Empleado, true, ref mensaje);
            if (Empleado == null)
                return false;

            Personas? persona = PersonasNegocio.GetPersonaPorDni(Empleado.Personas.Dni, ref mensaje);
            if (persona != null)
            {
                Empleado.Personas.IdPersona = persona.IdPersona;
                if (persona.IdPersona == null)
                {
                    mensaje = "Problemas con el id de la persona en la BD";
                    return false;
                }
                if (!PersonasNegocio.modificarPersona(Empleado.Personas, ref mensaje))
                    return false;
                else
                {
                    Empleado.Personas = null;
                    Empleado.IdPersona = (int)persona.IdPersona;
                }
            }
            else
            {
                Empleado.IdPersona = PersonasNegocio.RegistrarPersona(Empleado.Personas, ref mensaje);
                persona = Empleado.Personas;
                persona.IdPersona = Empleado.IdPersona;
            }
            if (Empleado.IdPersona < 1)
                return false;

            Estados? estado = EstadosNegocio.getEstado("Empleados", "Activo", ref mensaje);
            if (estado == null)
                estado = new Estados { Indole = "Empleados", Estado = "Activo" };

            if (estado.IdEstado == null)
                Empleado.IdEstado = EstadosNegocio.registrarEstado(estado, ref mensaje);

            if (Empleado.IdEstado < 1)
                return false;

            Empleado.Estados = null;
            Empleado.Personas = null;

            int idEmpleado = 0;

            try
            {
                idEmpleado = EmpleadosDatos.RegistrarEmpleado(Empleado, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.GestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
                mensaje += mensajeContactos;

            return idEmpleado > 0;
        }

        public static bool RegistrarEmpleadoBasico(Empleados? Empleado, ref string mensaje)
        {
            Empleado = ComprobarEmpleado(Empleado, true, ref mensaje);
            if (Empleado == null)
                return false;

            Empleado.Estados = EstadosNegocio.getEstado("Empleados", "Activo", ref mensaje);
            if (Empleado.Estados != null)
                Empleado.IdEstado = Empleado.Estados.IdEstado ?? 0;

            if (Empleado.IdEstado < 1)
                Empleado.IdEstado = EstadosNegocio.registrarEstado(new Estados { Indole = "Empleados", Estado = "Activo" }, ref mensaje);

            if (Empleado.IdEstado < 1)
                return false;

            Empleado.Estados = null;
            Empleado.Personas = null;

            int idEmpleado = 0;

            try
            {
                idEmpleado = EmpleadosDatos.RegistrarEmpleado(Empleado, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = "Error inesperado en la capa de negocio " + ex.Message;
                return false;
            }
            return idEmpleado > 0;
        }

        public static bool modificarEmpleado(Empleados? Empleado, List<Contactos>? contactos, ref string mensaje)
        {
            Empleado = ComprobarEmpleado(Empleado, false, ref mensaje);

            if (Empleado == null)
                return false;

            if (Empleado.IdEstado < 1)
            {
                mensaje = "El estado del Empleado no se ha podido encontrar";
                return false;
            }

            Personas? persona = PersonasNegocio.GetPersonaPorDni(Empleado.Personas.Dni, ref mensaje);
            if (persona == null)
            {
                mensaje = "No se pudo encontrar información de la persona a modificar";
                return false;
            }
            if (persona.IdPersona == null)
            {
                mensaje = "Problemas con el id de la persona en la BD";
                return false;
            }

            Empleado.Personas.IdPersona = persona.IdPersona;

            if (!PersonasNegocio.modificarPersona(Empleado.Personas, ref mensaje))
            {
                return false;
            }
            Empleado.Personas = null;
            Empleado.IdPersona = (int)persona.IdPersona;
            Empleado.Estados = null;

            bool exito = false;

            try
            {
                exito = EmpleadosDatos.ModificarEmpleado(Empleado, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
            string mensajeContactos = "";
            if (!PersonasNegocio.GestionarContactosPorPersona(persona, contactos, ref mensajeContactos))
            {
                mensaje += mensajeContactos;
            }
            return exito;
        }
    }
}

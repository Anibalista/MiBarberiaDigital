using EF_SGBM;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public class ComprobacionContexto
    {
        private Contexto contexto;

        public ComprobacionContexto(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public bool Comprobar(ref string mensaje)
        {
            if (contexto == null)
            {
                mensaje = "No se conecta a la BD";
                return false;
            }
            return true;
        }

        public bool ComprobarCajas(ref string mensaje)
        {
            if (contexto.Cajas == null)
            {
                mensaje = "No se conecta al registro de Cajas";
                return false;
            }
            return true;
        }

        public bool ComprobarCategorias(ref string mensaje)
        {
            if (contexto.Categorias == null)
            {
                mensaje = "No se conecta al registro de Cajas";
                return false;
            }
            return true;
        }

        public bool ComprobarClientes(ref string mensaje)
        {
            if (contexto.Clientes == null)
            {
                mensaje = "No se conecta al registro de Clientes";
                return false;
            }
            return true;
        }

        public bool ComprobarContactos(ref string mensaje)
        {
            if (contexto.Contactos == null)
            {
                mensaje = "No se conecta al registro de Contactos";
                return false;
            }
            return true;
        }

        public bool ComprobarCredenciales(ref string mensaje)
        {
            if (contexto.Credenciales == null)
            {
                mensaje = "No se conecta al registro de Credenciales";
                return false;
            }
            return true;
        }

        public bool ComprobarCuotasMembresias(ref string mensaje)
        {
            if (contexto.CuotasMembresias == null)
            {
                mensaje = "No se conecta al registro de Cuotas de Membresias";
                return false;
            }
            return true;
        }

        public bool ComprobarDetallesFacturas(ref string mensaje)
        {
            if (contexto.DetallesFacturas == null)
            {
                mensaje = "No se conecta al registro de Detalles de Facturas";
                return false;
            }
            return true;
        }

        public bool ComprobarDetallesVentas(ref string mensaje)
        {
            if (contexto.DetallesVentas == null)
            {
                mensaje = "No se conecta al registro de Detalles de Ventas";
                return false;
            }
            return true;
        }

        public bool ComprobarDomicilios(ref string mensaje)
        {
            if (contexto.Domicilios == null)
            {
                mensaje = "No se conecta al registro de Domicilios";
                return false;
            }
            return true;
        }

        public bool ComprobarEmpleados(ref string mensaje)
        {
            if (contexto.Empleados == null)
            {
                mensaje = "No se conecta al registro de Empleados";
                return false;
            }
            return true;
        }

        public bool ComprobarEstados(ref string mensaje)
        {
            if (contexto.Estados == null)
            {
                mensaje = "No se conecta al registro de Estados";
                return false;
            }
            return true;
        }

        public bool ComprobarFacturas(ref string mensaje)
        {
            if (contexto.Facturas == null)
            {
                mensaje = "No se conecta al registro de Facturas";
                return false;
            }
            return true;
        }

        public bool ComprobarFondosMembresias(ref string mensaje)
        {
            if (contexto.FondosMembresias == null)
            {
                mensaje = "No se conecta al registro de Fondos de Membresias";
                return false;
            }
            return true;
        }

        public bool ComprobarInsumos(ref string mensaje)
        {
            if (contexto.Insumos == null)
            {
                mensaje = "No se conecta al registro de Insumos";
                return false;
            }
            return true;
        }

        public bool ComprobarLocalidades(ref string mensaje)
        {
            if (contexto.Localidades == null)
            {
                mensaje = "No se conecta al registro de Localidades";
                return false;
            }
            return true;
        }

        public bool ComprobarMediosPago(ref string mensaje)
        {
            if (contexto.MediosPagos == null)
            {
                mensaje = "No se conecta al registro de Medios de Pago";
                return false;
            }
            return true;
        }

        public bool ComprobarMembresias(ref string mensaje)
        {
            if (contexto.Membresias == null)
            {
                mensaje = "No se conecta al registro de Membresias";
                return false;
            }
            return true;
        }

        public bool ComprobarMembresiasServicios(ref string mensaje)
        {
            if (contexto.MembresiasServicios == null)
            {
                mensaje = "No se conecta al registro intermedio de Membresias-Servicios";
                return false;
            }
            return true;
        }

        public bool ComprobarNiveles(ref string mensaje)
        {
            if (contexto.Niveles == null)
            {
                mensaje = "No se conecta al registro de Niveles";
                return false;
            }
            return true;
        }

        public bool ComprobarPersonas(ref string mensaje)
        {
            if (contexto.Personas == null)
            {
                mensaje = "No se conecta al registro de Personas";
                return false;
            }
            return true;
        }

        public bool ComprobarProductos(ref string mensaje)
        {
            if (contexto.Productos == null)
            {
                mensaje = "No se conecta al registro de Productos";
                return false;
            }
            return true;
        }

        public bool ComprobarProveedores(ref string mensaje)
        {
            if (contexto.Proveedores == null)
            {
                mensaje = "No se conecta al registro de Proveedores";
                return false;
            }
            return true;
        }

        public bool ComprobarServicios(ref string mensaje)
        {
            if (contexto.Servicios == null)
            {
                mensaje = "No se conecta al registro de Servicios";
                return false;
            }
            return true;
        }

        public bool ComprobarTiposMembresias(ref string mensaje)
        {
            if (contexto.TiposMembresias == null)
            {
                mensaje = "No se conecta al registro de Tipos de Membresias";
                return false;
            }
            return true;
        }

        public bool ComprobarTiposTransacciones(ref string mensaje)
        {
            if (contexto.TiposTransacciones == null)
            {
                mensaje = "No se conecta al registro de Tipos de Transacción";
                return false;
            }
            return true;
        }

        public bool ComprobarTransacciones(ref string mensaje)
        {
            if (contexto.Transacciones == null)
            {
                mensaje = "No se conecta al registro de Transacciones";
                return false;
            }
            return true;
        }

        public bool ComprobarUsuarios(ref string mensaje)
        {
            if (contexto.Usuarios == null)
            {
                mensaje = "No se conecta al registro de Usuarios";
                return false;
            }
            return true;
        }
        public bool ComprobarUnidadesMedidas(ref string mensaje)
        {
            if (contexto.UnidadesMedidas == null)
            {
                mensaje = "No se conecta al registro de Unidades de Medida";
                return false;
            }
            return true;
        }

        public bool ComprobarVentas(ref string mensaje)
        {
            if (contexto.Ventas == null)
            {
                mensaje = "No se conecta al registro de Ventas";
                return false;
            }
            return true;
        }

        internal bool ComprobarServiciosInsumos(ref string mensaje)
        {
            if (contexto.ServiciosInsumos == null)
            {
                mensaje = "No se conecta al registro intermedio de insumos";
                return false;
            }
            if (contexto.Insumos == null)
            {
                mensaje = "No se conecta al registro de insumos";
                return false;
            }
            return true;
        }
    }
}

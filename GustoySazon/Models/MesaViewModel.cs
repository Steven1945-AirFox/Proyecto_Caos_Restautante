using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Models
{
    public class MesaViewModel
    {
        public int MesaId { get; set; }
        
        public int NumeroMesa { get; set; }
        public string EstadoAtencion { get; set; }

        public List<PedidoDetalle> Pedidos { get; set; }
        public List<Platillo> Menu { get; set; }
        public List<ClienteMesaInfo> ClientesEnMesa { get; set; }





        //Para el mesero
        public List<MeseroViewModel> InfoMesero { get; set; }


    }

    public class PedidoDetalle
    {
        public int MenuId { get; set; }
        public string NombreComida { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Estado { get; set; }
        public int Id { get; set; }
    }

    public class MeseroViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
        public string Correo { get; set; }
        public string Rol { get; set; }

        public string Turno { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public decimal NivelEstres { get; set; }
        public decimal NivelEnergia { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal PropinasTurno { get; set; }

        public decimal CambioEstresUltimaHora { get; set; }
        public decimal CambioEnergiaUltimaHora { get; set; }
        public decimal CambioEficienciaUltimaHora { get; set; }
        public decimal CambioPropinasUltimaHora { get; set; }
        public string EstadoActual { get; set; }
        public decimal HorasTrabajadas { get; set; }


        public class MeseroMesaViewModel
        {
            // Propiedades del mesero
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Rol { get; set; }
            public string Turno { get; set; }
            public TimeSpan HoraInicio { get; set; }
            public TimeSpan HoraFin { get; set; }
            public decimal NivelEstres { get; set; }
            public decimal NivelEnergia { get; set; }
            public decimal Eficiencia { get; set; }
            public decimal PropinasTurno { get; set; }

            // Propiedades de las mesas
            public List<MesaInfo> Mesas { get; set; }
            public int MesaSeleccionadaId { get; set; }


            public MeseroMesaViewModel()
            {
                Mesas = new List<MesaInfo>();
            }
        }

        public class MesaInfo
        {
            public int MesaId { get; set; }
            public int NumeroMesa { get; set; }
            public string Ubicacion { get; set; }
            public string EstadoAtencion { get; set; }
            public int Sillas { get; set; }
            public int Ocupantes { get; set; }
            public List<PedidoDetalle> Pedidos { get; set; }
            public List<ClienteMesaInfo> ClientesEnMesa { get; set; }

            public MesaInfo()
            {
                Pedidos = new List<PedidoDetalle>();
                ClientesEnMesa = new List<ClienteMesaInfo>();
            }
        }


    }


}





public class Platillo
{
    public int Id { get; set; }
    public string NombreComida { get; set; }
    public decimal Precio { get; set; }
    public string Tipo { get; set; }
    public string Estado { get; set; }
    public string ImagenUrl { get; set; }
}

public class ClienteMesaInfo
{
    public string Nombre { get; set; }
    public bool TienePedido { get; set; }
    public string EstadoPedido { get; set; }

}



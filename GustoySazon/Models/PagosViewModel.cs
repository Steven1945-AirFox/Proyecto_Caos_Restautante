using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Models
{
    public class PagosViewModel
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int MesaId { get; set; }
        public int NumeroMesa { get; set; }

        public List<ItemOrden> ItemsOrden { get; set; } = new List<ItemOrden>();


        public decimal Subtotal { get; set; }


        public decimal IVA { get; set; }

        public decimal Propina { get; set; }


        //Aqui se obtiene y se envia el total de las comidas
        public decimal Total { get; set; }

        public List<TarjetaViewModel> Tarjetas { get; set; } = new List<TarjetaViewModel>();

        public int TarjetaSeleccionadaId { get; set; }

        public TipoPropina TipoPropina { get; set; } = TipoPropina.Porcentaje15;


        public decimal? PropinaPersonalizada { get; set; }

        public int Calificacion { get; set; }
        public string Comentario { get; set; }





        public bool EsPagoGrupal { get; set; }
        public List<ItemOrden> ItemsMesa { get; set; }
        public List<int> UsuariosMesa { get; set; }





        public List<ItemFactura> ItemsFactura { get; set; }


    }

    public class ItemOrden
    {
        public int OrdenId { get; set; }
        public int MenuId { get; set; }
        public string NombreComida { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal IVAUnitario => PrecioUnitario * 0.13m;
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public decimal IVATotal => IVAUnitario * Cantidad;

        public int UsuarioId { get; set; }
    }

    public class TarjetaViewModel
    {
        public int Id { get; set; }
        public string TipoTarjeta { get; set; }
        public string NumeroTarjeta { get; set; }
        public string NumeroTarjetaEnmascarado { get; set; }
        public string FechaVencimiento { get; set; }
        public string NombreTitular { get; set; }
    }

    public class ResenaViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5 estrellas.")]
        public int Calificacion { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        public string ResenaTexto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }



    public class ResenaRequest
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
    }








    public class ItemFactura
    {
        public string NombreComida { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IVA { get; set; }
        public decimal Subtotal { get; set; }
        public string NombreCliente { get; set; }
    }

 











    public enum TipoPropina
    {
        Porcentaje5 = 5,
        Porcentaje10 = 10,
        Porcentaje15 = 15,
        Porcentaje20 = 20,
        Personalizada = -1
    }
}

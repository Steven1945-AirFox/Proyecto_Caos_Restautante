using System;

namespace GustoySazon.Models
{
    public class OrdenModel
    {
        public int Mesa { get; set; }
        public string Nombre { get; set; }
        public string Instrucciones { get; set; }
        public int TiempoEstimado { get; set; }
        public int Id { get; set; }
        public int GrupoOrdenId { get; set; }
        public int UsuarioId { get; set; }
        public int MesaId { get; set; }
        public int MenuId { get; set; }
        public string NombreComida { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int? MeseroId { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace GustoySazon.Models
{
    public class FilaEsperaViewModel
    {
        public List<ClienteEnEspera> ClientesEnEspera { get; set; }
        public List<MesaDisponible> MesasDisponibles { get; set; }

        
        public int UsuarioIdActual { get; set; }
    }

    public class ClienteEnEspera
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime HoraRegistro { get; set; }
    }

    public class MesaDisponible
    {
        public int Id { get; set; }
        public int NumeroMesa { get; set; }
        public int Sillas { get; set; }
        public string Ubicacion { get; set; }
        public string Estado { get; set; }
        public int Ocupantes { get; set; }
    }
}

using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Hubs
{
    public class SolucionHub : Hub
    {
        // Envía las soluciones del evento a todos los clientes
        public void EnviarSoluciones(string categoria, string eventoId, string titulo, List<SolucionModel> soluciones)
        {
            Clients.All.MostrarSoluciones(categoria, eventoId, titulo, soluciones);
        }

        // Notifica que se eligió una solución
        public void ConfirmarSolucion(string eventoId, string severidad)
        {
            Clients.All.SolucionSeleccionada(eventoId, severidad);
        }
    }

    public class SolucionModel
    {
        public string severity { get; set; }
        public string text { get; set; }
    }
}
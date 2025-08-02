using GustoySazon.DAL;
using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.BLL
{
    public class EmpleadoBLL
    {
        private EmpleadoDAL dal = new EmpleadoDAL();

        public void RegistrarEmpleado(EmpleadoModel emp)
        {
            dal.InsertarEmpleado(emp);
        }

        public List<EmpleadoModel> ObtenerEmpleados()
        {
            return dal.ObtenerEmpleados();
        }

        public void ActualizarEmpleado(EmpleadoModel emp)
        {
            dal.ActualizarEmpleado(emp);
        }

        public void EliminarEmpleado(int id)
        {
            dal.EliminarEmpleado(id);
        }
    }
}


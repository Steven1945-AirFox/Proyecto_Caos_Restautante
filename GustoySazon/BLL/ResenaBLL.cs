using GustoySazon.DAL;
using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.BLL
{
    public class ResenaBLL
    {
        private ResenaDAL dal = new ResenaDAL();

        public List<ResenaModel> ObtenerResenas()
        {
            return dal.ObtenerResenas();
        }
    }
}
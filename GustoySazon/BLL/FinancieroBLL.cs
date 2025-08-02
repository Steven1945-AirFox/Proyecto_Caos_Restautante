using GustoySazon.DAL;
using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.BLL
{
    public class FinancieroBLL
    {
        private FinancieroDAL dal = new FinancieroDAL();

        public ResumenPagoModel ObtenerUltimaVenta()
        {
            return dal.ObtenerUltimaVentaRegistrada();
        }

    }
}
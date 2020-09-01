using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace Molinos.DataAgro.Business.Managers
{
    public class HttpContextManager : IHttpContextManager
    {
       
        public HttpContextManager()
        {
           
        }

        public string ObtenerPathLogoMail()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png");
        }
    }
}

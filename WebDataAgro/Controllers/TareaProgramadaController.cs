using Microsoft.Win32.TaskScheduler;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class TareaProgramadaController : Controller
    {
        private IComercialManager mobjComercialManager;

        public TareaProgramadaController(IComercialManager oComercialManager)
        {
            mobjComercialManager = oComercialManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionTareasProgramadas)]
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Buscar()
        {
            var model = new TareaProgramadaModel();

            using (var ts = new TaskService())
            {
                
                var tasks = ObtenerCarpetaDeTasks(ts).EnumerateTasks();
                model.Datos = tasks.Select(x => new TaskModel
                {
                    Name = x.Name,
                    LastRunTime = x.LastRunTime,
                    NextRunTime = x.NextRunTime,
                    RepeticionEnMinutos = x.Definition.Triggers.Any() ? (int)x.Definition.Triggers.First().Repetition.Interval.TotalMinutes : 0,
                    Action = x.Definition.Actions.Any() ? Regex.Match(((ExecAction)x.Definition.Actions.First()).Arguments,
                        @"-command {Invoke-WebRequest (.+) -UseDefaultCredential", RegexOptions.Singleline).Groups[1].Value : ""
                }).ToList();
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(TaskModel model)
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    // Create a new task definition and assign properties
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = model.Name;

                    //td.Principal.UserId = "NT AUTHORITY\\LOCALSERVICE";
                    //td.Principal.LogonType = TaskLogonType.ServiceAccount;
                    //String user = ConfigurationManager.AppSettings["ServiceUser"];
                    //String pass = ConfigurationManager.AppSettings["ServicePass"];
                    // Create a trigger that will fire the task at this time every other day
                    td.Triggers.Add(new DailyTrigger { Repetition = new RepetitionPattern(new TimeSpan(0, model.RepeticionEnMinutos, 0), TimeSpan.Zero), StartBoundary = model.Inicio });

                    // Create an action that will launch Notepad whenever the trigger fires
                    td.Actions.Add(new ExecAction("powershell.exe", $"powershell.exe -command {{Invoke-WebRequest {model.Action} -UseDefaultCredential}}", null));
                    //td.Principal.UserId = user;
                    //td.Principal.LogonType = TaskLogonType.Password;
                    ObtenerCarpetaDeTasks(ts).RegisterTaskDefinition(model.Name, td);
                    //, TaskCreation.Create, user, pass);
                }
            }
            catch (Exception e)
            {
                model.Errores.Add(new ErrorMessage("Ocurrió un error al intentar guardar la tarea: " + e.Message));
            }
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(TaskModel model)
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    ObtenerCarpetaDeTasks(ts).DeleteTask(model.Name);
                }
            }
            catch (Exception e)
            {
                model.Errores.Add(new ErrorMessage("Ocurrió un error al intentar guardar la tarea: " + e.Message));
            }
            
            return new JsonResult()
            {
                Data =model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmRangoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        private TaskFolder ObtenerCarpetaDeTasks(TaskService ts)
        {
            if(!ts.RootFolder.SubFolders.Any(x => x.Name == "DataAgro"))
            {
                ts.RootFolder.CreateFolder("DataAgro");
            }
            return ts.RootFolder.SubFolders["DataAgro"];
        }
    }
}
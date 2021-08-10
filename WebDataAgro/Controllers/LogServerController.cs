using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace WebDataAgro.Controllers
{

    public class LogServerController : Controller
    {
        private readonly string _logDir = @"C:\DataAgroLogs";

        public ActionResult Index(string log)
        {
            if (!string.IsNullOrEmpty(log))
            {
                return File(Path.Combine(_logDir, log), "text/plain");
            }

            var logs = Directory.GetFiles(_logDir)
                .Where(path => path.EndsWith(".log"))
                .Select(path => new FileInfo(path))
                .Select(file => new LogFileModel
                {
                    Name = file.Name,
                    LastWriteTime = file.LastWriteTime,
                    Url = "/logs/" + file.Name,
                    Size = GetFriendlyFileSize(file.Length)
                })
                .OrderByDescending(x => x.LastWriteTime)
                .ToList();

            //CorrectSortOrder(logs);

            return View(logs);
        }

        public ActionResult Delete(string log)
        {
            var logPath = Path.Combine(_logDir, log);
            System.IO.File.Delete(logPath);
            return RedirectToAction("Index");
        }

        private string GetFriendlyFileSize(long lengthInBytes)
        {
            var kb = Math.Round(lengthInBytes / 1024d);
            var groupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
            var friendly = kb.ToString("N0").Replace(groupSeparator, " ") + " KB";
            return friendly;
        }

        private void CorrectSortOrder(List<LogFileModel> logs)
        {
            if (logs.Any())
            {
                var latestLog = logs.First();
                logs.Remove(latestLog);
                logs.Reverse();
                logs.Insert(0, latestLog);
            }
        }

        public ActionResult ViewLog(string log)
        {
            string text = "";
            string textFile = Path.Combine(_logDir, log);
            using (var fs = new FileStream(textFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    text += line + "<br />";
                }
            }
            
            return File(Path.Combine(_logDir, log), "text/plain");

        }
    }
}
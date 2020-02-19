using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebDataAgro.Helpers.Excel
{
    public abstract class ExcelResultBase : ActionResult
    {
        protected readonly string FileName;

        protected ExcelResultBase(string fileName)
        {
            this.FileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ClearContent();
            response.ClearHeaders();
            response.Cache.SetMaxAge(new TimeSpan(0));

            using (var stream = new MemoryStream())
            {
                this.CreateFile(stream);

                //Return it to the client - strFile has been updated, so return it. 
                response.AddHeader("content-disposition", string.Format("attachment; filename=\"{0}\"", this.FileName));

                // see http://filext.com/faq/office_mime_types.php
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.ContentEncoding = Encoding.UTF8;
                stream.WriteTo(response.OutputStream);
            }

            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        
        protected abstract void CreateFile(MemoryStream stream);
    }
}

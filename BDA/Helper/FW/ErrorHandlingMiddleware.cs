using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace BDA.Helper
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, DataModel.DataEntities db/* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, db, context);
            }
        }

        private static async Task HandleExceptionAsync(Exception exception, DataModel.DataEntities db, HttpContext context)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            //TODO : beresin unathorized dan notfound
          
            db.LogError(exception, null);

            var msg = "Terjadi Kesalahan. Silahkan klik back dan coba kembali";

            if(exception is InvalidOperationException)
            {
                msg = exception.Message;
            }else //if (db.Env.IsDevelopment()) //TODO : remove comment
            {
                msg = exception.ToString();
            }
            if (exception.Message.Contains("[Hortonworks]") && exception.Message.Contains("ERROR")) {
                msg = "Terjadi kesalahan koneksi ke Hive. Silahkan hubungi admin.";
            }
            
            if (msg.StartsWith("Anda tidak memiliki izin untuk mengakses modul ini."))
            {
                context.Response.Redirect("/Home/Error");              
            }
            else
            {
                var result = JsonConvert.SerializeObject(new { error = msg });
                db.HttpContext.Response.ContentType = "application/json";
                db.HttpContext.Response.StatusCode = (int)code;
                await db.HttpContext.Response.WriteAsync(result);
            }                       
        }

    }

    static class Extensions
    {

        public static string GetFullErrorMessage(this ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }

    }
}

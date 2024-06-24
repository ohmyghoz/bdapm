using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BDA.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
namespace BDA.Controllers
{
    [AllowAnonymous]
    [Area("Website")]
    public class ImageController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public ImageController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index(string id)
        {            
            var byteList = new ImageHelper(db).GetImage(id);
            if(byteList != null)
            {
                return File(byteList, "image/png");
            }
            else
            {
                return null;
            }            
        }        
    }

    
}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BDA.DataModel;
using System;
using System.IO;
using BDA.Helper;

namespace BDA.Controllers
{
    [Area("FW")]
    public class AttachmentController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public AttachmentController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Sample()
        {
            if (HttpContext.User.Identity.Name != "admin") throw new InvalidOperationException("development only");
            return View();
        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions, string token)
        {
            var mdl = db.DecryptObjId(token);

            var query = (from q in db.FWAttachment
                         where q.Stsrc == "A" && q.AttachObjId == mdl.obj_id
                         select new AttachmentGridData() { attach_file_nama = q.AttachFileNama, attach_file_size = q.AttachFileSize, attach_id = q.AttachId }
                        ).ToList();
            foreach (var row in query)
            {
                row.attach_token = db.EncryptAttId(row.attach_id);
                if (!mdl.isReadOnly)
                {
                    row.attach_delete_token = db.EncryptAttIdForDelete(row.attach_id); // delete token cuma di supply kalau tidak readonly
                }
                row.ControlID = mdl.ControlID;
            }
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpPost]
        public IActionResult Upload(string token)
        {
            var mdl = db.DecryptObjId(token);
            if (mdl.isReadOnly)
            {
                throw new InvalidOperationException("Upload disabled because control is readonly.");
            }

            var myFile = Request.Form.Files["attUpload"];
            using (var memStream = new MemoryStream())
            {
                myFile.CopyTo(memStream);
                var att = db.SaveAttachment(mdl.obj_id, myFile.FileName, memStream.ToArray());
                db.FWAttachment.Add(att);
                db.SaveChanges();
            }
            return new EmptyResult();
        }
        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "token" })]
        public IActionResult AttDownload(string token, string attToken)
        {
            var mdl = db.DecryptObjId(token);            
            long attId = db.DecryptAttId(attToken);
            var attach_id = Convert.ToInt64(attId);
            db.GetFileFromAttachment(attToken);
            var oldAtt = db.FWAttachment.Where(x => x.Stsrc == "A" && x.AttachObjId == mdl.obj_id && x.AttachId == attach_id).FirstOrDefault();
            if (oldAtt != null)
            {
                Stream outputStream = db.GetFileFromAttachment(oldAtt);
                string fileName = oldAtt.AttachFileNama;
                if (outputStream != null)
                {
                    outputStream.Seek(0, 0);
                    return File(outputStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "token" })]
        public IActionResult AttDelete(string token, string attDeleteToken)
        {
            var mdl = db.DecryptObjId(token);
            if (mdl.isReadOnly)
            {
                throw new InvalidOperationException("Upload disabled because control is readonly.");
            }
            long attId = db.DecryptAttIdForDelete(attDeleteToken); //delete ada tambahan validasi nya
            var deleteId = Convert.ToInt64(attId);            
            var oldAtt = db.FWAttachment.Where(x => x.Stsrc == "A" && x.AttachObjId == mdl.obj_id && x.AttachId == deleteId).FirstOrDefault();
            if (oldAtt != null)
            {
                db.DeleteStsrc(oldAtt);
                db.SaveChanges();
            }
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);
        }
    }
    public class AttachmentGridData
    {
        public string attach_file_nama { get; set; }
        public int attach_file_size { get; set; }
        public string attach_token { get; set; }
        public string attach_delete_token { get; set; }
        public long attach_id { get; set; }
        public string ControlID{ get; set; }

    }
}
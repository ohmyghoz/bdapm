using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    public class TempTableHelper <T>
    {
        private DataEntities db;        
        public TempTableHelper(DataEntities db)
        {
            this.db = db;            
        }

        public T GetContentAsObject(Guid id)
        {
            var obj = db.FWTempTable.Find(id);
            if(obj != null)
            {
                var returnObj = JsonConvert.DeserializeObject<T>(obj.TempContent);
                return returnObj;
            }
            else
            {
                return default(T);
            }            
        }

        public int? UpdateContent(Guid id, T obj)
        {
            FWTempTable ent = db.FWTempTable.Find(id);
            if (ent != null)
            {
                string content = JsonConvert.SerializeObject(obj);
                ent.TempContent = content;
                ent.UpdatedDatetime = DateTime.Now;
                return db.SaveChanges();
            }
            return null;
        }

        public Guid CreateContent(T obj)
        {
            string content = JsonConvert.SerializeObject(obj);           
            
            FWTempTable ent = new FWTempTable();
            ent.TempContent = content;            
            ent.TempId = Guid.NewGuid();
            ent.UserId = db.HttpContext.User.Identity.Name;
            if (ent.UserId == null) ent.UserId = db.HttpContext.Request.Host.Host;
            ent.CreatedDatetime = DateTime.Now;
            db.FWTempTable.Add(ent);
            db.SaveChanges();
            return ent.TempId;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BDA.DataModel
{
    public class ImageHelper
    {
        private DataEntities db;
        public ImageHelper(DataEntities db)
        {
            this.db = db;
        }

        public string SaveImage(byte[] fileContent,  int width, int height)
        {

            string folder = db.GetSetting("ImagePath") + "\\";
            System.IO.Directory.CreateDirectory(folder);

            var id = Guid.NewGuid().ToString().Replace("-","");
            float ratio = (float) width / height;
            string newImagepath = folder + id + ".jpg";
            var tolleranceRatio = 0.1;
           

            using (Image image = Image.Load(fileContent))
            {

                var imgRatio = (float)image.Width / image.Height;

                if(Math.Abs(imgRatio - ratio) > tolleranceRatio){
                    throw new InvalidOperationException(String.Format("Ratio gambar (width/height) harus sekitar {0:n2} - {1:n2}", ratio - tolleranceRatio, ratio + tolleranceRatio));
                }

                image.Mutate(x => x
                     .Resize(width, height));
                image.Save(newImagepath); // Automatic encoder selected based on extension.
            }
            return id;           
        }

        public byte[] GetImage(string id)
        {
            string folder = db.GetSetting("ImagePath") + "\\";
            string newImagepath = folder + id + ".jpg";
            if (File.Exists(newImagepath))
            {
                return File.ReadAllBytes(newImagepath);
            }
            else
            {
                return null;
            }
            
        }

        public static string GetLink(string id)
        {
            return "~/Image/Index/" + id;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIUnlocked___backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost]
        [Route("postWebcamImages")]
        public void PostWebcamImages([FromBody]List<string> images)
        {
            // clear directory from old images 
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "./Storage/UserWebcam/"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            for (int i = 0; i < images.Count; i++)
            {
                string image = images[i];

                var base64Data = Regex.Match(image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                string filepath = Path.Combine(Environment.CurrentDirectory, "./Storage/UserWebcam/userimage" + i.ToString() + ".jpg");

                Image imageJpg;
                using (MemoryStream ms = new MemoryStream(binData))
                {
                    imageJpg = Image.FromStream(ms);
                    imageJpg.Save(filepath);
                }
            }
        }

        [HttpPost]
        [Route("postUserClasses")]
        public void PostUserClasses([FromBody]DTOs.UserClassesDto classesDto)
        {

        }

        [HttpGet]
        [Route("getImagesOfClass")]
        public List<string> GetImagesOfClass(string objectClass)
        {
            string dirPath = Path.Combine(Environment.CurrentDirectory, "Storage\\OwnDatabase\\" + objectClass + "\\train_data");
            DirectoryInfo d = new DirectoryInfo(dirPath);

            var Files = new FileInfo[0];
            if (System.IO.Directory.Exists(dirPath))
                 Files = d.GetFiles("*.jpg"); //Getting Text files

            var filelist = new List<string>();

            int limit = 50;
            int counter = 0;

            foreach (FileInfo file in Files)
            {
                if (counter == limit) break;
                else counter++;

                byte[] newImage = System.IO.File.ReadAllBytes(Path.Combine(dirPath, file.Name));
                filelist.Add(Convert.ToBase64String(newImage));
            }

            return filelist;
        }
    }
}

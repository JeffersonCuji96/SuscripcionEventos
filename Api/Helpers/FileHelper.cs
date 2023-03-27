using BL.DTO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Helpers
{
    public class FileHelper
    {
        public static string UploadImage(string? imageBase64,bool tipo)
        {
            var ruta = tipo == true ? "/Images/Events/" : "/Images/Users/";
            if (!string.IsNullOrEmpty(imageBase64))
            {
                var nameFile = Guid.NewGuid().ToString();
                var stream = new MemoryStream(Convert.FromBase64String(imageBase64));
                IFormFile file = new FormFile(stream, 0, stream.Length, nameFile, nameFile);
                ruta = $"{ruta + nameFile}" + ".png";
                using var fileStream = new FileStream("wwwroot" + ruta, FileMode.Create);
                file.CopyTo(fileStream);
                return ruta;
            }
            return $"{ruta}no-photo.png";
        }
        public static void RemoveImage(string path)
        {
            string imgDefault=path.Substring(path.Length-12,12);
            if (!string.IsNullOrEmpty(path) && imgDefault!="no-photo.png")
                File.Delete(path);
        }
    }
}

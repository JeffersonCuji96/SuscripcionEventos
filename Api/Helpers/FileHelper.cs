using BL.DTO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Se define dos carpetas de la ruta del servidor, en caso de que la foto a guardar sea de un 
        /// evento o usuario se procede a guardar en la ubicación que corresponde. El nombre de la imagen
        /// se genera con un identificador unico para el nombre de la imagen y se establece el formato png
        /// </summary>
        /// <param name="imageBase64"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Se obtiene el nombre de la imagen a través de la ruta, en caso de haber una foto que no sea
        /// la predeterminada que representa que no hay una imagen, se procede a eliminar
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveImage(string path)
        {
            string imgDefault=path.Substring(path.Length-12,12);
            if (!string.IsNullOrEmpty(path) && imgDefault!="no-photo.png")
                File.Delete(path);
        }
    }
}

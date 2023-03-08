namespace Api.Helpers
{
    public class FileHelper
    {
        public static string UploadImage(string imageBase64)
        {
            var nameFile = Guid.NewGuid().ToString();
            var stream = new MemoryStream(Convert.FromBase64String(imageBase64));
            IFormFile file = new FormFile(stream, 0, stream.Length, nameFile, nameFile);
            var ruta = $"wwwroot/Images/{nameFile}" + ".png";
            using var fileStream = new FileStream(ruta, FileMode.Create);
            file.CopyTo(fileStream);
            return ruta;
        }
        public static void RemoveImage(string path)
        {
            if (!string.IsNullOrEmpty(path))
                File.Delete(path);
        }
    }
}

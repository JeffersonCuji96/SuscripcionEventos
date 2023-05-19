using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsAvailablePhotoAttribute : ValidationAttribute
    {
        public readonly long _sizeImage;
        public IsAvailablePhotoAttribute(long sizeImage)
        {
            _sizeImage = sizeImage;
        }

        /// <summary>
        /// Método para validar el peso y formato de una imagen
        /// </summary>
        /// <remarks>
        /// Se valida que la imagen solo tenga formatos jpg y png y sea un archivo válido
        /// de tipo imagen. Y el tamaño se valida según la cantidad que se especifique en 
        /// el DTO donde se realice esta validación
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            string? imageBase64 = value?.ToString();
            if (!string.IsNullOrEmpty(imageBase64))
            {
                Span<byte> buffer = new(new byte[imageBase64.Length]);
                bool checkBase64 = Convert.TryFromBase64String(imageBase64, buffer, out int imageBytes);
                if (checkBase64)
                {
                    try
                    {
                        using var ms = new MemoryStream(buffer.ToArray(), 0, imageBytes);
                        Image image = Image.FromStream(ms, true);
                        if (image.RawFormat.ToString() == "Jpeg" || image.RawFormat.ToString() == "Png")
                        {
                            if (imageBytes > _sizeImage)
                            {
                                ErrorMessage = $"El peso de la imagen no debe ser superior a {_sizeImage / _sizeImage} MB";
                                return false;
                            }
                            return true;
                        }
                        ErrorMessage = "Se permiten sólo formatos png y jpg";
                    }
                    catch (ArgumentException)
                    {
                        ErrorMessage = "Formato de imagen no reconocido";
                    }
                    return false;
                }
                ErrorMessage = "La imagen procesada no es un archivo válido";
                return false;
            }
            return true;
        }
    }
}

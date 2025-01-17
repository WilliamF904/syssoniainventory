using System.Drawing.Imaging;
using System.Drawing;

namespace SysSoniaInventory.Task
{
    public class ImgSave
    {
        public string GuardarImagen(IFormFile file, int id, string name) // Método para guardar la imagen
        {
            if (file != null && file.Length > 0)
            {
                // Verificar el tipo de contenido
                string contentType = file.ContentType;
                if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
                {
                    throw new InvalidOperationException("Solo se permiten archivos de imagen.");
                }

                // Normalizar el nombre (reemplazar caracteres no válidos y proporcionar un valor predeterminado)
                string nombreNormalizado = !string.IsNullOrEmpty(name) ? name.Trim().Replace(" ", "_") : "sin_nombre";

                // Limitar el nombre a un máximo de 75 caracteres
                if (nombreNormalizado.Length > 75)
                {
                    nombreNormalizado = nombreNormalizado.Substring(0, 75);
                }

                // Crear el nombre del archivo con el formato id_nombre.png
                string nombreArchivo = $"{id}_{nombreNormalizado}.png";
                string rutaDirectorio = Path.Combine("wwwroot", "img");

                // Crear el directorio si no existe
                if (!Directory.Exists(rutaDirectorio))
                {
                    Directory.CreateDirectory(rutaDirectorio);
                }

                // Ruta completa del archivo
                string rutaCompleta = Path.Combine(rutaDirectorio, nombreArchivo);

                // Si el archivo ya existe, se reemplaza
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }

                // Convertir la imagen a PNG y guardarla
                using (var stream = file.OpenReadStream())
                {
                    using (var image = Image.FromStream(stream))
                    {
                        image.Save(rutaCompleta, ImageFormat.Png);
                    }
                }

                
                // Cambia esto en la función GuardarImagen
                return $"/img/{nombreArchivo}";

            }

            return null;
        }
    }
}

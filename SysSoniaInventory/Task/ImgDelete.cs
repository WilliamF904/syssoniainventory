namespace SysSoniaInventory.Task
{
    public class ImgDelete
    {
        public bool EliminarImagenPorUrl(string url) // Método para eliminar la imagen usando la URL
        {
            // Validar que la URL no sea nula o vacía
            if (string.IsNullOrEmpty(url))
            {
                return false; // No se puede eliminar si no hay una URL válida
            }

            // Convertir la URL relativa en una ruta completa
            string rutaDirectorio = Path.Combine("wwwroot");
            string rutaCompleta = Path.Combine(rutaDirectorio, url.TrimStart('/')); // Quita el '/' inicial de la URL

            // Verificar si el archivo existe antes de intentar eliminarlo
            if (File.Exists(rutaCompleta))
            {
                File.Delete(rutaCompleta); // Eliminar el archivo
                return true; // Indica que se eliminó exitosamente
            }

            return false; // Indica que el archivo no existía
        }
    }
}

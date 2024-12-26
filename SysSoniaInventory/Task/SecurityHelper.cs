using System.Security.Cryptography;
using System.Text;

namespace SysSoniaInventory.Task
{
    public class SecurityHelper
    { /// <summary>
      /// Encripta una cadena de texto utilizando SHA-256, incluyendo una llave secreta.
      /// </summary>
      /// <param name="input">Texto a encriptar.</param>
      /// <param name="secretKey">Llave secreta para fortalecer la encriptación.</param>
      /// <returns>Texto encriptado en formato hexadecimal.</returns>
        public static string EncryptSHA256(string input, string secretKey)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("El texto a encriptar no puede estar vacío.", nameof(input));

            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentException("La llave secreta no puede estar vacía.", nameof(secretKey));

            // Concatenar la entrada con la llave secreta
            string combinedInput = input + secretKey;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(combinedInput);
                byte[] hash = sha256.ComputeHash(bytes);

                // Convertir el hash a una cadena hexadecimal
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Direct_Barber.Recursos
{
    public class Utilidades
    {
        public static string EncriptarContra(string contrasena)
        {

            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] resultado = hash.ComputeHash(enc.GetBytes(contrasena));

                foreach (byte b in resultado)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string UploadedFile(IFormFile imagenFile, IWebHostEnvironment webHostEnvironment)
        {
            string fileName = null;
            if (imagenFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + imagenFile.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imagenFile.CopyTo(fileStream);
                }
            }
            return fileName;
        }
    }

}

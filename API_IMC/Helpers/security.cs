using System;
using System.Text;
using Konscious.Security.Cryptography;

namespace API_IMC.Helpers
{
    public class security
    {
        public static string HashPassword(string password)
        {
            // Crear un nuevo objeto Argon2id
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            // Configurar los parámetros de la función de hashing
            argon2.Salt = Encoding.UTF8.GetBytes(password);
            argon2.DegreeOfParallelism = 4;
            argon2.Iterations = 2;
            argon2.MemorySize = 512 * 512;

            // Realizar el hashing de la contraseña
            byte[] hashBytes = argon2.GetBytes(32);

            // Convertir los bytes del hash a una cadena base64 para almacenamiento
            string hashString = Convert.ToBase64String(hashBytes);

            // Retornar la cadena hash
            return hashString;
        }
    }
}
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Cryptography;
// using System.Text;
// using System.Threading.Tasks;

// namespace backend.Payment.Momo.Helper
// {
//     public static class MoMoHelper
//     {
//         private static string GenerateSignature(string text, string key)
//         {
//             // change according to your needs, an UTF8Encoding
//             // could be more suitable in certain situations
//             ASCIIEncoding encoding = new ASCIIEncoding();

//             byte[] textBytes = encoding.GetBytes(text);
//             byte[] keyBytes = encoding.GetBytes(key);

//             byte[] hashBytes;

//             using (HMACSHA256 hash = new HMACSHA256(keyBytes))
//                 hashBytes = hash.ComputeHash(textBytes);

//             return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
//         }
//     }
// }
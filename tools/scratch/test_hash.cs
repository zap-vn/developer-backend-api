using System;
using System.Security.Cryptography;
using System.Text;

string password = "0";

// 1. MD5
using (var md5 = MD5.Create()) {
    byte[] bytes = Encoding.UTF8.GetBytes(password);
    byte[] hash = md5.ComputeHash(bytes);
    var sb = new StringBuilder();
    foreach (byte b in hash) sb.Append(b.ToString("x2").ToLower());
    string md5Hash = sb.ToString();
    Console.WriteLine($"MD5: {md5Hash}");

    // 2. Salted SHA256
    string salt = "admin@backend.api.vn";
    using (var sha256 = SHA256.Create()) {
        byte[] bytes2 = Encoding.UTF8.GetBytes(md5Hash + salt);
        byte[] hash2 = sha256.ComputeHash(bytes2);
        string final = Convert.ToBase64String(hash2);
        Console.WriteLine($"Final: {final}");
    }
}

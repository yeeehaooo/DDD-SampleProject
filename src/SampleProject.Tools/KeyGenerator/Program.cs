using System.Security.Cryptography;

var keyBytes = new byte[32];
var ivBytes = new byte[16];

RandomNumberGenerator.Fill(keyBytes);
RandomNumberGenerator.Fill(ivBytes);

var key = Convert.ToBase64String(keyBytes);
var iv = Convert.ToBase64String(ivBytes);

Console.WriteLine("Key: " + key);
Console.WriteLine("IV: " + iv);

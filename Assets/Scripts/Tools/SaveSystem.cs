using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    public static bool Exists(string fileName, string folder = "")
    {
        string path = Application.persistentDataPath;
        if (!string.IsNullOrEmpty(folder))
            path = Path.Combine(path, folder);
        path = Path.Combine(path, string.Concat(fileName, ".txt"));
        return File.Exists(path);
    }

    public static bool RemoveFile(string fileName, string folder = "")
    {
        string path = Application.persistentDataPath;
        if (!string.IsNullOrEmpty(folder))
            path = Path.Combine(path, folder);
        path = Path.Combine(path, string.Concat(fileName, ".txt"));
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public static void Save<T>(T data, string fileName, string folder = "", string passWord = "")
    {
        string dataStr = JsonConvert.SerializeObject(data);
        WriteString(fileName, dataStr, passWord, folder);
    }

    public static T Load<T>(string fileName, string folder = "", string passWord = "")
    {
        string dataStr = ReadString(fileName, passWord, folder);
        if (string.IsNullOrEmpty(dataStr))
            return default;
        else
        {
            T data = JsonConvert.DeserializeObject<T>(dataStr);
            return data;
        }
    }

    public static void WriteString(string fileName, string valueReplace, string passPhrase, string folder = "")
    {
        string path = Application.persistentDataPath;
        if (!string.IsNullOrEmpty(folder))
        {
            path = Path.Combine(path, folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, string.Concat(fileName, ".txt"));
        //Debug.Log("path: " + path);
        valueReplace = Encrypt(valueReplace, passPhrase);
        File.WriteAllText(path, valueReplace);
    }
    public static string ReadString(string fileName, string passPhrase, string folder = "")
    {
        string path = Application.persistentDataPath;
        if (!string.IsNullOrEmpty(folder))
        {
            path = Path.Combine(path, folder);
        }
        path = Path.Combine(path, string.Concat(fileName, ".txt"));
        if (!File.Exists(path)) return null;
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        //Debug.Log("reader2: " + reader.ReadToEnd());
        string value = reader.ReadToEnd();
        reader.Close();

        return Decrypt(value, passPhrase);
    }

    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    public static string Encrypt(string plainText, string passPhrase)
    {
        // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
        // so that the same Salt and IV values can be used when decrypting.  
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
        // Get the complete stream of bytes that represent:
        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            // Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Clear Data")]
    public static void ClearData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Clear all PlayerPrefs");
        var paths = Directory.GetFiles(Application.persistentDataPath);
        foreach (var path in paths)
        {
            File.Delete(path);
            Debug.Log(string.Format("Clear file at: {0}", path));
        }
        paths = Directory.GetDirectories(Application.persistentDataPath);
        foreach (var path in paths)
        {
            Directory.Delete(path, true);
            Debug.Log(string.Format("Clear Folder at: {0}", path));
        }
    }
#endif
}

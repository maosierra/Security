using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Components.Forms;

namespace SecurityPage.Services
{
    public class AesService : IAesService
    {
        public MemoryStream FileEncrypt(string inputFilePath, byte[] passwordBytes)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var memoryStream = new MemoryStream();
            var aes = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            var fileStream = new FileStream(inputFilePath, FileMode.Open);

            int data;
            while ((data = fileStream.ReadByte()) != -1)
                cryptoStream.WriteByte((byte)data);

            cryptoStream.FlushFinalBlock();

            return memoryStream;
        }

        public MemoryStream FileEncrypt(MemoryStream inputFile, byte[] passwordBytes)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var memoryStream = new MemoryStream();
            var aes = new RijndaelManaged { KeySize = 256, BlockSize = 128 };

            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            //var fileStream = new FileStream(inputFilePath, FileMode.Open);

            //int data;
            //while ((data = inputFile.ReadByte()) != -1)
            //    cryptoStream.WriteByte((byte)data);

            var byteArrayInput = new byte[inputFile.Length];
            inputFile.Read(byteArrayInput, 0, byteArrayInput.Length);
            cryptoStream.Write(byteArrayInput, 0, byteArrayInput.Length);

            cryptoStream.FlushFinalBlock();
            memoryStream.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        public MemoryStream FileDecrypt(Stream encryptedFileStream, byte[] passwordBytes)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var AES = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            var cryptoStream = new CryptoStream(encryptedFileStream, AES.CreateDecryptor(), CryptoStreamMode.Read);
            var memoryStream = new MemoryStream();

            //int data;
            //while ((data = cryptoStream.ReadByte()) != -1)
            //    memoryStream.WriteByte((byte)data);
            var fsDecrypted = new StreamWriter(memoryStream);
            fsDecrypted.Write(new StreamReader(cryptoStream).ReadToEnd());
            fsDecrypted.Flush();
            memoryStream.Position = 0;


            return memoryStream;
        }

        public MemoryStream FileDecrypt(MemoryStream encryptedFileStream, byte[] passwordBytes)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var AES = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            var cryptoStream = new CryptoStream(encryptedFileStream, AES.CreateDecryptor(), CryptoStreamMode.Read);
            var memoryStream = new MemoryStream();

            //int data;
            //while ((data = cryptoStream.ReadByte()) != -1)
            //    memoryStream.WriteByte((byte)data);
            var fsDecrypted = new StreamWriter(memoryStream);
            fsDecrypted.Write(new StreamReader(cryptoStream).ReadToEnd());
            fsDecrypted.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}


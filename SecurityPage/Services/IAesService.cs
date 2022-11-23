using System;
namespace SecurityPage.Services
{
    public interface IAesService
    {
        public MemoryStream FileEncrypt(string inputFilePath, byte[] passwordBytes);
        public MemoryStream FileEncrypt(MemoryStream inputFile, byte[] passwordBytes);
        public MemoryStream FileDecrypt(Stream encryptedFileStream, byte[] passwordBytes);
        public MemoryStream FileDecrypt(MemoryStream encryptedFileStream, byte[] passwordBytes);
    }
}


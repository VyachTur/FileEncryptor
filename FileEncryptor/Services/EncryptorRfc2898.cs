using FileEncryptor.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileEncryptor.WPF.Services
{
    internal class EncryptorRfc2898 : IEncryptor
    {
        private static readonly byte[] s_salt =
        {
            0x26, 0xdc, 0xff, 0x00,
            0xad, 0xed, 0x7a, 0xee,
            0xc5, 0xfe, 0x07, 0xaf,
            0x4d, 0x08, 0x22, 0x3c
        };


        //private static ICryptoTransform GetEncryptor(string password, byte[] salt = null)
        //{
        //    var pdb = new Rfc2898DeriveBytes(password, salt ?? s_salt);
        //    var algorithm = Rijndael.Create();
        //    algorithm.Key = pdb.GetBytes(32);
        //    algorithm.IV = pdb.GetBytes(16);
        //    return algorithm.CreateEncryptor();
        //}

        //private static ICryptoTransform GetDecryptor(string password, byte[] salt = null)
        //{
        //    var pdb = new Rfc2898DeriveBytes(password, salt ?? s_salt);
        //    var algorithm = Rijndael.Create();
        //    algorithm.Key = pdb.GetBytes(32);
        //    algorithm.IV = pdb.GetBytes(16);
        //    return algorithm.CreateEncryptor();
        //}

        [Obsolete]
        private static ICryptoTransform GetCryptor(string password, byte[]? salt = null, bool isEncryptor = true)
        {
            var pdb = new Rfc2898DeriveBytes(password, salt ?? s_salt);
            var algorithm = Rijndael.Create();
            algorithm.Key = pdb.GetBytes(32);
            algorithm.IV = pdb.GetBytes(16);
            return isEncryptor ? algorithm.CreateEncryptor() : algorithm.CreateDecryptor();
        }

        [Obsolete]
        public void Encript(string sourcePath, string destinationPath, string password, int bufferLength = 104200)
        {
            var encryptor = GetCryptor(password);

            using var destinationEncrypted = File.Create(destinationPath, bufferLength);
            using var destination = new CryptoStream(destinationEncrypted, encryptor, CryptoStreamMode.Write);
            using var source = File.OpenRead(sourcePath);

            var buffer = new byte[bufferLength];
            int readed;
            do
            {
                Thread.Sleep(1);
                readed = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, readed);
            } while (readed > 0);
            destination.FlushFinalBlock();
        }

        [Obsolete]
        public bool Decript(string sourcePath, string destinationPath, string password, int bufferLength = 104200)
        {
            var decryptor = GetCryptor(password, isEncryptor:false);

            using var destinationDecrypted = File.Create(destinationPath, bufferLength);
            using var destination = new CryptoStream(destinationDecrypted, decryptor, CryptoStreamMode.Write);
            using var encryptedSource = File.OpenRead(sourcePath);

            var buffer = new byte[bufferLength];
            int readed;
            do
            {
                Thread.Sleep(1);
                readed = encryptedSource.Read(buffer, 0, bufferLength);
                destination.Write(buffer, 0, readed);
            } while (readed > 0);

            try
            {
                destination.FlushFinalBlock();
            }
            catch (CryptographicException)
            {
                return false;
            }

            return true;
        }




        [Obsolete]
        public async Task EncriptAsync(string sourcePath, string destinationPath, string password, int bufferLength = 104200, 
            IProgress<double>? progress = null, CancellationToken cancel = default)
        {
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Файл-источник для процесса шифрования не найден", sourcePath);
            if (bufferLength <= 0) throw new ArgumentOutOfRangeException(nameof(bufferLength), bufferLength, "Размер буфера чтения должен быть больше 0");
            
            cancel.ThrowIfCancellationRequested();

            var encryptor = GetCryptor(password);

            try
            {
                await using var destinationEncrypted = File.Create(destinationPath, bufferLength);
                await using var destination = new CryptoStream(destinationEncrypted, encryptor, CryptoStreamMode.Write);
                await using var source = File.OpenRead(sourcePath);

                double sourceFilePosition;
                double sourceFileLength = source.Length;

                var buffer = new byte[bufferLength];
                int readed;
                do
                {
                    readed = await source.ReadAsync(buffer, 0, buffer.Length, cancel).ConfigureAwait(false);
                    // Дополнительные действия по завершению асинхронной операции
                    // .........
                    await destination.WriteAsync(buffer, 0, readed, cancel).ConfigureAwait(false);

                    sourceFilePosition = source.Position;
                    progress?.Report(sourceFilePosition/sourceFileLength);

                    Thread.Sleep(1);

                    if (cancel.IsCancellationRequested)
                    {
                        // Очистка состояния операции
                        cancel.ThrowIfCancellationRequested();
                    }

                } while (readed > 0);

                destination.FlushFinalBlock();

                progress?.Report(1);
            }
            catch (OperationCanceledException)
            {
                File.Delete(destinationPath);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in EncryptAsync: {0}", ex);
                throw;
            }
        }


        [Obsolete]
        public async Task<bool> DecriptAsync(string sourcePath, string destinationPath, string password, int bufferLength = 104200, 
            IProgress<double>? progress = null, CancellationToken cancel = default)
        {
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Файл-источник для процесса дешифрования не найден", sourcePath);
            if (bufferLength <= 0) throw new ArgumentOutOfRangeException(nameof(bufferLength), bufferLength, "Размер буфера чтения должен быть больше 0");

            cancel.ThrowIfCancellationRequested();

            var decryptor = GetCryptor(password, isEncryptor: false);

            try
            {
                await using var destinationDecrypted = File.Create(destinationPath, bufferLength);
                await using var destination = new CryptoStream(destinationDecrypted, decryptor, CryptoStreamMode.Write);
                await using var encryptedSource = File.OpenRead(sourcePath);

                double sourceFilePosition;
                double sourceFileLength = encryptedSource.Length;

                var buffer = new byte[bufferLength];
                int readed;
                do
                {
                    readed = await encryptedSource.ReadAsync(buffer, 0, bufferLength, cancel).ConfigureAwait(false);
                    // Дополнительные действия по завершению асинхронной операции
                    // .........
                    await destination.WriteAsync(buffer, 0, readed, cancel).ConfigureAwait(false);

                    sourceFilePosition = encryptedSource.Position;
                    progress?.Report(sourceFilePosition / sourceFileLength);

                    Thread.Sleep(1);

                    cancel.ThrowIfCancellationRequested();

                } while (readed > 0);

                progress?.Report(1);

                try
                {
                    destination.FlushFinalBlock();
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                File.Delete(destinationPath);
                throw;
            }


            //return Task.FromResult(true);
            return true;
        }
    }
}

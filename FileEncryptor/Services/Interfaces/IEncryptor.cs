using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IEncryptor
    {
        void Encript(string sourcePath, string destinationPath, string password, int bufferLength = 104200);

        bool Decript(string sourcePath, string destinationPath, string password, int bufferLength = 104200);

        Task EncriptAsync(string sourcePath, string destinationPath, string password, int bufferLength = 104200, IProgress<double>? progress = null, CancellationToken cancel = default);

        Task<bool> DecriptAsync(string sourcePath, string destinationPath, string password, int bufferLength = 104200, IProgress<double>? progress = null, CancellationToken cancel = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IEncryptor
    {
        void Encript(string sourcePath, string destinationPath, string password, int bufferLength = 104200);

        bool Decript(string sourcePath, string destinationPath, string password, int bufferLength = 104200);
    }
}

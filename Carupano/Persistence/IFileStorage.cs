using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Persistence
{
    public interface IFileStorage
    {
        Task Save(string idOrPath, Stream stream);
        Task<Stream> Read(string idOrPath);
    }
}

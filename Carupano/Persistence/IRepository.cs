using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    interface IRepository
    {
        object FindById(string id);
    }
}

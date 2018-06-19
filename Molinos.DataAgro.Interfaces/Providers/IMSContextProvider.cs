using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.DataRepository;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMSContextProvider
    {
        MSContext GetMSContext();


        string GetIdActiveDirectory();
    }
}

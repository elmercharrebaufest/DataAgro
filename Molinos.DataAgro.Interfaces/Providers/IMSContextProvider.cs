
using Mastersoft.Framework.DataRepository;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMSContextProvider
    {
        MSContext GetMSContext();


        string GetIdActiveDirectory();
    }
}

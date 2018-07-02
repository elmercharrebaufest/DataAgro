
using Mastersoft.Framework.DataRepository;

using Molinos.DataAgro.Interfaces;

using WebDataAgro.Core;


namespace WebDataAgro
{
    public class MSContextProvider : IMSContextProvider
    {
        private MSContext Context {get;set;}

        public MSContextProvider()
        {
            Context = Util.GetMSContext();
        }
        public MSContext GetMSContext()
        {
            return Context;
        }

        public string GetIdActiveDirectory()
        {
            return Util.GetIdActiveDirectory();
        }
    }
}
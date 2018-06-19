using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.DataRepository;

using Molinos.DataAgro.Interfaces;

using WebDataAgro.Core;


namespace WebDataAgro
{
    public class MSContextProvider : IMSContextProvider
    {
        public MSContext GetMSContext()
        {
            return Util.GetMSContext();
        }

        public string GetIdActiveDirectory()
        {
            return Util.GetIdActiveDirectory();
        }
    }
}
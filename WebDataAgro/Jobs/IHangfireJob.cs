using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDataAgro.Job
{
    public interface IHangfireJob
    {
        void Execute();
    }
}

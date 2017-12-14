using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Threads
{
    public interface IServiceExecutor
    {
        int Execute(IServiceTask task);
    }
}

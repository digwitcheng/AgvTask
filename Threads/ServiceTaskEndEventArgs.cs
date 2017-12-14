using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Threads
{
    public class ServiceTaskEndEventArgs : EventArgs
    {
        public ServiceTaskEndEventArgs(IServiceTask _task)
        {
            this.task = _task;
        }

        private IServiceTask task;
        public IServiceTask Task
        {
            get { return this.task; }
            set { this.task = value;}
        }
    }
}

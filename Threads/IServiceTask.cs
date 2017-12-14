using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Threads
{
    public interface IServiceTask
    {
        event EventHandler<ServiceTaskEndEventArgs> OnServiceTaskEndHandler;

        int StartTask();
        void Run();
        int CancelTask();
        bool IsRunning
        {
            get;
        }
        bool IsCanceled
        {
            get;
        }
        String GetTaskName();
        void SetTaskName(string taskName);
    }
}

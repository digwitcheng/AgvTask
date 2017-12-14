using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ImageLib.Threads
{
    public class ServiceTaskAdapter : AbstractContinuedTask
    {
        private String taskName = "ServiceTaskAdapter";

        public override int PreparedTask()
        {
            return 0;
        }

        public override void ExecuteTask()
        {
           
        }

        public override int CleanTask()
        {
            return 0;
        }

        public override void TaskExecutedWatch(Stopwatch watch)
        {

        }

        public override String GetTaskName()
        {
            return this.taskName;
        }

        public override void SetTaskName(string taskName)
        {
            return;
        }
    }
}

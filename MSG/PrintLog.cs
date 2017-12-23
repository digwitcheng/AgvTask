using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using Const;
using ImageLib;
using ImageLib.Threads;
using VideoCoding.Services;

using AGV_TASK;
using TASK.MSG;
using TASK.AGV;
using TASK.XUMAP;
using Newtonsoft.Json;
using AGV_V1._0;

namespace TASK.MSG
{
    public class PrintLog : ServiceTaskAdapter
    {

        private String txtName = "LogPathIms.txt";
        private String taskName = "";

        public PrintLog(string _tsakName)
        {
            this.taskName = _tsakName;
        }

        public override int PreparedTask()
        {
            return 0;
        }

        public override void ExecuteTask()
        {
            if (QueueInstance.Instance.IsMyLogHasData())
            {
                string str = QueueInstance.Instance.GetMyLogList();
                if (str != "" || str == null)
                {
                    StreamWriter sw = new StreamWriter(txtName, true);
                    sw.WriteLine(str);
                    sw.Close();
                }
            }
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

        public override void SetTaskName(string _taskName)
        {
            this.taskName = _taskName;
        }
    }
}

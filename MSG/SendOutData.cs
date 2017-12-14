using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ImageLib;
using ImageLib.Threads;
using AGV_TASK.MSG;
using TASK.MSG;

namespace VideoCoding.Services.Tasks
{
    public class ImsMsgSendTask : ServiceTaskAdapter
    {
        private String taskName = "";

        public ImsMsgSendTask(String _taskName)
        {
            if (_taskName != null && !_taskName.Equals(""))
            {
                this.taskName = _taskName;
            }
        }

        public override int PreparedTask()
        {
            return 0;
        }

        public override void ExecuteTask()
        { 
            try
            {
               string getjson= QueueInstance.Instance.GetMyQueueList();
                //判断与PATH是否连接
             
               



               
                //ImsResponse sendcmd = GetResponseFromSendList();

                //if (sendcmd != null)
                //{
                //    DoSendImsMsg(sendcmd);
                //}
                //else
                //{
                //    //Thread.Sleep(ServiceConstantsDef.SEND_MSG_TO_IMS_INTERVIEW_TIME);
                //    Thread.Sleep(5000);
                //}
            }
            catch (Exception ex)
            {
             
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

        #region 私有方法
        //private ImsResponse GetResponseFromSendList()
        //{
        //    ImsResponse sendMsg = null;
        //    if (ImsNetManager.Instance.IsImsSendListHasData())
        //    {
        //        sendMsg = ImsNetManager.Instance.GetResponseFromImsSendList();
        //    }
        //    return sendMsg;
        //}

        //private void DoSendImsMsg(ImsResponse responseMsg)
        //{
        //    //判断发送给谁，GUI,PATH
        //    try
        //    { 

        //    }
        //    catch (SocketException ex)
        //    {
                
        //    }
             
        //}

        #endregion

    }
}

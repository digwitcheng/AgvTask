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
using VideoCoding.Entity.IMS;
using ImageLib.MSG;

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
            String methodStr = "ImsMsgSendTask.ExecuteTask";
            try
            {
                ImsResponse sendcmd = GetResponseFromSendList();

                if (sendcmd != null)
                {
                    DoSendImsMsg(sendcmd);
                }
                else
                {
                    Thread.Sleep(ServiceConstantsDef.SEND_MSG_TO_IMS_INTERVIEW_TIME);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(methodStr, "发送响应消息给IMS时捕获异常:" + ex.Message);
                Thread.Sleep(ServiceConstantsDef.SEND_MSG_TO_IMS_EXCEPTION_INTERVIEW_TIME);
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
        private ImsResponse GetResponseFromSendList()
        {
            ImsResponse sendMsg = null;
            if (ImsNetManager.Instance.IsImsSendListHasData())
            {
                sendMsg = ImsNetManager.Instance.GetResponseFromImsSendList();
            }
            return sendMsg;
        }

        private void DoSendImsMsg(ImsResponse responseMsg)
        {
            
            try
            {
                if (ImsNetManager.Instance.IsImsSocketConnect())
                {
                    IMsgInfo msg = responseMsg;
                    MsgSend msgSend = new MsgSend();
                    int res = msgSend.ConvertFrom(msg);
                    if (0 == res)
                    {
                        byte[] data = msgSend.CloneBytes();
                        ImsNetManager.Instance.NowImsStation.SendBytes(data);

                        Logger.LogInfo(null, "向IMS发送响应消息 ：【" + BitConverter.ToString(responseMsg.CloneMsgBytes()) + "】");
                    }
                    else
                    {
                        Logger.LogError(null, "向IMS发送响应消息 ：【" + "消息不合法" + "】");
                    }
                }
            }
            catch (SocketException ex)
            {
                ImsNetManager.Instance.NowImsStation.CloseImsConn();
                Logger.LogError(null, "IMS发送响应消息时捕获到SOCKET异常：【" + ex.Message+"】");
            }
            catch (Exception ex)
            {
                ImsNetManager.Instance.NowImsStation.CloseImsConn();
                Logger.LogError(null, "IMS发送响应消息时捕获到未知异常：【" + ex.Message+"】");
            }
        }
        #endregion

    }
}

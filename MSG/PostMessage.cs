using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGV_TASK.MSG;
using System.Threading;
using TASK.MSG;
using VideoCoding.Services;
using System.Net.Sockets;
using ImageLib.Threads;

namespace AGV_TASK.MSG
{
    public class PostMessage : ServiceTaskAdapter
    {
        private String taskName = "";

        public PostMessage(String _taskName)
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
            //String methodStr = "PostMessage.ExecuteTask";
            try
            {
                ImsResponse sendcmd = GetResponseFromSendList();

                if (sendcmd != null)
                {
                    DoSendImsMsg(sendcmd);
                }
                else
                {
                   // Thread.Sleep(ServiceConstantsDef.SEND_MSG_TO_IMS_INTERVIEW_TIME);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
               // Logger.LogError(methodStr, "发送响应消息给IMS时捕获异常:" + ex.Message);
               // Thread.Sleep(ServiceConstantsDef.SEND_MSG_TO_IMS_EXCEPTION_INTERVIEW_TIME);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "发送响应消息给IMS时捕获异常:" + ex.Message);
                Thread.Sleep(1000);
            }
        }

        public override int CleanTask()
        {
            return 0;
        }

        public override String GetTaskName()
        {
            return this.taskName;
        }

        #region 私有方法
        private ImsResponse GetResponseFromSendList()
        {
            ImsResponse sendMsg = null;
            if (QueueInstance.Instance.IsMyMessageHasData())
            {
                sendMsg = QueueInstance.Instance.GetMyMessageList();
            }
            return sendMsg;
        }

        private void DoSendImsMsg(ImsResponse responseMsg)
        {
            
            try
            {
                if (ImsNetManager.Instance.IsImsSocketConnect())
                {
                     
                    if(responseMsg.obj=="PATH")
                    {
                        string str = responseMsg.data;
                        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(str);
                        ImsNetManager.Instance.NowImsStation.SendBytes(buffer);

                       // Logger.LogInfo(null, "向IMS发送响应消息 ：【" + BitConverter.ToString(responseMsg.CloneMsgBytes()) + "】");
                        QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":给PATH发送目的地消息成功");
                        QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":"+"目的地"+str+"\n");

                    }
                    else
                    {
                       // Logger.LogError(null, "向IMS发送响应消息 ：【" + "消息不合法" + "】");
                        QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":给PATH发送消息,消息不合法");
                        QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "给PATH发送消息失败");
                    }
                }
            }
            catch (SocketException ex)
            {
                ImsNetManager.Instance.NowImsStation.CloseImsConn();
               // Logger.LogError(null, "IMS发送响应消息时捕获到SOCKET异常：【" + ex.Message+"】");
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "给PATH发送消息,捕获到SOCKET异常"+ex.Message);
                QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "给PATH发送消息失败");
            }
            catch (Exception ex)
            {
                ImsNetManager.Instance.NowImsStation.CloseImsConn();
                //Logger.LogError(null, "IMS发送响应消息时捕获到未知异常：【" + ex.Message+"】");
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "给PATH发送消息,捕获到未知异常"+ex.Message);
                QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "给PATH发送消息失败");
            }
        }
        #endregion
    }
}

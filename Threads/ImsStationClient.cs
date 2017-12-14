using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using ImageLib;
using ImageLib.Threads;
using TASK.MSG;
using System.Windows.Forms;

namespace VideoCoding.Services
{
    public class ImsStationClient
    {
        private volatile int thrdRecvMsgFromIms = 0;
        // private volatile int thrdReconnectIms = 0;

        private bool IsIms1 = true;

        private const string IMS_STATION_RECEIVE_TASK_NAME_PREFIX = "ImsRecvTask-";
        private const string IMS_STATION_RECONNECT_TASK_NAME_PREFIX = "ImsReconnTask-";

        private Socket imsSocket = null;


        #region 初始化操作
        public int InitImsStationClient(int a)
        {
            // String methodStr = "ImsStationClient.InitImsStationClient";
            int ret = -1;
            if (this.IsImsInit == true)
            {
                return 9;
            }

            if (a == 1)
            {
                IsIms1 = true;
            }
            else if (a == 2)
            {
                IsIms1 = false;
            }
            else if (a == 3)
            {
                IsIms1 = false;
            }


            if (IsSocketConnect())
            {
                CloseImsConn();
            }

            int resconn = ConnToIms();
            if (resconn != 0)
            {
                isImsInit = false;
                ret = 1;
            }
            else
            {

                isImsInit = true;
                ret = 0;
            }


            return ret;
        }
        #endregion

        private int ConnToIms()
        {
            //return 0;

            // String methodStr = "ImsStationClient.ConnToIms";

            if (this.IsNetArrived == false)
            {
                //Logger.LogInfo(methodStr, "IMS: 网络不通,无法创建Socket连接");
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "socket连接失败");
                QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "socket连接失败");
                return 9;
            }

            if (this.IsSocketConnect())
            {
                //Logger.LogInfo(methodStr, "IMS : Socket连接正常");
                //MessageBox.Show("PATH连接TASK成功");
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "socket连接正常");
                QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "socket连接正常");
                return 0;
            }
            else
            {
                CloseImsConn();
            }
            Socket tSck = null;
            try
            {
                tSck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                //Logger.LogError(methodStr, "IMS:创建Socket对象时捕获异常:" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "创建Socket对象时捕获异常" + ex.Message);
                return 1;
            }

            try
            {
                IPEndPoint AcsEndPoint = new IPEndPoint(IPAddress.Parse(this.ImsHostIpAddr.Trim()), this.ImsHostPort);

                tSck.Connect(AcsEndPoint);

                if (tSck.Connected)
                {
                    this.imsSocket = tSck;
                    //Logger.LogInfo(methodStr, "IMS:socket连接建立成功-IP:[" + this.ImsHostIpAddr + "]");
                    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "socket连接成功");
                    QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "socket连接成功");
                    BegReceiveImsTask(IsIms1);
                    return 0;
                }
                else
                {
                    //Logger.LogError(methodStr, "IMS:Socket连接建立失败");
                    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "socket连接建立失败");
                    QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "socket连接建立失败");
                    return 2;
                }
            }
            catch (SocketException ex)
            {
                // Logger.LogError(null, "IMS:Socket连接建立时捕获Socket异常:" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "创建Socket对象时捕获异常" + ex.Message);
                return 3;
            }
            catch (Exception ex)
            {
                //Logger.LogError(null, "IMS:Socket连接建立时捕获异常:" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "创建Socket对象时捕获异常" + ex.Message);
                return 4;
            }
        }

        public void CloseImsConn()
        {
            try
            {
                if (IsSocketConnect())
                {
                    imsSocket.Close();

                    //Logger.LogInfo(null, "IMS:Socket连接关闭成功");
                    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "socket连接关闭成功");
                    QueueInstance.Instance.AddMessageShowList(System.DateTime.Now.ToString() + ":" + "socket连接关闭成功");
                }
            }
            catch (SocketException ex)
            {
                //Logger.LogError(null, "IMS::" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "Socket连接关闭时捕获Socket异常" + ex.Message);
            }
            catch (Exception ex)
            {
                // Logger.LogError(null, "IMS::" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "Socket连接关闭时捕获异常" + ex.Message);
            }
            finally
            {
                imsSocket = null;
                this.IsImsInit = false;
            }
        }

        public int SendBytes(Byte[] responseMsg)
        {

            if (!this.IsSocketConnect())
            {
                //Logger.LogError(null, "IMS:Socket连接断开,SendBytes无法发送VCS指令");
                return 2;
            }

            try
            {
                this.imsSocket.Send(responseMsg);

                //Logger.LogInfo(null, "IMS:SendCmd发送了响应");

                return 0;
            }
            catch (Exception ex)
            {
                //Logger.LogError(null, "IMS::" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "socket发送响应时捕获异常" + ex.Message);
                return 3;
            }
        }

        public bool IsSocketConnect()
        {
            return ((this.imsSocket != null) && (this.imsSocket.Connected));
        }

        #region Tasks开启关闭操作
        /************************************************************************/
        /* 在ImsStaionClient类中，负责管理连接任务的开启关闭，重连任务的开启关闭
        /************************************************************************/
        private int BegReceiveImsTask(bool IsIms)
        {
            //  string methodStr = "ImsStationClient.BegReceiveImsTask";
            if (thrdRecvMsgFromIms == 0)
            {
                try
                {
                    if (ServiceManager.Instance.CurrentExecutor.Execute(
                        new ImsRecvTask(
                            ImsStationClient.IMS_STATION_RECEIVE_TASK_NAME_PREFIX + this.imsStationName, this, IsIms)) != 0)
                    {
                        //Logger.LogError(methodStr, "IMS指令接收任务启动失败");

                        return 1;
                    }

                    Thread.Sleep(1);
                    thrdRecvMsgFromIms = 1;

                    return 0;
                }
                catch (Exception ex)
                {
                    // Logger.LogError(methodStr, "IMS:" + ex.Message);
                    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "指令接收任务启动捕获异常" + ex.Message);

                    return 1;
                }
            }
            else
            {
                // Logger.LogInfo(methodStr, "IMS指令接收任务已经启动,不能重复启动");

                return 9;
            }
        }
#endregion




        #region 属性
        public Socket ImsSocket
        {
            get { return this.imsSocket; }
        }
        private bool isImsInit = false;
        public bool IsImsInit
        {
            get { return this.isImsInit; }
            set { this.isImsInit = value; }
        }

        private bool isNetArrived = true;
        public bool IsNetArrived
        {
            get { return isNetArrived; }
            set { this.isNetArrived = value; }
        }

        private string imsHostIpAddr;
        public string ImsHostIpAddr
        {
            get { return imsHostIpAddr; }
            set { imsHostIpAddr = value; }
        }

        private int imsHostPort;
        public int ImsHostPort
        {
            get { return imsHostPort; }
            set { imsHostPort = value; }
        }

        private bool isStationEndWork = false;
        public bool IsStationEndWork
        {
            get { return this.isStationEndWork; }
            set { this.isStationEndWork = value; }
        }

        private string imsStationName = "";
        public string ImsStationName
        {
            get { return this.imsStationName; }
            set { this.imsStationName = value; }
        }

        private bool isCloseStation = false;
        public bool IsCloseStation
        {
            get { return this.isCloseStation; }
            set { this.isCloseStation = value; }
        }
        #endregion
    }
}

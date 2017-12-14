using System;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.IO;
using ImageLib;
using ImageLib.Threads;
using AGV_TASK.MSG;
using TASK.MSG;

namespace VideoCoding.Services
{
    public class ImsNetManager
    {
        private ImsStationClient myStation = null;
     //   private ImsStationClient myStation1 = null;
      //  private ImsStationClient myStation2 = null;
    //    private ImsStationClient myStation3 = null;

        //接收来自GUI的消息
        private string pathIP = "127.0.0.1";
        public string PATHIP
        {
            get { return pathIP; }
        }

        private int pathPort = 0;
        public int PATHPORT
        {
            get { return pathPort; }
        }
 
        private const string PATH_IP_KEY = "PATHIP";
        private const string PATH_PORT_KEY = "PATHPORT";
         

        #region 单例构造
        /// <summary>
        /// 单例构造
        /// </summary>
        private static ImsNetManager instance;
        public static ImsNetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImsNetManager();
                }
                return instance;
            }
        }

        private ImsNetManager()
        {

        }
        #endregion

        public void Init()
        {
            this.myStation = new ImsStationClient();
            this.myStation.ImsHostIpAddr = this.pathIP;
            this.myStation.ImsHostPort = this.pathPort;
            string stationName = this.pathIP + ":" + this.pathPort;
            this.myStation.ImsStationName = stationName;
            this.myStation.InitImsStationClient(1);

             
        }

        public void ReadConfguration()
        {
            try
            {

                this.pathIP = ConfigurationManager.AppSettings[ImsNetManager.PATH_IP_KEY];
                this.pathPort = Convert.ToInt32(ConfigurationManager.AppSettings[ImsNetManager.PATH_PORT_KEY]); 


            }
            catch (System.Exception ex)
            {
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "读取配置文件失败" + ex.Message);
            }
        }

        public bool IsImsSocketConnect()
        {

            return (this.myStation != null && this.myStation.IsSocketConnect());


        }


        private volatile Queue<ImsResponse> imsResponseMsgList = new Queue<ImsResponse>();
        private object imsResponseMsgListLock = new object();

        public bool IsImsSendListHasData()
        {
            bool ret = false;
            lock (this.imsResponseMsgListLock)
            {
                ret = (this.imsResponseMsgList != null && this.imsResponseMsgList.Count > 0);
            }
            return ret;
        }

        public ImsResponse GetResponseFromImsSendList()
        {
            ImsResponse msg = null;
            lock (this.imsResponseMsgListLock)
            {
                msg = this.imsResponseMsgList.Dequeue() as ImsResponse;
            }
            return msg;
        }

        #region 属性
        public ImsStationClient NowImsStation
        {
            get { return this.myStation; }
        }
        #endregion


        #region 开启记录日志和发送消息线程
        public  void ThreadStart()
        {
            int res=ServiceManager.Instance.CurrentExecutor.Execute(new PrintLog("printLog"));
            if ( res== 0)
            {
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "日志队列线程开启");
            }
            //启动发送消息的线程
            if (ServiceManager.Instance.CurrentExecutor.Execute(new PostMessage("PostSomeMessage")) == 0)
            {

                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + ":" + "发送消息队列线程开启");
            }
        }
        #endregion
    }
}

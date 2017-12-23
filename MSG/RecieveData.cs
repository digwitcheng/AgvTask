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
using AGV_TASK.MSG;

namespace TASK.MSG
{
    public class ImsRecvTask : ServiceTaskAdapter
    {
        //public const int END_CONNTION = 0;
        //public const int MSG = 1;
        //public const int MAP_FILE = 2;
        //public const int AGV_FILE = 3;
        //public const int RESTART = 4;
        //public const int ARRIVED = 5;

        public const int SUCCESS = 6;
        public const int FAIRED = -1;

        private String taskName = "";
        private ImsStationClient myStation = null;

        public ImsRecvTask(string _tskName, ImsStationClient _station, bool IsIms)
        {
            myStation = _station;
            taskName = _tskName;
        }

        public override int PreparedTask()
        {
            return 0;
        }

        public static string getstring()
        {
            if (QueueInstance.Instance.IsMessageShowHasData())
            {
                return QueueInstance.Instance.GetMessageShowList();
            }
            else
            {
                return "暂时没有消息";
            }

        }

        public string queuedata;
        public Socket sckClient;
        public override void ExecuteTask()
        {
            //#region 接收
            //try
            //{
            //    //string jsonstring="";
            //    byte[] bRecTmp = new byte[1024 * 1024];

            //    sckClient = myStation.ImsSocket;
            //    int receiveNumber = sckClient.Receive(bRecTmp, SocketFlags.None);

            //    #region hxc

            //    //MapRead mmread = new MapRead();
            //    // mmread.MAP_classify();
            //    //AGVRead aaread = new AGVRead();
            //    //  aaread.AGV_Read();

            //    if (bRecTmp[0] == (byte)MessageType.msg)
            //    {
            //        //string json = Encoding.UTF8.GetString(bRecTmp, 1, receiveNumber - 1);
            //        ////TODO
            //        //QueueJson.Instance.AddMyQueueList(json);
            //        //json = null;
            //    }
            //    else if (bRecTmp[0] == (byte)MessageType.mapFile)
            //    {
            //        string path = System.Configuration.ConfigurationManager.AppSettings["MAPPath"].ToString();
            //        //System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase+
            //        // string path ="../../ElcMap.xml";
            //        int res = reciveFile(path, bRecTmp, receiveNumber);
            //        if (res != FAIRED)
            //        {
            //            string mstr = System.DateTime.Now.ToString() + ":" + "接收MAP文件成功";
            //            QueueInstance.Instance.AddMessageShowList(mstr);
            //            // OnMessageEvent("地图接收成功");
            //        }
            //        else
            //        {
            //            // OnMessageEvent("地图接收失败");
            //        }
            //        MapRead mread = new MapRead();
            //        mread.MAP_classify();
            //      //  mread.MAP_classify();
            //    }
            //    else if (bRecTmp[0] == (byte)MessageType.AgvFile)
            //    {
            //        string path = System.Configuration.ConfigurationManager.AppSettings["AGVPath"].ToString(); ;
            //        // string path ="../../ElcMap.xml";
            //        int res = reciveFile(path, bRecTmp, receiveNumber);
            //        if (res != FAIRED)
            //        {
            //            string mstr = System.DateTime.Now.ToString() + ":" + "接收AGV文件成功";
            //            QueueInstance.Instance.AddMessageShowList(mstr);
            //            //  OnMessageEvent("AGV文件接收成功");
            //        }
            //        else
            //        {
            //            // OnMessageEvent("AGV文件接收失败");
            //        }
            //        //获得地图信息之后，再获得AGV信息
            //        AGVRead aread = new AGVRead();
            //        aread.AGV_Read();
            //        //aread.AGV_Read();

            //    }
            //    else if (bRecTmp[0] == (byte)MessageType.reStart)
            //    {
            //        string mstr = System.DateTime.Now.ToString() + ":" + "复位";
            //        QueueInstance.Instance.AddMessageShowList(mstr);
            //        // #region 与界面建立socket连接，归位，所有小车回到RestArea

            //        //归位消息，所有小车去往RestArea,将处理得到的数据放到队列里
            //        for (int i = 0; i < AGVConstDefine.AGV.Count(); i++)
            //        {
            //            AGVDestination.Confirm_EndPoint(AGVConstDefine.AGV[i], "", AGVConstDefine.AGV[i].BeginX, AGVConstDefine.AGV[i].BeginY, "RestArea");
            //            SendData sd = JsonHelper.GetSendObj(AGVConstDefine.AGV[i]);
            //            string sendjs = Newtonsoft.Json.JsonConvert.SerializeObject(sd);
            //            //放进队列里
            //            if (sendjs.Trim() != "")
            //            {
            //                ImsResponse pathmessage = new ImsResponse();
            //                pathmessage.obj = "PATH";
            //                pathmessage.data = sendjs;
            //                //一个一个入队
            //                QueueInstance.Instance.AddMyMessageList(pathmessage);
            //            }

            //        }
            //    }
            //    if (bRecTmp[0] == (byte)MessageType.arrived)
            //    {
            //        string strContent = Encoding.ASCII.GetString(bRecTmp, 1, receiveNumber - 1);
            //        string mstr = System.DateTime.Now.ToString() + ":Arrive消息" + strContent+"\n";
            //        QueueInstance.Instance.AddMessageShowList(mstr);

            //        List<SendData> jsonObj = new List<SendData>();
            //        //从第0个位置开始查找json数据对象
            //        int start = strContent.IndexOf("{", 0);
            //        int endx = strContent.IndexOf("}", 0);
            //        while (start >= 0)
            //        {
            //            string str = strContent.Substring(start, endx - start + 1);
            //            //对收到的消息进行处理
            //            QueueInstance.Instance.AddYourMessageList(str);

            //            start = strContent.IndexOf("{", endx);
            //            endx = strContent.IndexOf("}", endx + 1);
            //        }
            //    }

            //    #endregion




            //    #region 原来接收xu

            //    //string strContent = Encoding.ASCII.GetString(bRecTmp, 0, receiveNumber);
            //    //string mstr = System.DateTime.Now.ToString() + ":" + strContent;
            //    //QueueInstance.Instance.AddMessageShowList(mstr);

            //    //MapRead mread = new MapRead();
            //    //mread.MAP_classify();
            //    //AGVRead aread = new AGVRead();
            //    //aread.AGV_Read();



            //    //int startJson = strContent.IndexOf("[", 0);
            //    //if (startJson > 0)
            //    //{
            //    //    #region 与界面建立socket连接，归位，所有小车回到RestArea
            //    //    if (strContent.Substring(0, startJson) == "GUI:restart")
            //    //    {
            //    //        //归位消息，所有小车去往RestArea,将处理得到的数据放到队列里
            //    //        for (int i = 0; i < AGVConstDefine.AGV.Count(); i++)
            //    //        {
            //    //            AGVDestination.Confirm_EndPoint(AGVConstDefine.AGV[i], "", AGVConstDefine.AGV[i].BeginX, AGVConstDefine.AGV[i].BeginY, "RestArea");
            //    //            SendData sd = JsonHelper.GetSendObj(AGVConstDefine.AGV[i]);
            //    //            string sendjs = Newtonsoft.Json.JsonConvert.SerializeObject(sd);
            //    //            //放进队列里
            //    //            if (sendjs.Trim() != "")
            //    //            {
            //    //                ImsResponse pathmessage=new ImsResponse();
            //    //                pathmessage.obj="PATH";
            //    //                pathmessage.data=sendjs;
            //    //                //一个一个入队
            //    //                QueueInstance.Instance.AddMyMessageList(pathmessage);
            //    //            }

            //    //        }
            //    //    }
            //    //    #endregion
            //    //    #region//获取MAP文件
            //    //    else if (strContent.Substring(0, startJson) == "PATH:XUMAP")
            //    //    {
            //    //        //string path = "../../ElcMap.xml";//System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase+
            //    //        //// string path ="../../ElcMap.xml";
            //    //        //reciveFile(path, buffer, r); 

            //    //        mread.MAP_classify();
            //    //        //frm.tb_MAP.Text = "PATH:XUMAP[]";
            //    //    }
            //    //    #endregion
            //    //    #region//获得AGV文件
            //    //    else if (strContent.Substring(0, startJson) == "PATH:AGV")
            //    //    {
            //    //        //获得地图信息之后，再获得AGV信息
            //    //        aread.AGV_Read();
            //    //        // frm.tb_AGV.Text = "PATH:AGV[]";
            //    //    }
            //    //    #endregion
            //    //    #region//获得小车到达目的地消息,对各个消息进行解析
            //    //    else if (strContent.Substring(0, startJson) == "PATH:Arrive")
            //    //    {
            //    //        List<SendData> jsonObj = new List<SendData>();
            //    //        //从第0个位置开始查找json数据对象
            //    //        int start = strContent.IndexOf("{", 0);
            //    //        int endx = strContent.IndexOf("}", 0);
            //    //        #region
            //    //        while (start > 0)
            //    //        {
            //    //            string str = strContent.Substring(start, endx - start + 1); 
            //    //            //对收到的消息进行处理
            //    //            QueueInstance.Instance.AddYourMessageList(str);

            //    //            start = strContent.IndexOf("{", endx);
            //    //            endx = strContent.IndexOf("}", endx + 1);
            //    //        }
            //    //        #endregion
            //    //    }
            //    //    #endregion

            //    //    #endregion   
            //    //}
            //    #endregion

            //}

            //catch (Exception ex)
            //{
            //    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "接收PATH消息失败" + ex.Message);
            //}

            //#endregion

        }


        //int reciveFile(string path, byte[] buffer, int r)
        //{
        //    try
        //    {
        //        using (FileStream fswrite = new FileStream(path, FileMode.Create, FileAccess.Write))
        //        {
        //            fswrite.Write(buffer, 1, r - 1);
        //        }
        //        //  MessageBox.Show("加载完成！");


        //        return SUCCESS;
        //    }
        //    catch
        //    {
        //        return FAIRED;
        //    }
        //}

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

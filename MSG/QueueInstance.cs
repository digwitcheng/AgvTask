using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AGV_TASK.MSG;

namespace TASK.MSG
{
    public class QueueInstance
    {
     

        //私有变量instance记录Queuelist唯一的实例，即instance是类queuelist的唯一实例
        private static QueueInstance instance = null;

        //提供可以供全局访问访问点
        public static QueueInstance Instance
        {
            get
            {
                //如果实例不在，则new一个新实例，否则返回一个实例
                if (instance == null)
                {
                    instance = new QueueInstance();
                }
                return instance;
            }
        }
        public QueueInstance()
        {
        }

        #region 用来发送的消息
        Queue<ImsResponse>MyMessage = new Queue<ImsResponse>();
        //加一个锁
        private Object MyMessageLock = new Object();

        //判断队列里是否有数据
        public bool IsMyMessageHasData()
        {
            bool ret = false;
            lock (this.MyMessageLock)
            {
                ret = (this.MyMessage != null && this.MyMessage.Count > 0);
            }
            return ret;
        }
       

        //判断队列里是否有数据
        public int GetMyMessageCount()
        {
            int ret = 0;
            lock (this.MyMessageLock)
            {
                ret = this.MyMessage.Count;
            }
            return ret;
        }
        
        //入队操作，增加数据
        public void AddMyMessageList(ImsResponse agvMap)
        {

            lock (this.MyMessageLock)
            {
                this.MyMessage.Enqueue(agvMap);
            }
        }

        //出队操作，获得数据
        public ImsResponse GetMyMessageList()
        {
           ImsResponse agvmap = null;
            lock (this.MyMessageLock)
            {
                bool has = (this.MyMessage != null && this.MyMessage.Count > 0);
                if (has)
                {
                    agvmap = this.MyMessage.Dequeue() as ImsResponse;
                }
               
            }
            return agvmap;
        }

        #endregion


        #region  处理收到的消息
        Queue<String> YourMessage = new Queue<String>();
        //加一个锁
        private Object YourMessageLock = new Object();

        //判断队列里是否有数据
        public bool IsYourMessageHasData()
        {
            bool ret = false;
            lock (this.YourMessageLock)
            {
                ret = (this.YourMessage != null && this.YourMessage.Count > 0);
            }
            return ret;
        }


        //判断队列里是否有数据
        public int GetYourMessageCount()
        {
            int ret = 0;
            lock (this.YourMessageLock)
            {
                ret = this.YourMessage.Count;
            }
            return ret;
        }

        //入队操作，增加数据
        public void AddYourMessageList(String agvMap)
        {

            lock (this.YourMessageLock)
            {
                this.YourMessage.Enqueue(agvMap);
            }
        }

        //出队操作，获得数据
        public String GetYourMessageList()
        {
            String agvmap = null;
            lock (this.YourMessageLock)
            {
                bool has = (this.YourMessage != null && this.YourMessage.Count > 0);
                if (has)
                {
                    agvmap = this.YourMessage.Dequeue() as String;
                }

            }
            return agvmap;
        }

        #endregion


        #region 记录日志
        Queue<String> MyLog = new Queue<String>();
        //加一个锁
        private Object MyLogLock = new Object();

        //判断队列里是否有数据
        public bool IsMyLogHasData()
        {
            bool ret = false;
            lock (this.MyLogLock)
            {
                ret = (this.MyLog != null && this.MyLog.Count > 0);
            }
            return ret;
        }


        //判断队列里是否有数据
        public int GetMyLogCount()
        {
            int ret = 0;
            lock (this.MyLogLock)
            {
                ret = this.MyLog.Count;
            }
            return ret;
        }

        //入队操作，增加数据
        public void AddMyLogList(String agvMap)
        {

            lock (this.MyLogLock)
            {
                this.MyLog.Enqueue(agvMap);
            }
        }

        //出队操作，获得数据
        public String GetMyLogList()
        {
            String agvmap = null;
            lock (this.MyLogLock)
            {
                bool has = (this.MyLog != null && this.MyLog.Count > 0);
                if (has)
                {
                    agvmap = this.MyLog.Dequeue() as String;
                }

            }
            return agvmap;
        }

        #endregion

       #region 用于在form显示连接消息，接收和发送的消息
        Queue<String> MessageShow = new Queue<String>();
        //加一个锁
        private Object MessageShowLock = new Object();

        //判断队列里是否有数据
        public bool IsMessageShowHasData()
        {
            bool ret = false;
            lock (this.MessageShowLock)
            {
                ret = (this.MessageShow != null && this.MessageShow.Count > 0);
            }
            return ret;
        }


        //判断队列里是否有数据
        public int GetMessageShowCount()
        {
            int ret = 0;
            lock (this.MessageShowLock)
            {
                ret = this.MessageShow.Count;
            }
            return ret;
        }

        //入队操作，增加数据
        public void AddMessageShowList(String agvMap)
        {

            lock (this.MessageShowLock)
            {
                this.MessageShow.Enqueue(agvMap);
            }
        }

        //出队操作，获得数据
        public String GetMessageShowList()
        {
            String agvmap = null;
            lock (this.MessageShowLock)
            {
                bool has = (this.MessageShow != null && this.MessageShow.Count > 0);
                if (has)
                {
                    agvmap = this.MessageShow.Dequeue() as String;
                }

            }
            return agvmap;
        }

#endregion

        

    }
}

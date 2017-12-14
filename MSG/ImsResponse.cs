using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGV_TASK.MSG
{
    public class ImsResponse
    {
        //记录消息将要发送到哪里
        //"PATH":表示发送到路径规划模块
        public string obj
        {
            get;
            set;
        }
        //表示将要发送的消息
        public string data
        {
            get;
            set;
        }

    }
}

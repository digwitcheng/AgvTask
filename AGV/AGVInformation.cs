using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Const;

namespace TASK.AGV
{
    public class AGVInformation
    {
        //小车编号
        public int Number;
        //始末位置
        public int BeginX;
        public int BeginY;
        public int EndX;
        public int EndY;
        public int DestX;
        public int DestY;
       // public int WorkNum;
        public int LWorkNum;
        public int RWorkNum;
        //电量
        public int Battery;
        //方向
        public Direction Dire;
        //状态
       // public V_State State;
        public State State;
        public string StartLoc;
        public string EndLoc;  

        //xzy 2018.3.11
        public int WorkStaionPassBy;
        //无参构造函数
        public AGVInformation()
        {
        }
    }
}

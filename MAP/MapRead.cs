using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGV_TASK;
using System.Xml;
using Const;

namespace TASK.XUMAP
{
 public   class MapRead
    {
        public MapRead()
        {
        }

        //地图宽度和高度
        public static int widthNum;
        public static int heightNum;

     //默认地图上的格子是从td0-0,td0-1排序
     //排队区入口数
     //public static int entranceNum = 6;
        public static int entranceNum;
     
     //左边工件台个数
        //public static int leftWorkstationNum = 8;
        public static int leftWorkstationNum;
        public static MAP[,] LeftWorkstation;
        public static MAP[] LeftWork;

     //右边工件台个数
       // public static int rightWorkstationNum = 9;
        public static int rightWorkstationNum;
     //数组里存放的是 第几个工件台，工件台的第几个入口
        public static MAP[,]RightWorkstation;
        public static MAP[] RightWork; 


       //扫描仪 
        public static int leftScannerNum;
        public static int rightScannerNum;
        public static MAP[] LeftScanner;
        public static MAP[] RightScanner;

       //投放口
      public static int destinationNum;
        public static MAP[] Destination;

        //休息区 
        public static int restnum; 
        public static MAP[] Rest;
        public static MAP[] RR;
       
        //public static int waiti;
        public static int lw, rw, krest, krscan, klscan; 
        //根据行号进行排序
        public void bubbleWait(MAP[] p, int n)
        {
            int i, j;
            for (i = 0; i < n; i++)
            {
                for (j = i; j < n; j++)
                {
                    if (p[i].x > p[j].x)
                    {
                        MAP tp = p[i];
                        p[i] = p[j];
                        p[j] = tp;
                    }
                    if (p[i].x == p[j].x)
                    {
                        if (p[i].y > p[j].y)
                        {
                            MAP tp = p[i];
                            p[i] = p[j];
                            p[j] = tp;
                        }
                    }
                }
            }
        }

        //根据列号进行排序
        public void bubbleRest(MAP[] p, int n)
        {
            int i, j;
            for (i = 0; i < n; i++)
            {
                for (j = i; j < n; j++)
                {
                    if (p[i].y > p[j].y)
                    {
                        MAP tp = p[i];
                        p[i] = p[j];
                        p[j] = tp;
                    }
                    if (p[i].y == p[j].y)
                    {
                        if (p[i].x > p[j].x)
                        {
                            MAP tp = p[i];
                            p[i] = p[j];
                            p[j] = tp;
                        }
                    }
                }
            }
        }

        public int trow = 0;
        public int flag = 0;  
        public int tcol=0;
        //归位后，小车一列一列放置在休息区中
        //每隔一列放置车
        //小车存放的位置存放在QQ数组0
        MAP[] RandRest(MAP[] PP, MAP[] QQ)
        {
            int i;
            int qqi = 0;

            for (i = 0; i < restnum; i++)
            {
                MAP tp = new MAP();
                tp = PP[i];
                if (tp.y == tcol)
                {
                    MAP tq=new MAP();
                    tq.x = tp.x;
                    tq.y = tp.y;
                  //  tq.occupy = false;
                    QQ[qqi]=tq;
                    qqi++;

                }
                if (tp.y > tcol)
                {
                    tcol = tcol + 2;
                    flag = flag + 1;
                }
            }
            krest = qqi;
            return QQ;

        }


        //小车在排队区应该位于队列的最后一个以及其他两个上去和下去的两个通道
        MAP[] RandWait(MAP[] PP, MAP[] WW)
        {
            int i = 0;
            int wwi = 0;
            int waiti = PP.Count();
           while(i<waiti)
            {
                if (i+2<waiti && PP[i].x == PP[i + 1].x  &&  PP[i].x!=PP[i+2].x)
                {
                    WW[wwi] = PP[i];
                    WW[wwi+1] = PP[i + 1];
                    wwi = wwi + 2;
                    i=i+2;
                }
                else if (i+7<waiti&& PP[i].x == PP[i + 7].x &&PP[i].y<40)
                {
                    WW[wwi++] = PP[i];
                    i = i + 7;
                }
                else if (i + 7 < waiti && PP[i].x == PP[i + 7].x && PP[i].y > 40)
                {
                    WW[wwi++] = PP[i+7];
                    i = i + 8;
                }
                else
                {
                    i++;
                }
            }
           WW[wwi++] = PP[waiti-2];
           WW[wwi++] = PP[waiti-1];
           if (PP[waiti - 1].y < 40)
           {
               lw = wwi;
           }
           else
           {
               rw = wwi;
           }
            return WW;

        }

        //读取休息区，排队区，投放口的格子信息
       public void MAP_classify( )
        {
            
            int resti = 0,desti = 0, lscani = 0, rscani = 0;
            //int lwaiti=0,rwaiti=0;
            int i;
            string tdname;
            string[] td;
            int tdx, tdy;

            string pathMap =System.Configuration.ConfigurationManager.AppSettings["MAPPath"].ToString();
            XmlDocument xmlfile = new XmlDocument();
            xmlfile.Load(pathMap);
           //地图的长，宽
            XmlNode map_w = xmlfile.SelectSingleNode("config/Map/widthNum");
            widthNum= Convert.ToInt32(map_w.InnerText); 
            XmlNode map_h = xmlfile.SelectSingleNode("config/Map/heightNum");
            heightNum = Convert.ToInt32(map_h.InnerText); 
           //排队区入口，左右工件台
            entranceNum = Convert.ToInt32(xmlfile.SelectSingleNode("config/Map/entranceNum").InnerText);
            leftWorkstationNum = Convert.ToInt32(xmlfile.SelectSingleNode("config/Map/leftWorkstationNum").InnerText);
            rightWorkstationNum = Convert.ToInt32(xmlfile.SelectSingleNode("config/Map/rightWorkstationNum").InnerText);
           // LeftWorkstation = new MAP[leftWorkstationNum*2, entranceNum];
           LeftWorkstation = new MAP[leftWorkstationNum, entranceNum];
           RightWorkstation = new MAP[rightWorkstationNum, entranceNum];
           LeftWork = new MAP[leftWorkstationNum];
           RightWork = new MAP[rightWorkstationNum];
           //左右扫描仪
           leftScannerNum = Convert.ToInt32(xmlfile.SelectSingleNode("config/Map/leftScannerNum").InnerText);
           rightScannerNum = Convert.ToInt32(xmlfile.SelectSingleNode("config/Map/rightScannerNum").InnerText);
           LeftScanner = new MAP[leftScannerNum];
           RightScanner = new MAP[rightScannerNum];
           //投放口
            XmlNode map_destination = xmlfile.SelectSingleNode("config/Map/Destnum");
            destinationNum = Convert.ToInt32(map_destination.InnerText);
            Destination = new MAP[destinationNum];
            //休息区
            XmlNode map_rest = xmlfile.SelectSingleNode("config/Map/Restnum");
            restnum = Convert.ToInt32(map_rest.InnerText);
            Rest = new MAP[restnum];
            
            //遍历获得休息区的位置信息
            XmlNodeList gridlist = xmlfile.SelectSingleNode("config/Grid").ChildNodes;

           //li第i个工件台，lj第j个排队区入口
           int li=0,lj=0,ri=0,rj=0;
            for (i = 0; i < gridlist.Count; i++)
            {

                if (gridlist[i].InnerText == "休息区")
                {
                    tdname = gridlist[i].Name;
                    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                    tdx = Convert.ToInt32(td[0]);
                    tdy = Convert.ToInt32(td[1]);

                    MAP trest = new MAP();
                    trest.x = tdx;
                    trest.y = tdy;
                  //  trest.occupy = false;
                    Rest[resti] = trest; 
                    resti++;
                }
                else if (gridlist[i].InnerText == "排队区入口")
                {
                    tdname = gridlist[i].Name;
                    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                    tdx = Convert.ToInt32(td[0]);
                    tdy = Convert.ToInt32(td[1]);

                    MAP twait = new MAP();
                    twait.x=tdx;
                    twait.y = tdy;
                    twait.agvNumOfQueuing = 0; 
                    //if (twait.y < (widthNum/2))
                    int side = 0;//0,只有右边;1,只有左边;2,两边
                    if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
                    else if (MapRead.rightWorkstationNum == 0) side = 1;
                    if (twait.y < (widthNum/side) && side != 0)
                    {
                        LeftWorkstation[li, lj] = twait; 
                        if (lj == (entranceNum-1))
                        {
                            LeftWork[li] = twait;//一个工件台的最后一个排队区入口
                            li++;
                            lj = 0;
                        }
                        else
                        {
                            lj++;
                        }
                    }
                    else
                    {
                       RightWorkstation[ri, rj] = twait; 
                        if (rj ==( entranceNum-1))
                        {
                            RightWork[ri] = twait;
                            ri++;
                            rj = 0;
                        }
                        else
                        {
                            rj++;
                        }
                    }
                } 
                else  if (gridlist[i].InnerText == "扫描仪")
                {
                    tdname = gridlist[i].Name;
                    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                    tdx = Convert.ToInt32(td[0]);
                    tdy = Convert.ToInt32(td[1]);

                    MAP tscan = new MAP();
                    tscan.x = tdx;
                    tscan.y = tdy;
                    tscan.agvNumOfQueuing = 0;
                   // tscan.occupy = false;
                   // if (tscan.y < (widthNum / 2))
                    int side = 0;//0,只有右边;1,只有左边;2,两边
                    if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
                    else if (MapRead.rightWorkstationNum == 0) side = 1;
                    if (tscan.y < (widthNum / side) && side != 0)
                    {
                        LeftScanner[lscani++] = tscan;
                    }
                    else
                    {
                        RightScanner[rscani++] = tscan;
                    }
                }
                else  if (gridlist[i].InnerText == "投放口")
                {
                    tdname = gridlist[i].Name;
                    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                    tdx = Convert.ToInt32(td[0]);
                    tdy = Convert.ToInt32(td[1]);

                    MAP tdest = new MAP();
                    tdest.x = tdx;
                    tdest.y = tdy;
                  //  tdest.occupy = false;
                    Destination[desti++] = tdest;
                }
                 
            }
            krscan = rscani;
            klscan = lscani;
            // 处理休息区
            bubbleRest(Rest, restnum);
            RR = new MAP[restnum];
            RandRest(Rest, RR);

            //处理排队区
            //bubbleWait(Lwait, lwaitnum);
            //LWW = new XUMAP[lwaitnum];
            //RandWait(Lwait, LWW);
         

            //bubbleWait(Rwait, rwaitnum);
            //RWW = new XUMAP[rwaitnum];
            //RandWait(Rwait,RWW);
           //处理投放口
            bubbleWait(Destination, destinationNum);
           
        }
        
    }
}

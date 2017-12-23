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
     //默认地图上的格子是从td0-0,td0-1排序
     //排队区入口数
        public static int Entrance = 6;
     
     //左边工件台个数
        public static int LWorkPlace = 8;
        public static MAP[,] lwork;
        public static MAP[] LW;

     //右边工件台个数
        public static int RWorkPlace = 9;
        public static MAP[,] rwork;
        public static MAP[] RW;


       //扫描仪
        public static MAP[] Lsc;
        public static MAP[] Rsc;

     //投放口
        public static MAP[] dest;

        //休息区
        public static MAP[] rest;
        public static MAP[] RR;
       


        //用于记录休息区，投放口的个数

        public static  int resti;

        //public static int waiti;
        public static int lw, rw, krest, krscan, klscan;

        //public static  int AGVConstDefine.desti;

     //地图宽度和高度
        public static int wnum;
        public static int hnum;


        //public static XUMAP[] Lwait;
        //public static XUMAP[] Rwait;
        //public static XUMAP[] LWW;
        //public static XUMAP[] RWW;
        //public static int lwaitnum;
        //public static int rwaitnum;

        public static int lscannum;
        public static int rscannum;

        public static int restnum;
        public static int destnum;
     
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

            for (i = 0; i < resti; i++)
            {
                MAP tp = new MAP();
                tp = PP[i];
                if (tp.y == tcol)
                {
                    MAP tq=new MAP();
                    tq.x = tp.x;
                    tq.y = tp.y;
                    tq.occupy = false;
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
            resti = 0;
            int desti = 0, lscani = 0, rscani = 0;
            //int lwaiti=0,rwaiti=0;
            int i;
            string tdname;
            string[] td;
            int tdx, tdy;

            string pathMap =System.Configuration.ConfigurationManager.AppSettings["MAPPath"].ToString();
            XmlDocument xmlfile = new XmlDocument();
            xmlfile.Load(pathMap);

            XmlNode map_w = xmlfile.SelectSingleNode("config/Map/Widthnum");
            wnum= Convert.ToInt32(map_w.InnerText);

            XmlNode map_h = xmlfile.SelectSingleNode("config/Map/Heightnum");
            hnum = Convert.ToInt32(map_h.InnerText);

            XmlNode REST = xmlfile.SelectSingleNode("config/Map/Restnum");
            restnum = Convert.ToInt32(REST.InnerText);
            rest = new MAP[restnum];


           // XmlNode LWAIT = xmlfile.SelectSingleNode("config/Map/LWaitnum");
           //lwaitnum = Convert.ToInt32(LWAIT.InnerText);
           // Lwait = new XUMAP[lwaitnum];
           Lsc = new MAP[LWorkPlace*2];

           // XmlNode RWAIT = xmlfile.SelectSingleNode("config/Map/RWaitnum");
           // rwaitnum = Convert.ToInt32(RWAIT.InnerText);
           // Rwait = new XUMAP[rwaitnum];
            Rsc = new MAP[RWorkPlace*2];

            XmlNode DEST = xmlfile.SelectSingleNode("config/Map/Destnum");
            destnum = Convert.ToInt32(DEST.InnerText);
            dest = new MAP[destnum];
            
            //遍历获得休息区的位置信息
            XmlNodeList gridlist = xmlfile.SelectSingleNode("config/Grid").ChildNodes;

            lwork = new MAP[LWorkPlace, Entrance];
            rwork = new MAP[RWorkPlace, Entrance];
            LW = new MAP[LWorkPlace];
            RW = new MAP[RWorkPlace];
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
                    trest.occupy = false;
                    rest[resti] = trest; 
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
                    twait.agventer = 0;

                    twait.occupy = false;
                    if (twait.y < (wnum/2))
                    {
                        lwork[li, lj] = twait;
                        //LW[li].agventer = 0;
                        if (lj == (Entrance-1))
                        {
                            LW[li] = twait;
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
                        rwork[ri, rj] = twait;
                       // RW[ri].agventer = 0;
                        if (rj ==( Entrance-1))
                        {
                            RW[ri] = twait;
                            ri++;
                            rj = 0;
                        }
                        else
                        {
                            rj++;
                        }
                    }
                }
                //else if (gridlist[i].InnerText == "排队区")
                //{
                //    tdname = gridlist[i].Name;
                //    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                //    tdx = Convert.ToInt32(td[0]);
                //    tdy = Convert.ToInt32(td[1]);

                //    XUMAP twait = new XUMAP();
                //    twait.x = tdx;
                //    twait.y = tdy;
                //    twait.occupy = false;
                //    if (twait.y < 40)
                //    {
                //        Lwait[lwaiti++] = twait;
                //    }
                //    else
                //    {
                //        Rwait[rwaiti++] = twait;
                //    }
                //}
                else  if (gridlist[i].InnerText == "扫描仪")
                {
                    tdname = gridlist[i].Name;
                    td = tdname.Split(new string[] { "td", "-" }, StringSplitOptions.RemoveEmptyEntries);
                    tdx = Convert.ToInt32(td[0]);
                    tdy = Convert.ToInt32(td[1]);

                    MAP tscan = new MAP();
                    tscan.x = tdx;
                    tscan.y = tdy;
                    tscan.agventer = 0;
                    tscan.occupy = false;
                    if (tscan.y < 40)
                    {
                        Lsc[lscani++] = tscan;
                    }
                    else
                    {
                        Rsc[rscani++] = tscan;
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
                    tdest.occupy = false;
                    dest[desti++] = tdest;
                }
                 
            }
            krscan = rscani;
            klscan = lscani;
            // 处理休息区
            bubbleRest(rest, resti);
            RR = new MAP[restnum];
            RandRest(rest, RR);

            //处理排队区
            //bubbleWait(Lwait, lwaitnum);
            //LWW = new XUMAP[lwaitnum];
            //RandWait(Lwait, LWW);
         

            //bubbleWait(Rwait, rwaitnum);
            //RWW = new XUMAP[rwaitnum];
            //RandWait(Rwait,RWW);
           //处理投放口
            bubbleWait(dest, destnum);
           
        }
        
    }
}

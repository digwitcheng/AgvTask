using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASK.AGV;
using Const;
using AGV_TASK;
using TASK.MAP;
using System.Threading;
using System.Xml;

namespace TASK.AGV
{
    public class AGVDestination
    {

        private static int[] wAgv =new int [MapRead.LWorkPlace+MapRead.RWorkPlace];
        private const int MIN_COUNT = 4;

        //  MapRead MapRead = new MapRead();
        public static int seed = 400;
        static Random rd = new Random(seed);

        public static AGVInformation Confirm_EndPoint(AGVInformation agv, string startloc, int x, int y, string endloc)
        {
            if (endloc == "RestArea")
            {
                if (agv.State == State.cannotToDestination)
                {
                    CanToRest(agv, x, y);
                }
                //休息1.0
                ToRest(agv, x, y);
            }
            else if (endloc == "WaitArea")
            {

                if (startloc != "DestArea")
                {
                    //表演1.0
                    RandToWait(agv, x, y);

                }
                else if (startloc == "DestArea")
                {
                    DestToWait(agv, x, y);
                }
            }
            else if (startloc == "WaitArea" && endloc == "ScanArea")
            {
                //表演2.0
                WaitToScan(agv, x, y);
            }
            else if (startloc == "ScanArea" && endloc == "DestArea")
            {
                //表演3.0
                ScanToDest(agv, x, y);
            }
            else if (startloc == "DestArea" && endloc == "DestArea")
            {
                agv.EndX = agv.BeginX;
                agv.EndY = agv.BeginY;
                if (agv.State == State.unloading)
                {
                    agv.State = State.unloading;
                }
                else if (agv.State == State.free)
                {
                    agv.State = State.free;
                }


            }
            else if (startloc == "DestArea" && endloc == "RestArea")
            {
                DestToRest(agv, x, y);
            }


            return agv;
        }
        //按照小车编号，按照顺序

        public static int randSeed = 200;
        public static void CanToRest(AGVInformation agv, int x, int y)
        {
            int agvnum = agv.Number;

            int canRest;
            Random r = new Random(randSeed);
            if (randSeed < 500)
            {
                randSeed++;
            }
            else
            {
                randSeed = 200;
            }
            canRest = r.Next(0, MapRead.krest);
            agv.EndX = MapRead.RR[canRest].x;
            agv.EndY = MapRead.RR[canRest].y;
            agv.Dire = Direction.Right;
            //agv.StartLoc = agv.StartLoc;
            agv.EndLoc = "RestArea";
            agv.State = State.free;
        }
        public static void ToRest(AGVInformation agv, int x, int y)
        {
            int agvnum = agv.Number;
            agv.EndX = MapRead.RR[agvnum % (MapRead.krest)].x;
            agv.EndY = MapRead.RR[agvnum % (MapRead.krest)].y;
            agv.Dire = Direction.Right;
            //agv.StartLoc = agv.StartLoc;
            agv.EndLoc = "RestArea";
            agv.State = State.free;
        }

        //三个工件台就近最少排队原则
        public static  int  ThreeLeast(AGVInformation agv, int x, int y)
        {
            int workStart = 0;
            

            int agvnum = agv.Number;
            int choosework = 0;
            if (y < (MapRead.wnum / 2))
            {
                //选定三个相邻的工件台
                #region
                for (int i = 0; i < MapRead.LWorkPlace; i++)
                   // for (int j = 0; j < MapRead.Entrance; j++)
                    {
                       // if (System.Math.Abs(x - MapRead.lwork[i, j].x) <= 3)
                        if (System.Math.Abs(x - MapRead.LW[i].x) <= 6)
                        {
                            if (i == 0)
                            {
                                workStart = i;
                                break;
                            }
                            else if (i > 0 && i + 1 < MapRead.LWorkPlace)
                            {
                                workStart = i - 1;
                                break;
                            }
                            else
                            {
                                workStart = i - 2;
                                break;
                            }
                        }
                    }
                #endregion
                //选择最少被锁定排队的工件台
                //int choosework = 0;
                if (MapRead.LW[workStart].agventer < MapRead.LW[workStart + 1].agventer)
                {
                    choosework = workStart;
                }
                else
                {
                    if (MapRead.LW[workStart + 1].agventer < MapRead.LW[workStart + 2].agventer)
                    {
                        choosework = workStart + 1;
                    }
                    else
                    {
                        choosework = workStart + 2;
                    }
                }
                //return choosework;
            }
            else if(y>=(MapRead.wnum/2))
            {
                //选定三个相邻的工件台
                #region
                for (int i = 0; i < MapRead.RWorkPlace; i++)
                    //for (int j = 0; j < MapRead.Entrance; j++)
                    {
                       // if (System.Math.Abs(x - MapRead.rwork[i, j].x) <= 3)
                        if (System.Math.Abs(x - MapRead.RW[i].x) <= 6)
                        {
                            if (i == 0)
                            {
                                workStart = i;
                                break;
                            }
                            else if (i > 0 && i + 1 < MapRead.RWorkPlace)
                            {
                                workStart = i - 1;
                                break;
                            }
                            else
                            {
                                workStart = i - 2;
                                break;
                            }
                        }
                    }
                #endregion
                //选择最少被锁定排队的工件台
                //int choosework = 0;
                if (MapRead.RW[workStart].agventer < MapRead.RW[workStart + 1].agventer)
                {
                    choosework = workStart;
                }
                else
                {
                    if (MapRead.RW[workStart + 1].agventer < MapRead.RW[workStart + 2].agventer)
                    {
                        choosework = workStart + 1;
                    }
                    else
                    {
                        choosework = workStart + 2;
                    }
                }
                choosework += MapRead.LWorkPlace;
                //return choosework;
            }
            return choosework;
        }
        /// <summary>
        ///如果工作台的agv小车相差过大，直接就把agv分配给少的，不管离工作台近不近，
        /// </summary>
        /// <returns>相差不大就返回-1，不用考虑均匀分配问题，否则直接返回工作台id</returns>
        static int AgvLeast()
        {
            int min = int.MaxValue;
            int minIndex=-1;
            int max = int.MinValue;
            for (int i = 0; i < wAgv.Length; i++)
            {
                if (min > wAgv[i])
                {
                    min = wAgv[i];
                    minIndex=i;
                }
                if (max < wAgv[i])
                {
                    max = wAgv[i];
                }
            }
            if (max - min > MIN_COUNT)
            {
                return minIndex;
            }
            else
            {
                return -1;
            }

        }

        public static  int workj = 0;
        //按照小车编号，按照顺序
        public static void RandToWait(AGVInformation agv, int x, int y)
        {


            int agvnum = agv.Number;
            //小车
            //if (y < (MapRead.wnum/2) )
            //{
            //    agv.EndX = MapRead.LWW[agvnum % MapRead.lw ].x;
            //    agv.EndY = MapRead.LWW[agvnum % MapRead.lw].y;
            //}
            //else
            //{
            //    agv.EndX = MapRead.RWW[agvnum % MapRead.rw].x;
            //    agv.EndY = MapRead.RWW[agvnum % MapRead.rw].y;
            //}
            //agv.StartLoc = "RandArea";
            //agv.EndLoc = "WaitArea";
            //agv.State = State.free;


           int worki= AgvLeast();
           if (worki < 0 || worki>= wAgv.Length)
           {
            worki = ThreeLeast(agv, x, y);

           }

       
           // agv.WorkNum = worki;
            if (worki<MapRead.LWorkPlace)
            {
               // int worki = agvnum % MapRead.LWorkPlace;
                agv.EndX = MapRead.lwork[worki,workj].x;
                agv.EndY = MapRead.lwork[worki,workj].y;
                MapRead.LW[worki].agventer++;
                agv.LWorkNum = worki;
                agv.RWorkNum = -1;
                wAgv[worki]++;
            }
            else
            {
                 worki = worki-MapRead.LWorkPlace;
              // int worki = agvnum % MapRead.RWorkPlace;
                agv.EndX = MapRead.rwork[worki, workj].x;
                agv.EndY = MapRead.rwork[worki, workj].y;
                MapRead.RW[worki].agventer++;
                agv.RWorkNum = worki;
                agv.LWorkNum = -1;
                wAgv[worki + MapRead.LWorkPlace]++;
            }
            if (workj == (MapRead.Entrance-1))
            {

                workj = 0;
            }
            else
            {
                workj++;
            }

            //agv.StartLoc = "RandArea";
            agv.EndLoc = "WaitArea";
            agv.State = State.free;

        }

        public static void DestToWait(AGVInformation agv, int x, int y)
        {
              int agvnum = agv.Number;
            ////就近,在地图的左边
            //    if (y <= 45)
            //    {
            //        int i = 0;
            //        int lcnt = MapRead.lw;
            //        //while(i<lcnt&&MapRead.LWW[i]!=null)
            //        for(i=0;i<lcnt;i++)
            //        {
            //            //判断地图中DestArea与WaitArea的行数是否相差1

            //            int lnum = i % lcnt;
            //            if (System.Math.Abs(MapRead.LWW[lnum].x - agv.BeginX) == 0
            //                || System.Math.Abs(MapRead.LWW[lnum].x - agv.BeginX) == 1
            //                || System.Math.Abs(MapRead.LWW[lnum].x - agv.BeginX) == 3

            //               )
            //            {
            //                agv.EndX = MapRead.LWW[lnum].x;
            //                agv.EndY = MapRead.LWW[lnum].y;
            //                agv.StartLoc = "DestArea";
            //                agv.EndLoc = "WaitArea";
            //                agv.Dire = Direction.Right;
            //                //MapRead.WW[i].occupy = true; 
            //                break;
            //            }
            //            //else
            //            //{
            //            //    i++;
            //            //}
            //        }
            //    }
            //    //地图在右边
            //    else
            //    {
            //        int i = 0, rcnt = MapRead.rw;
            //       // while(i<rcnt && MapRead.RWW[i]!=null)
            //        for (i = 0; i <MapRead.RWW.Count(); i++)
            //        {
            //            int rnum = i % rcnt;
            //            if (System.Math.Abs(MapRead.RWW[rnum].x- agv.BeginX) == 0
            //                || System.Math.Abs(MapRead.RWW[rnum].x - agv.BeginX) == 1
            //                || System.Math.Abs(MapRead.RWW[rnum].x - agv.BeginX) == 3
            //                )
            //            {
            //                agv .EndX = MapRead.RWW[rnum].x;
            //                agv.EndY = MapRead.RWW[rnum].y;
            //                agv.StartLoc = "DestArea";
            //                agv.EndLoc = "WaitArea";
            //                agv.Dire = Direction.Left;
            //              //  MapRead.WW[i].occupy = true;
            //                break;
            //            }
            //            i++;
            //        }
            //    }
            //    agv.State = State.free;
            //MapRead.dest[AGVConstDefine.p[agvnum].destnum].occupy = false;

              int worki = AgvLeast();
              if (worki < 0 || worki >= wAgv.Length)
              {
                  worki = ThreeLeast(agv, x, y);

              }
            //agv.WorkNum=worki;
              if (worki < MapRead.LWorkPlace)
              {
               // int worki = agvnum % MapRead.LWorkPlace;
                agv.EndX = MapRead.lwork[worki, workj].x;
                agv.EndY = MapRead.lwork[worki, workj].y;
                MapRead.LW[worki].agventer++;
                agv.LWorkNum = worki;
                agv.RWorkNum = -1;
                wAgv[worki]++;
            }
            else
            {
                worki -= MapRead.LWorkPlace;
               //int worki = agvnum % MapRead.RWorkPlace;
                agv.EndX = MapRead.rwork[worki, workj].x;
                agv.EndY = MapRead.rwork[worki, workj].y;
                MapRead.RW[worki].agventer++;
                agv.RWorkNum = worki;
                agv.LWorkNum = -1;
                wAgv[worki + MapRead.LWorkPlace]++;
            }
            if (workj == (MapRead.Entrance - 1))
            {

                workj = 0;
            }
            else
            {
                workj++;
            }
            agv.StartLoc = "DestArea";
            agv.EndLoc = "WaitArea";
            agv.State = State.free;
        }

        public static void DestToRest(AGVInformation agv, int x, int y)
        {
            int agvnum = agv.Number % MapRead.krest;
            agv.EndX = MapRead.RR[agvnum].x;
            agv.EndY = MapRead.RR[agvnum].y;
            agv.Dire = Direction.Right;
            agv.State = State.free;
            agv.StartLoc = "DestArea";
            agv.EndLoc = "RestArea";
        }

        public static void WaitToScan(AGVInformation agv, int x, int y)
        {           
            int agvnum = agv.Number;
            //往前走
            string pathMap = System.Configuration.ConfigurationManager.AppSettings["MAPPath"].ToString();
            XmlDocument xmlfile = new XmlDocument();
            xmlfile.Load(pathMap);
            string agvxy = "config/Grid/td" + x.ToString() + "-" + y.ToString();
            XmlElement td = (XmlElement)xmlfile.SelectSingleNode(agvxy);
            string tdattr = td.Attributes["direction"].InnerText;

            string agvxy1 = "config/Grid/td" + x.ToString() + "-" + (y + 1).ToString();
            XmlElement td1 = (XmlElement)xmlfile.SelectSingleNode(agvxy1);
            string tdattr1 = td.Attributes["direction"].InnerText;
            if (agv.BeginY < 50)
            {
                for (int i = 0; i < MapRead.klscan; i++)
                {
                    if (System.Math.Abs(MapRead.Lsc[i].x - agv.BeginX) == 0
                        || System.Math.Abs(MapRead.Lsc[i].x - agv.BeginX) == 1
                        )
                    {
                        agv.EndX = MapRead.Lsc[i].x;
                        agv.EndY = MapRead.Lsc[i].y;
                        agv.Dire = Direction.Left;
                        //  MapRead.WW[i].occupy = true;
                        break;
                    }
                }
            }

            else
            {
                for (int i = 0; i < MapRead.krscan; i++)
                {
                    if (System.Math.Abs(MapRead.Rsc[i].x - agv.BeginX) == 0
                        || System.Math.Abs(MapRead.Rsc[i].x - agv.BeginX) == 1
                        )
                    {
                        agv.EndX = MapRead.Rsc[i].x;
                        agv.EndY = MapRead.Rsc[i].y;
                        agv.Dire = Direction.Right;
                        //  MapRead.WW[i].occupy = true;
                        break;
                    }
                }
            }

            agv.StartLoc = "WaitArea";
            agv.EndLoc = "ScanArea";
            agv.State = State.free;
        }

        public static void ScanToDest(AGVInformation agv, int x, int y)
        {

        
            int agvnum = agv.Number;
            //选中任意一个DestArea
          //  Random rd = new Random(seed);
            int destagvnum;
            int tx, ty;
            int ss = MapRead.destnum;
            destagvnum = rd.Next(0, ss);
            //已被占用
            //OccuFlag记录被占的投放口
            //while (MapRead.dest[destagvnum].occupy == true)
            //{
            //    if (seed < 1000)
            //    {
            //        seed++;
            //    }
            //    else
            //    {
            //        seed = (seed + 400) % 1000;
            //    }
            //    Random rd1 = new Random(seed);
            //    destagvnum = rd1.Next(0, MapRead.destnum);
            //}
            //记录的是第agvnum个小车对应的投放口的区域
            AGVConstDefine.DEST tp = new AGVConstDefine.DEST(); ;


            tp.destnum = destagvnum;
            AGVConstDefine.p[agvnum] = tp;
            tx = MapRead.dest[destagvnum].x;
            ty = MapRead.dest[destagvnum].y;
            //小车在DestArea下面
            if (agv.BeginX > tx)
            {
                agv.Dire = Direction.Up;
                agv.EndX = tx;
                agv.EndY = ty - 1;
                agv.DestX = tx;
                agv.DestY = ty;
            }
            else
            {
                agv.Dire = Direction.Down;
                agv.EndX = tx;
                agv.EndY = ty + 1;
                agv.DestX = tx;
                agv.DestY = ty;
            }
             //MapRead.dest[destagvnum].occupy = true;

           //  MapRead.dest[agv.Number].occupy = true;

            agv.StartLoc = "ScanArea";
            agv.EndLoc = "DestArea";
            agv.State = State.carried;
           // if (y < (MapRead.wnum / 2))
           // {
            if (agv.LWorkNum != -1 && MapRead.LW[agv.LWorkNum].agventer > 0)
            {
                wAgv[agv.LWorkNum]--;
                int la = MapRead.LW[agv.LWorkNum].agventer;
                MapRead.LW[agv.LWorkNum].agventer--;
                agv.LWorkNum = -1;
            }

           ////////// }
           ////////// else
           ////////// {
            else if (agv.RWorkNum != -1 && MapRead.RW[agv.RWorkNum].agventer > 0)
            {
                wAgv[agv.RWorkNum + MapRead.LWorkPlace]--;
                int ra = MapRead.RW[agv.RWorkNum].agventer;
                MapRead.RW[agv.RWorkNum].agventer--;
                agv.RWorkNum = -1;
            }
          //  }
        }

        public AGVDestination()
        {
        }
    }
}
 

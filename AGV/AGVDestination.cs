using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASK.AGV;
using Const;
using AGV_TASK;
using TASK.XUMAP;
using System.Threading;
using System.Xml;

namespace TASK.AGV
{
    public class AGVDestination
    {

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
                    //工作1.0
                    RandToWait(agv, x, y);

                }
                else if (startloc == "DestArea")
                {
                    DestToWait(agv, x, y);
                }
            }
            else if (startloc == "WaitArea" && endloc == "ScanArea")
            {
                //工作2.0
                WaitToScan(agv, x, y);
            }
            else if (startloc == "ScanArea" && endloc == "DestArea")
            {
                //工作3.0
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

        //选择较近的三个工件台中的最少排队的一个工件台
        public static  int  ThreeLeast(AGVInformation agv, int x, int y)
        {
            int agvnum = agv.Number;
            int choosework = 0;

            int side = 0;//0,只有右边;1,只有左边;2,两边
            if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
            else if (MapRead.rightWorkstationNum == 0) side = 1; 

            if (y < (MapRead.widthNum / side)  && side!=0)
            {
                //锁定较近的工件台
                #region
                int workStart = NearestWorkStation(MapRead.LeftWork, x);
                #endregion
                //再选当前排队中最少的一个工件台
                //int choosework = 0;
                choosework = LeastEntrance(MapRead.LeftWork,workStart);
            }
            else
            {
                //选定三个相邻的工件台
                int workStart = NearestWorkStation(MapRead.RightWork, x);
                //选择最少被锁定排队的工件台
                //int choosework = 0;
                choosework = LeastEntrance(MapRead.RightWork,workStart);
            }
            return choosework;
        }
        static int NearestWorkStation(MAP[] MAPArray,int x)
        {
            int workStart = 0;

            //for (int i = 0; i < MapRead.rightWorkstationNum; i++)
            ////for (int j = 0; j < MapRead.entranceNum; j++)
            //{
            //    // if (System.Math.Abs(x - MapRead.RightWorkstation[i, j].x) <= 3)
            //    if (System.Math.Abs(x - MapRead.RightWork[i].x) <= 6)
            //    {
            //        if (i == 0)
            //        {
            //            workStart = i;
            //            break;
            //        }
            //        else if (i > 0 && i + 1 < MapRead.rightWorkstationNum)
            //        {
            //            workStart = i - 1;
            //            break;
            //        }
            //        else
            //        {
            //            workStart = i - 2;
            //            break;
            //        }
            //    }
            //}

            int min = int.MaxValue;
            for (int i = 0; i < MAPArray.Length; i++)
            //for (int j = 0; j < MapRead.entranceNum; j++)
            {
                // if (System.Math.Abs(x - MapRead.RightWorkstation[i, j].x) <= 3)
                if (Math.Abs(x - MAPArray[i].x) < min)
                {
                    min = Math.Abs(x - MAPArray[i].x);
                    workStart = i;
                }
            }
            /* hxc 2017.12
            if (workStart > 0 && workStart < MAPArray.Length - 1)
            {
                workStart = workStart - 1;
            }
            if (workStart == MAPArray.Length - 2)
            {
                workStart = workStart - 1;
            }
            */

            //xzy 2018.3.11 
            if (workStart > 0 && workStart < MAPArray.Length - 1)
            {
                workStart = workStart - 1;
            }
            else if (workStart == MAPArray.Length - 1)
            {
                //xzy 2018.3.11 考虑工件台个数
                if (MAPArray.Length >= 3) workStart = workStart - 2;
                else if (MAPArray.Length < 3 ) workStart = 0;
            }

            return workStart;
        }
       static int LeastEntrance(MAP[] MAPArray,int workStart)
        {
            //int choosework = 0;

            //if (MAPArray[workStart].agvNumOfQueuing < MAPArray[workStart + 1].agvNumOfQueuing)
            //{
            //    choosework = workStart;
            //}
            //else
            //{
            //    if (MAPArray[workStart + 1].agvNumOfQueuing < MAPArray[workStart + 2].agvNumOfQueuing)
            //    {
            //        choosework = workStart + 1;
            //    }
            //    else
            //    {
            //        choosework = workStart + 2;
            //    }
            //}
            //return choosework;


            int min = int.MaxValue;
            int minIndex = 0;
            int WorkStationNumSort = 0;
           //xzy 2018.3.11 考虑工件台个数
            if (MAPArray.Length >= 3) WorkStationNumSort = 3;
            else WorkStationNumSort = MAPArray.Length;

            for (int i = workStart; i < workStart + WorkStationNumSort; i++)
            {
                if (min > MAPArray[i].agvNumOfQueuing)
                {
                    min = MAPArray[i].agvNumOfQueuing;
                    minIndex = i;
                }
            }
            return minIndex;
        }


      
        //按照小车编号，按照顺序
        public static void RandToWait(AGVInformation agv, int x, int y)
        {
            int agvnum = agv.Number;
            int worki = ThreeLeast(agv, x, y), workj = 0;
            agv.WorkStaionPassBy = worki;
           // agv.WorkNum = worki;
            //xzy 2017 排队区有左右两边
           // if (y < (MapRead.widthNum / 2))

            //xzy 2018.3 考虑排队区是否在左右两边
            int side = 0 ;//0,只有右边;1,只有左边;2,两边
            if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
            else if (MapRead.rightWorkstationNum == 0) side = 1; 

            if (y < (MapRead.widthNum / side) &&  side !=0 )
            {
                while (MapRead.LeftWorkstation[worki, workj].occupy == true)
                {
                    if (workj == (MapRead.entranceNum - 1))
                    {
                        workj = 0;
                        for (int i = 0; i < MapRead.entranceNum; i++)
                            MapRead.LeftWorkstation[worki, i].occupy = false;
                    }
                    else
                    { workj++; }
                }
                agv.EndX = MapRead.LeftWorkstation[worki,workj].x;
                agv.EndY = MapRead.LeftWorkstation[worki, workj].y;
                MapRead.LeftWork[worki].agvNumOfQueuing++;
                MapRead.LeftWorkstation[worki, workj].occupy = true;
                agv.LWorkNum = worki;
                agv.RWorkNum = -1;
            }
            else 
            {
                while (MapRead.RightWorkstation[worki, workj].occupy == true)
                {
                    if (workj == (MapRead.entranceNum - 1))
                    {
                        workj = 0;
                        for (int i = 0; i < MapRead.entranceNum; i++)
                            MapRead.RightWorkstation[worki, i].occupy = false;
                    }
                    else
                    { workj++; }
                }
              // int worki = agvnum % MapRead.rightWorkstationNum;
                agv.EndX = MapRead.RightWorkstation[worki, workj].x;
                agv.EndY = MapRead.RightWorkstation[worki, workj].y;
                MapRead.RightWork[worki].agvNumOfQueuing++;
                MapRead.RightWorkstation[worki, workj].occupy = true;
                agv.RWorkNum = worki;
                agv.LWorkNum = -1;
             //   MapRead.RightWorkstation[worki, workj].occupy = true;
            }
            //while (MapRead.RightWorkstation[worki, workj].occupy == true)
            //{ 
            //    workj++;
            //    if (workj == (MapRead.entranceNum - 1))
            //    {

            //        workj = 0;
            //    }
            //}
            //if (workj == (MapRead.entranceNum-1))
            //{

            //    workj = 0;
            //}
            //else
            //{
            //    workj++;
            //}

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
            //MapRead.Destination[AGVConstDefine.p[agvnum].destinationNum].occupy = false;

              //int worki = ThreeLeast(agv, x, y);
              int worki = ThreeLeast(agv, x, y), workj = 0;
              agv.WorkStaionPassBy = worki;
            
            //agv.WorkNum=worki;
            //xzy 2017 if (y < (MapRead.widthNum / 2))

              //xzy 2018.3 考虑排队区是否在左右两边
              int side = 0;//0,只有右边;1,只有左边;2,两边
              //左右两边
              if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
              //左边
              else if (MapRead.rightWorkstationNum == 0) side = 1;

              if (y < (MapRead.widthNum / side) && side != 0)
            {
                while (MapRead.LeftWorkstation[worki, workj].occupy == true)
                {
                    if (workj == (MapRead.entranceNum - 1))
                    {
                        workj = 0;
                        for (int i = 0; i < MapRead.entranceNum; i++)
                            MapRead.LeftWorkstation[worki, i].occupy = false;
                    }
                    else
                    { workj++; }
                }
               // int worki = agvnum % MapRead.leftWorkstationNum;
                agv.EndX = MapRead.LeftWorkstation[worki, workj].x;
                agv.EndY = MapRead.LeftWorkstation[worki, workj].y;
                MapRead.LeftWork[worki].agvNumOfQueuing++;
                agv.LWorkNum = worki;
                agv.RWorkNum = -1;
                MapRead.LeftWorkstation[worki, workj].occupy = true;
            }
            else
            {
                while (MapRead.RightWorkstation[worki, workj].occupy == true)
                {
                    if (workj == (MapRead.entranceNum - 1))
                    {
                        workj = 0;
                        for (int i = 0; i < MapRead.entranceNum; i++)
                            MapRead.RightWorkstation[worki, i].occupy = false;
                    }
                    else
                    { workj++; }
                }
               //int worki = agvnum % MapRead.rightWorkstationNum;
                agv.EndX = MapRead.RightWorkstation[worki, workj].x;
                agv.EndY = MapRead.RightWorkstation[worki, workj].y;
                MapRead.RightWork[worki].agvNumOfQueuing++;
                agv.RWorkNum = worki;
                agv.LWorkNum = -1;
                MapRead.RightWorkstation[worki, workj].occupy = true;
            }
            
            //if (workj == (MapRead.entranceNum - 1))
            //{

            //    workj = 0;
            //}
            //else
            //{
            //    workj++;
            //}
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
           //xzy 2017if (agv.BeginY < 50)

            //xzy 2018.3.11
            int side = 0;//0,只有右边;1,只有左边;2,两边
            if (MapRead.leftWorkstationNum != 0 && MapRead.rightWorkstationNum != 0) side = 2;
            else if (MapRead.rightWorkstationNum == 0) side = 1;
            if (agv.BeginY < (MapRead.widthNum / side) && side != 0)
            {
                for (int i = 0; i < MapRead.klscan; i++)
                {
                    if (System.Math.Abs(MapRead.LeftScanner[i].x - agv.BeginX) == 0
                      //  || System.Math.Abs(MapRead.LeftScanner[i].x - agv.BeginX) == 1
                        )
                    {
                        agv.EndX = MapRead.LeftScanner[i].x;
                        agv.EndY = MapRead.LeftScanner[i].y;
                        agv.Dire = Direction.Left;
                        //xzy 2018.3.11 该工件台排队数减少1
                        MapRead.LeftWork[agv.WorkStaionPassBy].agvNumOfQueuing--;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < MapRead.krscan; i++)
                {
                    if (System.Math.Abs(MapRead.RightScanner[i].x - agv.BeginX) == 0
                       // || System.Math.Abs(MapRead.RightScanner[i].x - agv.BeginX) == 1
                        )
                    {
                        agv.EndX = MapRead.RightScanner[i].x;
                        agv.EndY = MapRead.RightScanner[i].y;
                        agv.Dire = Direction.Right;
                        MapRead.RightWork[agv.WorkStaionPassBy].agvNumOfQueuing--;
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
            int ss = MapRead.destinationNum;
            destagvnum = rd.Next(0, ss);
            //已被占用
            //OccuFlag记录被占的投放口
            //while (MapRead.Destination[destagvnum].occupy == true)
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
            //    destagvnum = rd1.Next(0, MapRead.destinationNum);
            //}
            //记录的是第agvnum个小车对应的投放口的区域
            AGVConstDefine.DEST tp = new AGVConstDefine.DEST();
            tp.destinationNum = destagvnum;
            AGVConstDefine.p[agvnum] = tp;
            tx = MapRead.Destination[destagvnum].x;
            ty = MapRead.Destination[destagvnum].y;
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
             //MapRead.Destination[destagvnum].occupy = true;

           //  MapRead.Destination[agv.Number].occupy = true;

            agv.StartLoc = "ScanArea";
            agv.EndLoc = "DestArea";
            agv.State = State.carried;
           // if (y < (MapRead.widthnum / 2))
           // {
            if (agv.LWorkNum != -1 && MapRead.LeftWork[agv.LWorkNum].agvNumOfQueuing > 0)
            {
                int la = MapRead.LeftWork[agv.LWorkNum].agvNumOfQueuing;
                MapRead.LeftWork[agv.LWorkNum].agvNumOfQueuing--;
                agv.LWorkNum = -1;
            }

           ////////// }
           ////////// else
           ////////// {
            else if (agv.RWorkNum != -1 && MapRead.RightWork[agv.RWorkNum].agvNumOfQueuing > 0)
            {
                int ra = MapRead.RightWork[agv.RWorkNum].agvNumOfQueuing;
                MapRead.RightWork[agv.RWorkNum].agvNumOfQueuing--;
                agv.RWorkNum = -1;
            }
          //  }
        }

        public AGVDestination()
        {
        }
    }
}
 

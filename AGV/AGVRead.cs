using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TASK.AGV;
using Const;

namespace TASK.AGV
{
    class AGVRead
    {
         
        public void AGV_Read()
        {

            string pathAGV = System.Configuration.ConfigurationManager.AppSettings["AGVPath"].ToString(); ;
            XmlDocument xmlfile = new XmlDocument();
            xmlfile.Load(pathAGV);
            XmlNode agvsum = xmlfile.SelectSingleNode("Info/SUM");
            int sum = Convert.ToInt32(agvsum.InnerText);
            AGVConstDefine.AGVSUM = sum;
            AGVConstDefine.AGV = new AGVInformation[AGVConstDefine.AGVSUM];

            XmlNodeList agvlist = xmlfile.SelectSingleNode("Info/AGV_info").ChildNodes;
           
            int i;
            //for (i = 0; i < agvlist.Count; i++)
            for (i = 0; i < sum; i++)
            {
             AGVInformation tagv = new AGVInformation();
            tagv.Number = Convert.ToInt32(agvlist[i].Attributes["Num"].InnerText.ToString()); ;
            tagv.BeginX = Convert.ToInt32(agvlist[tagv.Number].Attributes["BeginX"].InnerText.ToString());
            tagv.BeginY = Convert.ToInt32(agvlist[tagv.Number].Attributes["BeginY"].InnerText.ToString());
            tagv.StartLoc = agvlist[tagv.Number].Attributes["StartLoc"].InnerText;
            string d = agvlist[tagv.Number].Attributes["Direction"].InnerText.ToString();
            tagv.Dire =(Const.Direction)Enum.Parse(typeof(Direction),d);
            tagv.State = (Const.State)Enum.Parse(typeof(State), agvlist[tagv.Number].Attributes["State"].InnerText);
            tagv.Battery = Convert.ToInt32(agvlist[tagv.Number].Attributes["Battery"].InnerText.ToString());
            if (agvlist[tagv.Number].Attributes["EndX"].InnerText != null && agvlist[tagv.Number].Attributes["EndY"].InnerText != null && agvlist[tagv.Number].Attributes["State"].InnerText=="carried")

            {
                tagv.EndX = Convert.ToInt32(agvlist[tagv.Number].Attributes["EndX"].InnerText.ToString());
                tagv.EndY = Convert.ToInt32(agvlist[tagv.Number].Attributes["EndY"].InnerText.ToString());
                tagv.DestX = Convert.ToInt32(agvlist[tagv.Number].Attributes["DestX"].InnerText.ToString());
                tagv.DestY = Convert.ToInt32(agvlist[tagv.Number].Attributes["DestY"].InnerText.ToString());
            }
            AGVConstDefine.AGV[tagv.Number] = tagv;
            }

        }
    }
}

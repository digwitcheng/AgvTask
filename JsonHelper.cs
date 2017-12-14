using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASK.AGV;

namespace AGV_V1._0
{
    class JsonHelper
    {
        //public static   string ConvertToJson(List<Vehicle> v, int tPtr)
        //{
        //    SendData[] sendObj = new SendData[v.Count()];
        //    for (int i = 0; i < v.Count(); i++)
        //    {
        //        SendData obj = GetSendObj(v[i], tPtr);
        //        sendObj[i] = obj;
        //    }

        //    return Newtonsoft.Json.JsonConvert.SerializeObject(sendObj);
        //}
        public static string ConvertToJson(AGVInformation[] v)
        {
            SendData[] sendObj = new SendData[v.Count()];
            for (int i = 0; i < v.Count(); i++)
            {
                SendData obj = GetSendObj(v[i]);
                sendObj[i] = obj;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(sendObj);
        }

        public static SendData GetSendObj(AGVInformation v)
        {
            SendData obj = new SendData();
            obj.Num = v.Number;
            obj.BeginX = v.BeginX;
            obj.BeginY = v.BeginY;
            obj.EndX = v.EndX;
            obj.EndY = v.EndY;
            obj.DestX = v.DestX;
            obj.DestY = v.DestY;
            obj.StartLoc = v.StartLoc;
            obj.EndLoc = v.EndLoc;
            obj.State = v.State;
            obj.Arrive = false;
            return obj;
        }
        /// <summary>
        /// T代表要返回的数据类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToData<T>(string json)
        {
            List<T> listData = new List<T>();
            int start = json.IndexOf("{", 0);
            int end = json.IndexOf("}", 0);
            while (start >= 0 && end > 0)
            {
                string str = json.Substring(start, end - start + 1);
                System.Console.WriteLine(str);
                T dataObj = (T)JsonConvert.DeserializeObject(str, typeof(T));
                listData.Add(dataObj);
                start = json.IndexOf("{", end);
                end = json.IndexOf("}", end + 1);
            }
            return listData;
        }
    }
}

using AGV_V1._0;
using Const;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASK.AGV;
using TASK.XUMAP;
using TASK.MSG;

namespace AGV_TASK.MSG
{
     class ProcessRecDate
    {
        public const int SUCCESS = 6;
        public const int FAIRED = -1;

         private static ProcessRecDate instance;
         public static ProcessRecDate Instance
         {
             get
             {
                 if (null == instance)
                 {
                     instance = new ProcessRecDate();
                 }
                 return instance;
             }
         }
         public void Process(object state)
         {
             
         }
      
    }
}

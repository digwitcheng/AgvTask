using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TASK.XUMAP
{
  public   class MAP
    {
        
        public int x;
        public int y;
        public bool occupy;//按照1 2 3 4~的顺序将occupy置为true，直到最后一个入口；再将所有的occupy清为false
        public string style;

        public int agvNumOfQueuing; //小车进入的顺序

        public MAP()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TASK.MAP
{
  public   class MAP
    {
        
        public int x;
        public int y;
        public bool occupy; //occupy 是否已经有小车占用 ,false:未占用;true:已占用
        public string style;

        public int agventer;

        public MAP()
        {
        }
    }
}

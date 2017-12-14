//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace AGV_TASK
//{
//    class TestClass
//    {
//        //声明一个delegate（委托）类型：testDelegate，该类型可以搭载返回值为空，参数只有一个(long型)的方法。  
//        public delegate void testDelegate(long i);

//        //声明一个testDelegate类型的对象。该对象代表了返回值为空，参数只有一个(long型)的方法。它可以搭载N个方法。  
//        public testDelegate mainThread;

//        /// <summary>  
//        /// 测试方法  
//        /// </summary>  
//        public void testFunction()
//        {
//            long i = 0;
//            while (true)
//            {
//                i++;
//                mainThread(i); //调用委托对象  
//                Thread.Sleep(1000);  //线程等待1000毫秒  
//            }
//        } 
//    }
//}

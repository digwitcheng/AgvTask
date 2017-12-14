using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageLib.Threads;

namespace ImageLib.Threads
{
    public class ServiceManager
    {
        private ServiceExecutor serviceExecutor = new ServiceExecutor();
        
        #region 单例构造 
        private static ServiceManager instance;

        public static ServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceManager();
                }
                return instance;
            }
        }

     private ServiceManager()
        {
        }
        #endregion

        #region 状态函数
        public bool IsTaskRunning(String taskName)
        {
            bool ret = false;
            IServiceTask task = ServiceManager.Instance.CurrentExecutor.GetTaskByName(taskName);
            if (task != null && task.IsRunning)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
            return ret;
        }
        #endregion

        #region 属性
        public ServiceExecutor CurrentExecutor
        {
            get { return this.serviceExecutor; }
        }

        #endregion
    }
}

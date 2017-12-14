using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using TASK.MSG;

namespace ImageLib.Threads
{
    public abstract class AbstractContinuedTask : IServiceTask
    {
        protected volatile bool isRunning = false;
        protected volatile bool isCanceled = false;
        protected String className = "AbstractServiceTask";
        private const int TASK_SLEEP_TIME_SPAN = 5;
        public event EventHandler<ServiceTaskEndEventArgs> OnServiceTaskEndHandler;

        public int StartTask()
        {
            int ret = -1;
            try
            {
                if(PreparedTask() != 0 )
                {
                    return 9;
                }
                this.isRunning = true;
                ret = 0;
            }
            catch (System.Exception ex)
            {
                //Logger.LogError(className, ":" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "启动任务时捕获未知异常" + ex.Message);
                this.isRunning = false;
                this.isCanceled = true;
                ret = 9;
            }

            return ret;
        }
        public void Run()
        {
            Stopwatch watch = new Stopwatch();

            while (this.isRunning)
            {
                try
                {
                    watch.Start();
                    ExecuteTask();
                    watch.Stop();

                    TaskExecutedWatch(watch);
                }
                catch (Exception ex)
                {
                    //Logger.LogError(className, ":" + ex.Message);
                    QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "执行任务捕获未知异常" + ex.Message);
                    CancelTask();
                }
                Thread.Sleep(TASK_SLEEP_TIME_SPAN);
            }
            //Logger.LogInfo(className, "执行任务成功完成【" + this.GetTaskName() +"】");
        }

        public int CancelTask()
        {
            int ret = -1;

            try
            {
                CleanTask();

                if (this.OnServiceTaskEndHandler != null)
                {
                    EventHandlerTrigger.TriggerEvent<ServiceTaskEndEventArgs>(this.OnServiceTaskEndHandler, this,
                        new ServiceTaskEndEventArgs(this));
                }

                ret = 0;
            }
            catch (System.Exception ex)
            {
               // Logger.LogError(className, ":" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "结束任务捕获未知异常" + ex.Message);
                ret = 9;
            }
            finally
            {
                this.isRunning = false;
                this.isCanceled = true;
            }
            return ret;
        }

        #region 属性
        public bool IsRunning
        {
            get { return this.isRunning; }
        }

        public bool IsCanceled
        {
            get { return this.isCanceled; }
        }

        #endregion

        #region 抽象方法

        public abstract int PreparedTask();
        public abstract void ExecuteTask();
        public abstract int CleanTask();
        public abstract void TaskExecutedWatch(Stopwatch watch);
        public abstract String GetTaskName();
        public abstract void SetTaskName(string taskName);

        #endregion

    }
}

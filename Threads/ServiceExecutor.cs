using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using TASK.MSG;

/************************************************************************/
/*                                                                      */
/************************************************************************/
namespace ImageLib.Threads
{
    public class ServiceExecutor : IServiceExecutor
    {
        private Dictionary<String, IServiceTask> taskDict = new Dictionary<String, IServiceTask>();
        private int currentTaskCount = 0;
        private const int allowedTaskNumber = 30;
        private Object addTaskLock = new Object();

        public enum CurrentExecutorState
        {
            ADD_TASK_SUCCESS = 0,
            TASK_IS_ON_RUNNING = 1,
            HAS_SAME_NAME_TASK_IN_QUEUE = 2,
            ADD_TASK_IS_NULL = 3,
            TASK_START_FAILED = 4,
            TASK_NAME_IS_EMPTY = 5,
            REACH_MAX_TASK_LIMIT = 9,
        };

        public IServiceTask GetTaskByName(String taskName)
        {
            IServiceTask retTask = null;

            if (taskName == null || taskName.Equals(""))
            {
                return retTask;
            }

            if (this.taskDict.ContainsKey(taskName))
            {
                retTask = this.taskDict[taskName];
            }

            return retTask;
        }

        public int Execute(IServiceTask task)
        {
            int ret = -1;

            if(task == null)
            {
                return (int)CurrentExecutorState.ADD_TASK_IS_NULL;
            }

            lock(this.addTaskLock)
            {
                currentTaskCount = taskDict.Count;
                if(currentTaskCount == allowedTaskNumber)
                {
                    return (int)CurrentExecutorState.REACH_MAX_TASK_LIMIT;
                }

                if(task.IsRunning)
                {
                    return (int)CurrentExecutorState.TASK_IS_ON_RUNNING;
                }

                if (task.GetTaskName() == null || task.GetTaskName().Equals(""))
                {
                    return (int)CurrentExecutorState.TASK_NAME_IS_EMPTY;
                }

                if(this.taskDict.ContainsKey(task.GetTaskName()))
                {
                    //Logger.LogInfo("ServiceExecutor.Execute", "工作任务【" + task.GetTaskName() + "】已经添加，不能重复添加相同任务");
                    return (int)CurrentExecutorState.HAS_SAME_NAME_TASK_IN_QUEUE;
                }

                this.taskDict.Add(task.GetTaskName(), task);
                task.OnServiceTaskEndHandler += this.OnServiceTaskEndHandler;

                if (task.StartTask() != 0)
                {
                    this.taskDict.Remove(task.GetTaskName());
                    task.OnServiceTaskEndHandler -= this.OnServiceTaskEndHandler;
                }

                ThreadPool.QueueUserWorkItem(new WaitCallback(RunTask), task);
                ret = (int)CurrentExecutorState.ADD_TASK_SUCCESS;

                //Logger.LogInfo("ServiceExecutor.Execute", "工作任务添加成功【" + task.GetTaskName() + "】");
                
            }
            
            return ret;
        }

        private void RunTask(Object _task)
        {
            IServiceTask task = (IServiceTask)_task;
            task.Run();
            //Logger.LogInfo("ServiceExecutor.RunTask", "任务【" + task.GetTaskName() + "】所在工作线程退出");
        }

        private void OnServiceTaskEndHandler(Object sender, ServiceTaskEndEventArgs args)
        {
            
            try
            {
                if (args == null || args.Task == null)
                {
                    return;
                }

                IServiceTask task = args.Task;
                
                if(this.taskDict.ContainsKey(task.GetTaskName()))
                {
                    this.taskDict.Remove(task.GetTaskName());
                }
                task.OnServiceTaskEndHandler -= this.OnServiceTaskEndHandler;
                //Logger.LogInfo("ServiceExecutor.OnServiceTaskEndHandler", "工作任务清理成功 : " + task.GetTaskName());

            }
            catch (System.Exception ex)
            {
                //Logger.LogError(methodStr, "在Service Executor的事件函数中捕获未知异常" + ex.Message);
                QueueInstance.Instance.AddMyLogList(System.DateTime.Now.ToString() + "创建Socket对象时捕获异常" + ex.Message);
            }
        }

        public void PrintAllTaskName()
        {
            int count = 0;
            foreach (IServiceTask task in this.taskDict.Values)
            {
                count++;
                string taskName = task.GetTaskName();
                //Logger.LogInfo("PrintAllTaskName","序号:【" + count + "】【" + taskName + "】");
            }
            //Logger.LogInfo(null, "【---------------------------------------------------------------------------------】");
        }
    }
}

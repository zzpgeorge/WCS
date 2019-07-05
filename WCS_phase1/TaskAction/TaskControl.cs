using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Models;
using System.Data;
using WCS_phase1.Functions;

namespace WCS_phase1.TaskAction
{
    class TaskControl
    {
        MySQL mySQL = new MySQL();
        SimpleTools tools = new SimpleTools();

        /// <summary>
        /// 执行WCS入库清单
        /// </summary>
        public void Run_In()
        {
            try
            {
                // 获取可执行的入库清单
                DataTable dtwcs = mySQL.SelectAll("SELECT * FROM wcs_command_master WHERE WCS_TYPE = '1' AND STEP = '2' ORDER BY CREATION_TIME");
                if (tools.IsNoData(dtwcs))
                {
                    return;
                }

                List<WCSCommand> wcsList = dtwcs.ToDataList<WCSCommand>();
                // 遍历执行入库任务
                foreach (WCSCommand wcs in wcsList)
                {
                    PerformingTask_in(wcs.TASK_UID_1);
                    PerformingTask_in(wcs.TASK_UID_2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行入库任务
        /// </summary>
        /// <param name="task_uid"></param>
        public void PerformingTask_in(String task_uid)
        {
            if (String.IsNullOrEmpty(task_uid))
            {
                return;
            }
            try
            {
                //获取入库任务
                String sql = String.Format(@"select * from wcs_task_info where SITE = 'N' and TASK_UID = '{0}'",task_uid);
                DataTable dtTask = mySQL.SelectAll(sql);
                if (tools.IsNoData(dtTask))
                {
                    return;
                }
                TaskInfo user = dtTask.ToDataEntity<TaskInfo>();
                //按需分配各设备指令
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

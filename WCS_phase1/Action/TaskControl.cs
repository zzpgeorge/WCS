using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Models;
using System.Data;
using WCS_phase1.Functions;
using System.Configuration;

namespace WCS_phase1.Action
{
    class TaskControl
    {
        MySQL mySQL = new MySQL();
        SimpleTools tools = new SimpleTools();
        TaskTools task = new TaskTools();

        #region 初步入库任务
        /// <summary>
        /// 执行WCS入库清单（初步执行）
        /// </summary>
        public void Run_In_Initial()
        {
            try
            {
                // 获取可执行的入库清单
                DataTable dtcommand = mySQL.SelectAll("select * from wcs_command_v where TASK_TYPE = '1' and STEP = '2' order by CREATION_TIME");
                if (tools.IsNoData(dtcommand))
                {
                    return;
                }

                List<WCS_COMMAND_V> comList = dtcommand.ToDataList<WCS_COMMAND_V>();
                // 遍历执行入库任务
                foreach (WCS_COMMAND_V com in comList)
                {
                    Task_In_Initial(com);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行入库任务（初步执行）
        /// </summary>
        /// <param name="command"></param>
        public void Task_In_Initial(WCS_COMMAND_V command)
        {
            try
            {
                // 摆渡车到固定辊台对接点
                String ARFloc = task.GetARFLoc(command.FRT);    //获取对应摆渡车位置
                task.CreateTask(command.WCS_NO, ConstantValues.摆渡车定位, ARFloc);  //生成摆渡车任务

                // 运输车到摆渡车对接点
                task.CreateTask(command.WCS_NO, ConstantValues.运输车复位1, ConfigurationManager.AppSettings["StandbyP1"]);  //生成运输车任务

                // 行车到运输车对接取货点
                String ABCloc = task.GetABCTrackLoc(command.LOC_1);     //获取对应行车位置
                task.CreateTask(command.WCS_NO, ConstantValues.行车定位, ABCloc);     //生成行车任务

                //更新WCS COMMAND状态——执行中
                task.UpdateStep(command.WCS_NO, "3");
            }
            catch (Exception ex)
            {
                //发生异常初始化资讯
                task.UpdateStep(command.WCS_NO, "2");
                task.DeleteTask(command.WCS_NO, "");
                throw ex;
            }
        }
        #endregion

        #region 可持续入库任务

        #endregion

        #region 读写设备
        /// <summary>
        /// 分配设备任务&下发设备指令
        /// </summary>
        public void RWDevice()
        {
            try
            {
                // 获取待分配设备任务
                DataTable dtitem = mySQL.SelectAll("select * from wcs_task_item where STATUS = 'N' and DEVICE is null order by CREATION_TIME");
                if (tools.IsNoData(dtitem))
                {
                    return;
                }
                List<WCS_TASK_ITEM> itemList = dtitem.ToDataList<WCS_TASK_ITEM>();
                // 遍历分配设备
                foreach (WCS_TASK_ITEM item in itemList)
                {
                    ReadDevice(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 分配设备
        /// </summary>
        /// <param name="item"></param>
        public void ReadDevice(WCS_TASK_ITEM item)
        {
            try
            {
                switch (item.ITEM_ID.Substring(0, 2))
                {
                    case "01":  //摆渡车
                        //获取所有摆渡车状态
                        //计算位置允许范围
                        //判断是否需要移动位置
                        //确认设备
                        //更新状态
                        //下发指令
                        break;
                    case "02":  //运输车
                        break;
                    case "03":  //行车
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 设备对接

        #endregion

        #region 接轨出库任务

        #endregion

        #region 可持续出库任务

        #endregion

    }
}

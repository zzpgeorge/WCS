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
                task.CreateItem(command.WCS_NO, ItemId.摆渡车定位固定辊台, ARFloc);  //生成摆渡车任务

                // 运输车到摆渡车对接点
                task.CreateItem(command.WCS_NO, ItemId.运输车复位1, ConfigurationManager.AppSettings["StandbyP1"]);  //生成运输车任务

                // 行车到运输车对接取货点
                String ABCloc = task.GetABCTrackLoc(command.LOC_1);     //获取对应行车位置
                task.CreateItem(command.WCS_NO, ItemId.行车轨道定位, ABCloc);     //生成行车任务

                //更新WCS COMMAND状态——执行中
                task.UpdateCommand(command.WCS_NO, CommandStep.执行中);
                //更新WCS TASK状态——任务中
                task.UpdateTaskByWCSNo(command.WCS_NO, TaskSite.任务中);
            }
            catch (Exception ex)
            {
                //初始化
                task.UpdateCommand(command.WCS_NO, CommandStep.请求执行);
                task.UpdateTaskByWCSNo(command.WCS_NO, TaskSite.未执行);
                task.DeleteItem(command.WCS_NO, "");
                throw ex;
            }
        }
        #endregion

        #region 可持续入库任务

        #endregion



        #region 对接设备
        /// <summary>
        /// 读写设备
        /// </summary>
        public void RWDevice()
        {
            try
            {
                // 获取待分配设备任务
                DataTable dtitem = mySQL.SelectAll("select * from WCS_TASK_ITEM where STATUS = 'N' and DEVICE is null order by CREATION_TIME");
                if (tools.IsNoData(dtitem))
                {
                    return;
                }
                List<WCS_TASK_ITEM> itemList = dtitem.ToDataList<WCS_TASK_ITEM>();
                // 遍历分配设备&下发指令
                foreach (WCS_TASK_ITEM item in itemList)
                {
                    ReadDevice(item);
                    WriteDeviceMove(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 分配任务设备
        /// </summary>
        /// <param name="item"></param>
        public void ReadDevice(WCS_TASK_ITEM item)
        {
            try
            {
                switch (item.ITEM_ID.Substring(0, 2))
                {
                    case "01":
                        #region 摆渡车
                        // =>根据任务讯息获取位置允许范围可用设备

                        // =>确认设备

                        //更新状态
                        task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.请求执行);
                        #endregion
                        break;
                    case "02":
                        #region 运输车
                        // =>根据任务讯息获取位置允许范围可用设备

                        // =>确认设备

                        //更新状态
                        task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.请求执行);
                        #endregion
                        break;
                    case "03":
                        #region 行车
                        // =>根据任务讯息获取位置允许范围可用设备

                        // =>确认设备

                        //更新状态
                        task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.请求执行);
                        #endregion
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //初始化
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.不可执行);
                throw ex;
            }
        }

        /// <summary>
        /// 下发设备定位指令
        /// </summary>
        /// <param name="item"></param>
        public void WriteDeviceMove(WCS_TASK_ITEM item)
        {
            try
            {
                //状态不为[请求执行]的任务不能继续作业
                if (item.STATUS != ItemStatus.请求执行)
                {
                    return;
                }
                //对接任务不作业
                if (item.ITEM_ID.Substring(0, 2) == "11")
                {
                    return;
                }

                // =>组合资讯，下发指令

                //更新状态
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.任务中);
            }
            catch (Exception ex)
            {
                //初始化
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.请求执行);
                throw ex;
            }
        }

        /// <summary>
        /// 设备到位进行对接作业 [对接任务排程触发]
        /// </summary>
        public void LinkDevice()
        {
            try
            {
                #region 固定辊台 <==> 摆渡车
                // 获取已完成对接阶段的摆渡车任务
                List<WCS_TASK_ITEM> itemList_ARF = task.GetItemList_R(ItemId.摆渡车定位固定辊台);
                // 遍历生成滚棒任务
                foreach (WCS_TASK_ITEM item_ARF in itemList_ARF)
                {
                    CreateTask_ARF_FRT(item_ARF);
                }
                #endregion

                #region 摆渡车 <==> 运输车
                // 获取已完成对接阶段的摆渡车任务
                List<WCS_TASK_ITEM> itemList_A = task.GetItemList_R(ItemId.摆渡车定位运输车对接);
                // 遍历生成滚棒任务
                foreach (WCS_TASK_ITEM item_A in itemList_A)
                {
                    CreateTask_ARF_RGV(item_A);
                }
                #endregion

                #region 运输车 <==> 运输车
                // 获取已完成对接阶段的运输车任务
                List<WCS_TASK_ITEM> itemList_R = task.GetItemList_R(ItemId.运输车对接定位);
                // 遍历生成滚棒任务
                foreach (WCS_TASK_ITEM item_R in itemList_R)
                {
                    CreateTask_RGV_RGV(item_R);
                }
                #endregion

                #region 运输车 <==> 行车
                // 获取已完成对接阶段的运输车任务
                List<WCS_TASK_ITEM> itemList_ABC = task.GetItemList_R(ItemId.行车轨道定位);
                // 遍历生成夹具取放任务
                foreach (WCS_TASK_ITEM item_ABC in itemList_ABC)
                {
                    CreateTask_RGV_ABC(item_ABC);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建摆渡车&固定辊台对接出入库 ITEM 任务
        /// </summary>
        /// <param name="item"></param>
        public void CreateTask_ARF_FRT(WCS_TASK_ITEM item)
        {
            try
            {
                // 判断是出入库类型
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   // 入库  固定辊台 (货物)==> 摆渡车
                        // 先动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车入库, item.DEVICE, "", "", ItemStatus.请求执行);
                        // 后动固定辊台滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.固定辊台入库, task.GetFRTDevice(item.LOC_TO), "", "", ItemStatus.请求执行);
                        break;
                    case "O":   // 出库  摆渡车 (货物)==> 固定辊台
                        // 先动固定辊台滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.固定辊台出库, task.GetFRTDevice(item.LOC_TO), "", "", ItemStatus.请求执行);
                        // 后动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车出库, item.DEVICE, "", "", ItemStatus.请求执行);
                        break;
                    default:
                        break;
                }
                //摆渡车初始任务更新状态——完成
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.完成任务);
            }
            catch (Exception ex)
            {
                //恢复
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接);
                task.DeleteItem(item.WCS_NO, ItemId.固定辊台入库);
                task.DeleteItem(item.WCS_NO, ItemId.固定辊台出库);
                task.DeleteItem(item.WCS_NO, ItemId.摆渡车入库);
                task.DeleteItem(item.WCS_NO, ItemId.摆渡车出库);
                throw ex;
            }
        }

        /// <summary>
        /// 创建摆渡车&运输车对接出入库 ITEM 任务
        /// </summary>
        /// <param name="item"></param>
        public void CreateTask_ARF_RGV(WCS_TASK_ITEM item)
        {
            String wcsno_R = "";
            String itemid_R = "";
            String device_R = "";
            try
            {
                // 查看运输车是否到位
                String sql_R = String.Format(@"select WCS_NO, ITEM_ID, DEVICE from WCS_TASK_ITEM where STATUS = 'R' and WCS_NO = '{0}' and ITEM_ID = '{1}'", item.WCS_NO, ItemId.运输车复位1);
                DataTable dtitem_R = mySQL.SelectAll(sql_R);
                if (tools.IsNoData(dtitem_R))
                {
                    return;
                }
                wcsno_R = dtitem_R.Rows[0]["WCS_NO"].ToString();
                itemid_R = dtitem_R.Rows[0]["ITEM_ID"].ToString();
                device_R = dtitem_R.Rows[0]["DEVICE"].ToString();
                // 判断是出入库类型
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   // 入库  摆渡车 (货物)==> 运输车
                        // 先动运输车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车入库, device_R, "", "", ItemStatus.请求执行);
                        // 后动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车入库, item.DEVICE, "", "", ItemStatus.请求执行);
                        break;
                    case "O":   // 出库  运输车 (货物)==> 摆渡车
                        // 先动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车出库, item.DEVICE, "", "", ItemStatus.请求执行);
                        // 后动运输车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, device_R, "", "", ItemStatus.请求执行);
                        break;
                    default:
                        break;
                }
                //摆渡车&运输车初始任务更新状态——完成
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.完成任务);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.完成任务);
            }
            catch (Exception ex)
            {
                //恢复
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接);
                task.DeleteItem(item.WCS_NO, ItemId.运输车入库);
                task.DeleteItem(item.WCS_NO, ItemId.摆渡车入库);
                task.DeleteItem(item.WCS_NO, ItemId.摆渡车出库);
                task.DeleteItem(item.WCS_NO, ItemId.运输车出库);
                throw ex;
            }
        }

        /// <summary>
        /// 创建运输车&运输车对接出入库 ITEM 任务
        /// </summary>
        /// <param name="item"></param>
        public void CreateTask_RGV_RGV(WCS_TASK_ITEM item)
        {
            String wcsno_R = "";
            String itemid_R = "";
            String device_R = "";
            try
            {
                // 查看运输车是否在运输车对接待命点
                String sql_R = String.Format(@"select WCS_NO, ITEM_ID, DEVICE from WCS_TASK_ITEM where STATUS = 'R' and WCS_NO = '{0}' and ITEM_ID = '{1}'", item.WCS_NO, ItemId.运输车复位2);
                DataTable dtitem_R = mySQL.SelectAll(sql_R);
                if (tools.IsNoData(dtitem_R))
                {
                    return;
                }
                wcsno_R = dtitem_R.Rows[0]["WCS_NO"].ToString();
                itemid_R = dtitem_R.Rows[0]["ITEM_ID"].ToString();
                device_R = dtitem_R.Rows[0]["DEVICE"].ToString();
                // 判断是出入库类型
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   // 入库  运输车[外] (货物)==> 运输车[内]
                        // 先动运输车滚棒[内]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车入库, device_R, "", "", ItemStatus.请求执行);
                        // 后动运输车滚棒[外]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车入库, item.DEVICE, "", "", ItemStatus.请求执行);
                        break;
                    case "O":   // 出库  运输车[内] (货物)==> 运输车[外]
                        // 先动运输车滚棒[外]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, item.DEVICE, "", "", ItemStatus.请求执行);
                        // 后动运输车滚棒[内]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, device_R, "", "", ItemStatus.请求执行);
                        break;
                    default:
                        break;
                }
                //内外运输车初始任务更新状态——完成
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.完成任务);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.完成任务);
            }
            catch (Exception ex)
            {
                //恢复
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接);
                task.DeleteItem(item.WCS_NO, ItemId.运输车入库);
                task.DeleteItem(item.WCS_NO, ItemId.运输车出库);
                throw ex;
            }
        }

        /// <summary>
        /// 创建运输车&行车对接出入库 ITEM 任务
        /// </summary>
        /// <param name="item"></param>
        public void CreateTask_RGV_ABC(WCS_TASK_ITEM item)
        {
            String wcsno_R = "";
            String itemid_R = "";
            String device_R = "";
            try
            {
                // 查看运输车是否在运输车对接待命点
                String sql_R = String.Format(@"select WCS_NO, ITEM_ID, DEVICE from WCS_TASK_ITEM where STATUS = 'R' and WCS_NO = '{0}' and ITEM_ID = '{1}'", item.WCS_NO, ItemId.运输车定位);
                DataTable dtitem_R = mySQL.SelectAll(sql_R);
                if (tools.IsNoData(dtitem_R))
                {
                    return;
                }
                wcsno_R = dtitem_R.Rows[0]["WCS_NO"].ToString();
                itemid_R = dtitem_R.Rows[0]["ITEM_ID"].ToString();
                device_R = dtitem_R.Rows[0]["DEVICE"].ToString();
                // 判断是出入库类型
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   // 入库  运输车 (货物)==> 行车 
                        // 行车取货
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车取货, item.DEVICE, "", item.LOC_TO, ItemStatus.请求执行);
                        break;
                    case "O":   // 出库  行车 (货物)==> 运输车 
                        // 行车放货
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车放货, item.DEVICE, "", item.LOC_TO, ItemStatus.请求执行);
                        break;
                    default:
                        break;
                }
                //行车&运输车初始任务更新状态——完成
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.完成任务);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.完成任务);
            }
            catch (Exception ex)
            {
                //恢复
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接);
                task.DeleteItem(item.WCS_NO, ItemId.行车取货);
                task.DeleteItem(item.WCS_NO, ItemId.行车放货);
                throw ex;
            }
        }
        #endregion



        #region 接轨出库任务

        #endregion

        #region 可持续出库任务

        #endregion

    }
}

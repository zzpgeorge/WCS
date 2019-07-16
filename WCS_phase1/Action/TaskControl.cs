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
        public void Run_InInitial()
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
                    Task_InInitial(com);
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
        public void Task_InInitial(WCS_COMMAND_V command)
        {
            try
            {
                // 选择目的货位最靠外的货物优先处理
                String loc1 = task.GetRGVLoc(2, command.LOC_1); //运输车辊台②任务1
                String loc2 = task.GetRGVLoc(1, command.LOC_2); //运输车辊台①任务2
                String loc = task.GetLocByRgvToLoc(loc1, loc2); //处理货位
                if (loc == "NG")
                {
                    //不能没有货物目的位置
                    return;
                }

                // 摆渡车到固定辊台对接点
                String ARFloc = task.GetARFLoc(command.FRT);    //获取对应摆渡车位置
                task.CreateItem(command.WCS_NO, ItemId.摆渡车定位固定辊台, ARFloc);  //生成摆渡车任务

                // 运输车到摆渡车对接点
                task.CreateItem(command.WCS_NO, ItemId.运输车复位1, ConfigurationManager.AppSettings["StandbyP1"]);  //生成运输车任务

                // 行车到运输车对接取货点
                String ABCloc = task.GetABCTrackLoc(loc);     //获取对应行车位置
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

        #region 可持续任务
        /// <summary>
        /// 执行判断是否需要后续作业
        /// </summary>
        public void Run_TaskContinued()
        {
            try
            {
                // 以wcs_no为单位提取最后一笔任务
                DataTable dtlast = mySQL.SelectAll(@"select * from WCS_TASK_ITEM where (WCS_NO,CREATION_TIME) in 
                                                    (select WCS_NO, MAX(CREATION_TIME) from WCS_TASK_ITEM group by WCS_NO) order by CREATION_TIME");
                if (tools.IsNoData(dtlast))
                {
                    return;
                }
                List<WCS_TASK_ITEM> lastList = dtlast.ToDataList<WCS_TASK_ITEM>();
                // 遍历后续判断作业
                foreach (WCS_TASK_ITEM last in lastList)
                {
                    Task_Continued(last);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 后续任务作业
        /// </summary>
        /// <param name="item"></param>
        public void Task_Continued(WCS_TASK_ITEM item)
        {
            try
            {
                // 任务目的比对检测
                CheckTask(item.WCS_NO, item.LOC_TO);

                // 清单是[结束]状态不作业
                if (task.GetCommandStep(item.WCS_NO) == CommandStep.结束)
                {
                    return;
                }

                // Item非[完成]状态不作业
                if (item.STATUS != ItemStatus.完成任务)
                {
                    return;
                }

                // 依出入库类型处理
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   //入库
                        ProcessInTask(item);
                        break;
                    case "O":   //出库
                        ProcessOutTask(item);
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

        /// <summary>
        /// 检查比对任务目的
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wcs_no"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public void CheckTask(String wcs_no, String loc)
        {
            try
            {
                // 获取对应清单
                String sql = String.Format(@"select TASK_TYPE,FRT,TASK_UID_1,LOC_1,TASK_UID_2,LOC_2 from wcs_command_v where STEP <>'4' and WCS_NO = '{0}'", wcs_no);
                DataTable dt = mySQL.SelectAll(sql);
                if (tools.IsNoData(dt))
                {
                    return;
                }
                String tasktype = dt.Rows[0]["TASK_TYPE"].ToString();
                String frt = dt.Rows[0]["FRT"].ToString();
                String taskid1 = dt.Rows[0]["TASK_UID_1"].ToString();
                String loc1 = dt.Rows[0]["LOC_1"].ToString();
                String taskid2 = dt.Rows[0]["TASK_UID_2"].ToString();
                String loc2 = dt.Rows[0]["LOC_2"].ToString();
                // 判断目的位置是否一致
                // 更新Task状态(Command于数据库TRIGGER 'update_command_T' 触发更新)
                switch (tasktype)
                {
                    case TaskType.入库:
                        if (loc == task.GetLocForIn(loc1))
                        {
                            task.UpdateTask(taskid1, TaskSite.完成);
                        }
                        if (loc == task.GetLocForIn(loc2))
                        {
                            task.UpdateTask(taskid2, TaskSite.完成);
                        }
                        break;
                    case TaskType.出库:
                        if (loc == frt)
                        {
                            task.UpdateTask(taskid1, TaskSite.完成);
                            task.UpdateTask(taskid2, TaskSite.完成);
                        }
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

        /// <summary>
        /// 入库任务处理
        /// </summary>
        /// <param name="item"></param>
        public void ProcessInTask(WCS_TASK_ITEM item)
        {
            try
            {
                // 摆渡车于运输车对接点
                String AR = ConfigurationManager.AppSettings["StandbyAR"];
                // 运输车[内]待命复位点
                String R = ConfigurationManager.AppSettings["StandbyP2"];
                // 运输车于运输车对接点
                String RR = ConfigurationManager.AppSettings["StandbyRR"];

                // 获取对应清单
                String sql = String.Format(@"select TASK_UID_1,LOC_1,SITE_1, TASK_UID_2,LOC_2,SITE_2 from wcs_command_v where WCS_NO = '{0}'", item.WCS_NO);
                DataTable dt = mySQL.SelectAll(sql);
                if (tools.IsNoData(dt))
                {
                    return;
                }
                String taskid1 = task.GetRGVLoc(2, dt.Rows[0]["TASK_UID_1"].ToString()); //任务1
                String taskid2 = task.GetRGVLoc(2, dt.Rows[0]["TASK_UID_2"].ToString()); //任务2
                String site1 = task.GetRGVLoc(2, dt.Rows[0]["SITE_1"].ToString()); //状态1
                String site2 = task.GetRGVLoc(2, dt.Rows[0]["SITE_2"].ToString()); //状态2
                String loc1 = task.GetRGVLoc(2, dt.Rows[0]["LOC_1"].ToString()); //目标点1
                String loc2 = task.GetRGVLoc(2, dt.Rows[0]["LOC_2"].ToString()); //目标点2
                // 默认入库时 taskid1对接运输车设备辊台②、taskid2对接运输车设备辊台①
                String loc_1 = task.GetRGVLoc(2, loc1); //辊台②任务1
                String loc_2 = task.GetRGVLoc(1, loc2); //辊台①任务2
                String loc; //执行目标

                switch (item.ITEM_ID)   //根据最后的设备指令，可得货物已在流程中该设备对接的下一设备处
                {
                    case ItemId.固定辊台入库: //目的设备为对接的摆渡车，可直接加以分配
                        #region 将摆渡车移至运输车对接位置
                        // 可断定货物需移至运输车
                        // 生成摆渡车任务
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车定位运输车对接, item.LOC_TO, "", AR, ItemStatus.请求执行);
                        #endregion

                        break;
                    case ItemId.摆渡车入库:  //目的设备为对接的运输车，可直接加以分配
                        #region 将运输车移至行车对接位置 || 运输车间对接
                        // 根据货物目的地判断是否需要运输车对接运输车
                        loc = task.GetLocByRgvToLoc(loc_1, loc_2);
                        if (loc == "NG")
                        {
                            //不能没有货物目的位置
                            break;
                        }
                        // 判断是否需要对接到运输车[内]范围内作业
                        if (Convert.ToInt32(loc) >= Convert.ToInt32(R))  // 需对接运输车[内]
                        {
                            // 生成运输车[内]复位任务
                            task.CreateItem(item.WCS_NO, ItemId.运输车复位2, R);    // 待分配设备
                            // 生成运输车[外]对接位任务
                            task.CreateCustomItem(item.WCS_NO, ItemId.运输车对接定位, item.LOC_TO, "", RR, ItemStatus.请求执行);
                        }
                        else
                        {
                            // 生成运输车[外]定位任务
                            task.CreateCustomItem(item.WCS_NO, ItemId.运输车定位, item.LOC_TO, "", loc, ItemStatus.请求执行);
                        }
                        #endregion

                        break;
                    case ItemId.运输车入库:  //目的设备为对接的运输车，可直接加以分配
                        #region 将运输车移至行车对接位置
                        // 判断是否作业过运输车定位对接行车任务
                        String sqlrr = String.Format(@"select * from wcs_task_item where ITEM_ID = '033' and STATUS not in ('E','X') and WCS_NO = '{0}'", item.WCS_NO);
                        DataTable dtrr = mySQL.SelectAll(sqlrr);
                        if (tools.IsNoData(dt))
                        {
                            loc = task.GetLocByRgvToLoc(loc_1, loc_2);
                            if (loc == "NG")
                            {
                                //不能没有货物目的位置
                                break;
                            }
                            // 生成运输车定位任务
                            task.CreateCustomItem(item.WCS_NO, ItemId.运输车定位, item.LOC_TO, "", loc, ItemStatus.请求执行);
                        }
                        #endregion

                        break;
                    case ItemId.行车取货:
                        #region 将运输车移至行车对接位置 && 行车定位
                        // 默认入库时 taskid1对接运输车设备辊台②、taskid2对接运输车设备辊台①
                        // 获取当前运输车加以分配
                        String rgv = task.GetItemDeviceLast(item.WCS_NO, ItemId.运输车定位);
                        // =>获取当前运输车资讯
                        // =>获取有货&无货辊台各对应的WMS任务目标
                        String loc_Y = "";  //有货辊台对应目标点
                        String loc_N = "";  //无货辊台对应目标点
                        // 根据当前运输车坐标及任务目标，生成对应运输车定位/对接运输车任务

                        // 生成行车库存定位任务
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车库存定位, item.DEVICE, "", task.GetABCStockLoc(loc_N), ItemStatus.请求执行);
                        #endregion

                        break;
                    case ItemId.行车放货:
                        #region 行车定位
                        // 未完成的任务目标点
                        loc = site1 == "Y"?loc2:loc1;
                        // 行车到运输车对接取货点
                        String ABCloc = task.GetABCTrackLoc(loc); //获取对应行车位置
                        // 生成行车轨道定位任务
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车轨道定位, item.DEVICE, "", ABCloc, ItemStatus.请求执行);
                        #endregion

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

        /// <summary>
        /// 出库任务处理
        /// </summary>
        /// <param name="item"></param>
        public void ProcessOutTask(WCS_TASK_ITEM item)
        {
            try
            {
                // 摆渡车于运输车对接点
                String AR = ConfigurationManager.AppSettings["StandbyAR"];
                // 运输车[内]待命复位点
                String R = ConfigurationManager.AppSettings["StandbyP2"];
                // 运输车于运输车对接点
                String RR = ConfigurationManager.AppSettings["StandbyRR"];

                // 获取对应清单
                String sql = String.Format(@"select TASK_UID_1,LOC_1,SITE_1, TASK_UID_2,LOC_2,SITE_2 from wcs_command_v where WCS_NO = '{0}'", item.WCS_NO);
                DataTable dt = mySQL.SelectAll(sql);
                if (tools.IsNoData(dt))
                {
                    return;
                }
                String taskid1 = task.GetRGVLoc(2, dt.Rows[0]["TASK_UID_1"].ToString()); //任务1
                String taskid2 = task.GetRGVLoc(2, dt.Rows[0]["TASK_UID_2"].ToString()); //任务2
                String site1 = task.GetRGVLoc(2, dt.Rows[0]["SITE_1"].ToString()); //状态1
                String site2 = task.GetRGVLoc(2, dt.Rows[0]["SITE_2"].ToString()); //状态2
                String loc1 = task.GetRGVLoc(2, dt.Rows[0]["LOC_1"].ToString()); //目标点1
                String loc2 = task.GetRGVLoc(2, dt.Rows[0]["LOC_2"].ToString()); //目标点2
                // 默认入库时 taskid1对接运输车设备辊台②、taskid2对接运输车设备辊台①
                String loc_1 = task.GetRGVLoc(2, loc1); //辊台②任务1
                String loc_2 = task.GetRGVLoc(1, loc2); //辊台①任务2
                String loc; //执行目标

                switch (item.ITEM_ID)   //根据最后的设备指令，可得货物已在流程中该设备对接的下一设备处
                {
                    case ItemId.行车取货:
                        #region 行车轨道定位与运输车对接点
                        #endregion

                        break;
                    case ItemId.行车放货:
                        #region 将运输车移至行车对接位置 / 将运输车复位待命点 && 行车库存定位
                        #endregion

                        break;
                    case ItemId.运输车出库:
                        #region 将摆渡车移至固定辊台对接点 / 将运输车间对接 / 将运输车移至摆渡车对接点
                        #endregion

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

        #region 接轨出库任务
        /// <summary>
        /// 执行入库完成后操作出库作业
        /// </summary>
        public void Run_OutFollow()
        {
            // 判断是否存在出库清单
            // 判断是否存在满足入库条件的任务
            // =>获取设备资讯
            // =>判断是否存在空闲的摆渡车&固定辊台
            // 查询运输车&行车最佳的出库清单

            // 生成行车库存定位任务
            // 生成运输车对接行车任务
            // 生成摆渡车对接运输车任务
        }
        #endregion


        #region 分配设备
        /// <summary>
        /// 执行分配设备至各个任务
        /// </summary>
        public void Run_ItemDevice()
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
        #endregion

        #region 下发指令(除滚棒任务)
        /// <summary>
        /// 执行设备通讯下发指令(除滚棒任务)
        /// </summary>
        public void Run_SendOrderNotRoller()
        {
            try
            {
                // 获取请求执行的任务
                DataTable dtitem = mySQL.SelectAll("select * from WCS_TASK_ITEM where STATUS = 'Q' and LEFT(ITEM_ID,2) <> '11' order by CREATION_TIME");
                if (tools.IsNoData(dtitem))
                {
                    return;
                }
                List<WCS_TASK_ITEM> itemList = dtitem.ToDataList<WCS_TASK_ITEM>();
                // 遍历下发指令
                foreach (WCS_TASK_ITEM item in itemList)
                {
                    WriteDeviceNotRoller(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 下发设备指令(除滚棒任务)
        /// </summary>
        /// <param name="item"></param>
        public void WriteDeviceNotRoller(WCS_TASK_ITEM item)
        {
            try
            {
                // =>运输车[外]对接目的位置需计算

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
        #endregion

        #region 设备对接
        /// <summary>
        /// 执行设备到位进行对接作业 [对接任务排程触发下发指令]
        /// </summary>
        public void Run_LinkDevice()
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

                #region 行车 <==> 库存货位
                // 获取已完成对接阶段的运输车任务
                List<WCS_TASK_ITEM> itemList_LOC = task.GetItemList_R(ItemId.行车库存定位);
                // 遍历生成夹具取放任务
                foreach (WCS_TASK_ITEM item_LOC in itemList_LOC)
                {
                    CreateTask_ABC(item_LOC);
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
                        task.CreateCustomItem(item.WCS_NO, ItemId.固定辊台入库, task.GetFRTDevice(item.LOC_TO), "", item.DEVICE, ItemStatus.请求执行); //入库目的为摆渡车
                        break;
                    case "O":   // 出库  摆渡车 (货物)==> 固定辊台
                        // 先动固定辊台滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.固定辊台出库, task.GetFRTDevice(item.LOC_TO), "", "", ItemStatus.请求执行);
                        // 后动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车出库, item.DEVICE, "", task.GetFRTDevice(item.LOC_TO), ItemStatus.请求执行); //出库目的为固定辊台
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
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接中);
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
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车入库, item.DEVICE, "", device_R, ItemStatus.请求执行); //入库目的为运输车
                        break;
                    case "O":   // 出库  运输车 (货物)==> 摆渡车
                        // 先动摆渡车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.摆渡车出库, item.DEVICE, "", "", ItemStatus.请求执行);
                        // 后动运输车滚棒
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, device_R, "", item.DEVICE, ItemStatus.请求执行); //出库目的为摆渡车
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
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接中);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接中);
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
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车入库, item.DEVICE, "", device_R, ItemStatus.请求执行); //入库目的为运输车[内]
                        break;
                    case "O":   // 出库  运输车[内] (货物)==> 运输车[外]
                        // 先动运输车滚棒[外]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, item.DEVICE, "", "", ItemStatus.请求执行);
                        // 后动运输车滚棒[内]
                        task.CreateCustomItem(item.WCS_NO, ItemId.运输车出库, device_R, "", item.DEVICE, ItemStatus.请求执行);    //出库目的为运输车[外]
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
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接中);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接中);
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
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接中);
                task.UpdateItem(wcsno_R, itemid_R, ItemColumnName.作业状态, ItemStatus.交接中);
                task.DeleteItem(item.WCS_NO, ItemId.行车取货);
                task.DeleteItem(item.WCS_NO, ItemId.行车放货);
                throw ex;
            }
        }

        /// <summary>
        /// 创建行车出入库取放货 ITEM 任务
        /// </summary>
        /// <param name="item"></param>
        public void CreateTask_ABC(WCS_TASK_ITEM item)
        {
            try
            {
                // 判断是出入库类型
                switch (item.WCS_NO.Substring(0, 1))
                {
                    case "I":   // 入库  行车 (货物)==> 货位 
                        // 行车放货
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车放货, item.DEVICE, "", item.LOC_TO, ItemStatus.请求执行);
                        break;
                    case "O":   // 出库  货位 (货物)==> 行车
                        // 行车取货
                        task.CreateCustomItem(item.WCS_NO, ItemId.行车取货, item.DEVICE, "", item.LOC_TO, ItemStatus.请求执行);
                        break;
                    default:
                        break;
                }
                //行车&运输车初始任务更新状态——完成
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.完成任务);
            }
            catch (Exception ex)
            {
                //恢复
                task.UpdateItem(item.WCS_NO, item.ITEM_ID, ItemColumnName.作业状态, ItemStatus.交接中);
                task.DeleteItem(item.WCS_NO, ItemId.行车取货);
                task.DeleteItem(item.WCS_NO, ItemId.行车放货);
                throw ex;
            }
        }
        #endregion

        #region 下发指令(对接任务)[排程触发]
        /// <summary>
        /// 执行设备通讯下发指令(仅滚棒任务)
        /// </summary>
        public void Run_SendOrderOnlyRoller()
        {
            try
            {
                // 以wcs_no为单位提取任务
                // 依时间先后顺序下发指令
                // 循环获取设备状态
                // 前任务完成再启动后任务
                // 更新任务状态
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}

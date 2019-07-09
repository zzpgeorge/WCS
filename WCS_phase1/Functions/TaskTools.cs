using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Models;
using System.Data;
using WCS_phase1.Functions;

namespace WCS_phase1.Functions
{
    public class TaskTools
    {
        MySQL mySQL = new MySQL();
        SimpleTools tools = new SimpleTools();

        /// <summary>
        /// 获取摆渡车于目的固定辊台对接点位
        /// </summary>
        /// <param name="frt"></param>
        /// <returns></returns>
        public String GetARFLoc(String frt)
        {
            try
            {
                String sql = String.Format(@"select distinct ARF_LOC from WCS_CONFIG_LOC where FRT_LOC = '{0}'", frt);
                DataTable dtloc = mySQL.SelectAll(sql);
                if (tools.IsNoData(dtloc))
                {
                    return "";
                }
                return dtloc.Rows[0]["ARF_LOC"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取行车目的轨道点位
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public String GetABCTrackLoc(String loc)
        {
            try
            {
                String sql = String.Format(@"select distinct ABC_LOC_TRACK from WCS_CONFIG_LOC where WMS_LOC = '{0}'", loc);
                DataTable dtloc = mySQL.SelectAll(sql);
                if (tools.IsNoData(dtloc))
                {
                    return "";
                }
                return dtloc.Rows[0]["ABC_LOC_TRACK"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取摆渡车对接的固定辊台的设备编号
        /// </summary>
        /// <param name="arf"></param>
        /// <returns></returns>
        public String GetFRTDevice(String arf)
        {
            try
            {
                String sql = String.Format(@"select distinct FRT_LOC from WCS_CONFIG_LOC where ARF_LOC = '{0}'", arf);
                DataTable dtloc = mySQL.SelectAll(sql);
                if (tools.IsNoData(dtloc))
                {
                    return "";
                }
                return dtloc.Rows[0]["FRT_LOC"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新 COMMAND 状态[1.生成单号，2.请求执行，3.执行中，4.结束]
        /// </summary>
        /// <param name="step"></param>
        /// <param name="wcs_no"></param>
        public void UpdateCommand(String wcs_no, String step)
        {
            try
            {
                String sql = String.Format(@"update WCS_COMMAND_MASTER set STEP = '{0}',UPDATE_TIME = NOW() where WCS_NO = '{1}'", step, wcs_no);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新 TASK 状态 By TASK_UID [N:未执行,W:任务中,Y:完成,X:失效]
        /// </summary>
        /// <param name="task_uid"></param>
        /// <param name="site"></param>
        public void UpdateTask(String task_uid, String site)
        {
            try
            {
                String sql = String.Format(@"update WCS_TASK_INFO set SITE = '{0}',UPDATE_TIME = NOW() where TASK_UID = '{0}'", site, task_uid);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新 TASK 状态 By WCS_NO [N:未执行,W:任务中,Y:完成,X:失效]
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="site"></param>
        public void UpdateTaskByWCSNo(String wcs_no, String site)
        {
            try
            {
                String sql = String.Format(@"update WCS_TASK_INFO set SITE = '{0}',UPDATE_TIME = NOW()
                                              where TASK_UID in (select TASK_UID_1 from WCS_COMMAND_MASTER where WCS_NO = '{1}')
                                                 or TASK_UID in (select TASK_UID_2 from WCS_COMMAND_MASTER where WCS_NO = '{1}')", site, wcs_no);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新 ITEM 明细：
        /// 设备编号(key = DEVICE)；
        /// 来源位置(key = LOC_FROM)；
        /// 作业状态(key = STATUS)[value = N:不可执行,Q:请求执行,W:任务中,X:失效,R:交接,E:出现异常,Y:完成任务]
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateItem(String wcs_no, String item_id, String key, String value)
        {
            try
            {
                String sql = String.Format(@"update WCS_TASK_ITEM set {0} = '{1}',UPDATE_TIME = NOW() where WCS_NO = '{2}' and ITEM_ID = '{3}'", key, value, wcs_no, item_id);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建 ITEM 初始任务
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        /// <param name="loc_to"></param>
        public void CreateItem(String wcs_no, String item_id, String loc_to)
        {
            try
            {
                String sql = String.Format(@"insert into WCS_TASK_ITEM(WCS_NO,ITEM_ID,LOC_TO) values('{0}','{1}','{2}')", wcs_no, item_id, loc_to);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 创建自定义 ITEM 任务
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        /// <param name="device"></param>
        /// <param name="loc_from"></param>
        /// <param name="loc_to"></param>
        /// <param name="status"></param>
        public void CreateCustomItem(String wcs_no, String item_id, String device, String loc_from, String loc_to, String status)
        {
            try
            {
                String sql = String.Format(@"insert into WCS_TASK_ITEM(WCS_NO,ITEM_ID,DEVICE,LOC_FROM,LOC_TO,STATUS) values('{0}','{1}','{2}','{3}','{4}','{5}')", 
                             wcs_no, item_id, device, loc_from, loc_to, status);
                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 删除'N'/'Q'状态的 ITEM 任务[ item_id 为空则全部清除]
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        public void DeleteItem(String wcs_no, String item_id)
        {
            String sql;
            try
            {
                if (String.IsNullOrWhiteSpace(item_id))
                {
                    sql = String.Format(@"delete from WCS_TASK_ITEM where STATUS in ('N','Q') and WCS_NO = '{0}'", wcs_no);
                }
                else
                {
                    sql = String.Format(@"delete from WCS_TASK_ITEM where STATUS in ('N','Q') and WCS_NO = '{0}' and ITEM_ID = '{1}'", wcs_no, item_id);
                }

                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取对应 item_id 属对接状态中的 Item List
        /// </summary>
        /// <param name="item_id"></param>
        /// <returns></returns>
        public List<WCS_TASK_ITEM> GetItemList_R(String item_id)
        {
            String sql;
            try
            {
                sql = String.Format(@"select * From WCS_TASK_ITEM where STATUS = 'R' and ITEM_ID = '{0}' order by CREATION_TIME", item_id);
                DataTable dtitem = mySQL.SelectAll(sql);
                if (tools.IsNoData(dtitem))
                {
                    return null;
                }
                List<WCS_TASK_ITEM> itemList = dtitem.ToDataList<WCS_TASK_ITEM>();
                return itemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

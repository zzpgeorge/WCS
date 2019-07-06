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
        /// 更新 COMMAND 状态[1.生成单号，2.请求执行，3.执行中，4.结束]
        /// </summary>
        /// <param name="step"></param>
        /// <param name="wcs_no"></param>
        public void UpdateStep(String wcs_no, String step)
        {
            try
            {
                mySQL.ExcuteSql(String.Format(@"update WCS_COMMAND_MASTER set STEP = '{0}',UPDATE_TIME = NOW() where WCS_NO = '{1}'", step, wcs_no));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建 ITEM 任务
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        /// <param name="loc_to"></param>
        public void CreateTask(String wcs_no, String item_id, String loc_to)
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
        /// 删除 ITEM 任务[ item_id 为空则全部清除]
        /// </summary>
        /// <param name="wcs_no"></param>
        /// <param name="item_id"></param>
        public void DeleteTask(String wcs_no, String item_id)
        {
            String sql;
            try
            {
                if (String.IsNullOrWhiteSpace(item_id))
                {
                    sql = String.Format(@"delete from WCS_TASK_ITEM where wcs_no = '{0}'", wcs_no);
                }
                else
                {
                    sql = String.Format(@"delete from WCS_TASK_ITEM where wcs_no = '{0}' and item_id = '{1}'", wcs_no, item_id);
                }

                mySQL.ExcuteSql(sql);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

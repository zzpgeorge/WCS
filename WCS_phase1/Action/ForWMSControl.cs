using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Models;
using System.Data;
using WCS_phase1.Functions;
using System.Configuration;
using WCS_phase1.Http;

namespace WCS_phase1.Action
{
    class ForWMSControl
    {
        /// <summary>
        /// 获取WMS资讯写入WCS数据库
        /// </summary>
        /// <param name="wms"></param>
        public bool WriteTaskToWCS(WmsModel wms)
        {
            try
            {
                MySQL mySQL = new MySQL();
                String sql = String.Format(@"insert into wcs_task_info(TASK_UID, TASK_TYPE, BARCODE, W_S_LOC, W_D_LOC) values('{0}','{1}','{2}','{3}','{4}')",
                    wms.Task_UID.ToString(), wms.Task_type.ToString(), wms.Barcode.ToString(), wms.W_S_Loc.ToString(), wms.W_D_Loc.ToString());
                mySQL.ExcuteSql(sql);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

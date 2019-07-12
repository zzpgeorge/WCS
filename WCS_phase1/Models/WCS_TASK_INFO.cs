using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// WCS任务资讯     WCS_TASK_INFO
    /// </summary>
    public class WCS_TASK_INFO
    {
        /// <summary>
        /// 任务UID
        /// </summary>
        public String TASK_UID { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public String TASK_TYPE { get; set; }

        /// <summary>
        /// 货物码
        /// </summary>
        public String BARCODE { get; set; }

        /// <summary>
        /// 来源货位
        /// </summary>
        public String W_S_LOC { get; set; }

        /// <summary>
        /// 目标货位
        /// </summary>
        public String W_D_LOC { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public String SITE { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UPDATE_TIME { get; set; }
    }

    /// <summary>
    /// Task类型
    /// </summary>
    public class TaskType
    {
        public const String AGV搬运 = "0";
        public const String 入库 = "1";
        public const String 出库 = "2";
        public const String 移仓 = "3";
        public const String 盘点 = "4";
    }

    /// <summary>
    /// Task状态
    /// </summary>
    public class TaskSite
    {
        public const String 未执行 = "N";
        public const String 任务中 = "W";
        public const String 完成 = "Y";
        public const String 失效 = "X";
    }
}

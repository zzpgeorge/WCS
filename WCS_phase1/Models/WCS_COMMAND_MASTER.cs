using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// WCS任务指令总控   WCS_COMMAND_MASTER
    /// </summary>
    public class WCS_COMMAND_MASTER
    {
        /// <summary>
        /// WCS单号
        /// </summary>
        public String WCS_NO { get; set; }
        
        /// <summary>
        /// 任务UID_1
        /// </summary>
        public String TASK_UID_1 { get; set; }

        /// <summary>
        /// 任务UID_2
        /// </summary>
        public String TASK_UID_2 { get; set; }

        /// <summary>
        /// 固定辊台位置
        /// </summary>
        public String FR_LOC { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        public String STEP { get; set; }

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
    /// Command状态
    /// </summary>
    public class CommandStep
    {
        public const String 生成单号 = "1";
        public const String 请求执行 = "2";
        public const String 执行中 = "3";
        public const String 结束 = "4";
    }
}

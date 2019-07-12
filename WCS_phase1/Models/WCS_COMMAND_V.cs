using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// WCS任务指令view   WCS_COMMAND_V
    /// </summary>
    public class WCS_COMMAND_V
    {
        /// <summary>
        /// WCS单号
        /// </summary>
        public String WCS_NO { get; set; }

        /// <summary>
        /// 固定辊台位置
        /// </summary>
        public String FRT { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        public String STEP { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public String TASK_TYPE { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }

        /// <summary>
        /// 任务1
        /// </summary>
        public String TASK_UID_1 { get; set; }

        /// <summary>
        /// 目的位置1
        /// </summary>
        public String LOC_1 { get; set; }

        /// <summary>
        /// 任务状态1
        /// </summary>
        public String SITE_1 { get; set; }

        /// <summary>
        /// 任务2
        /// </summary>
        public String TASK_UID_2 { get; set; }

        /// <summary>
        /// 目的位置2
        /// </summary>
        public String LOC_2 { get; set; }

        /// <summary>
        /// 任务状态2
        /// </summary>
        public String SITE_2 { get; set; }

    }
}

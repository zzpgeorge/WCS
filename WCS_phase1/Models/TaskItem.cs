using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// 子任务资讯
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// 任务UID
        /// </summary>
        public String TASK_UID { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public String ITEM_ID { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public String DEVICE { get; set; }

        /// <summary>
        /// 启动位置
        /// </summary>
        public String LOC_FROM { get; set; }

        /// <summary>
        /// 目的位置
        /// </summary>
        public String LOC_TO { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public String STATUS { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UPDATE_TIME { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// WCS指令资讯     WCS_TASK_ITEM
    /// </summary>
    public class WCS_TASK_ITEM
    {
        /// <summary>
        /// WCS单号
        /// </summary>
        public String WCS_NO { get; set; }

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

    /// <summary>
    /// Item作业ID
    /// </summary>
    public class ItemId
    {
        public const String 固定辊台入库 = "111";
        public const String 固定辊台出库 = "112";
        public const String 摆渡车入库 = "113";
        public const String 摆渡车出库 = "114";
        public const String 运输车入库 = "115";
        public const String 运输车出库 = "116";

        public const String 摆渡车定位固定辊台 = "011";
        public const String 摆渡车定位运输车对接 = "012";
        public const String 摆渡车定位 = "013";

        public const String 运输车定位 = "021";
        public const String 运输车复位1 = "022";    // 摆渡车对接待命点
        public const String 运输车复位2 = "023";    // 运输车对接待命点
        public const String 运输车对接定位 = "024";

        public const String 行车轨道定位 = "031";
        public const String 行车库存定位 = "032";
        public const String 行车取货 = "033";
        public const String 行车放货 = "034";
    }

    /// <summary>
    /// Item状态
    /// </summary>
    public class ItemStatus
    {
        public const String 不可执行 = "N";
        public const String 请求执行 = "Q";
        public const String 任务中 = "W";
        public const String 失效 = "X";
        public const String 交接 = "R";
        public const String 出现异常 = "E";
        public const String 完成任务 = "Y";
    }

    /// <summary>
    /// Item列名
    /// </summary>
    public class ItemColumnName
    {
        public const String 设备编号 = "DEVICE";
        public const String 来源位置 = "LOC_FROM";
        public const String 作业状态 = "STATUS";
    }
}

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
        public static String 固摆滚棒 = "111";
        public static String 摆运滚棒 = "112";
        public static String 运运滚棒 = "113";

        public static String 摆渡车定位 = "011";

        public static String 运输车定位 = "021";
        public static String 运输车复位1 = "022";
        public static String 运输车复位2 = "023";

        public static String 行车定位 = "031";
        public static String 行车取货 = "032";
        public static String 行车放货 = "033";
    }

    /// <summary>
    /// Item状态
    /// </summary>
    public class ItemStatus
    {
        public static String 不可执行 = "N";
        public static String 请求执行 = "Q";
        public static String 任务中 = "W";
        public static String 失效 = "X";
        public static String 交接 = "R";
        public static String 出现异常 = "E";
        public static String 完成任务 = "Y";
    }

    /// <summary>
    /// Item列名
    /// </summary>
    public class ItemColumnName
    {
        public static String 设备编号 = "DEVICE";
        public static String 来源位置 = "LOC_FROM";
        public static String 作业状态 = "STATUS";
    }
}

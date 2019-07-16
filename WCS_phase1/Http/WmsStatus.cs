using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Http
{

    /// <summary>
    /// WMS 状态定义 
    /// </summary>
    public enum WmsStatus
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 入库任务：从 WCS 发出进仓任务
        /// </summary>
        StockInTask = 1,

        /// <summary>
        /// 出库任务：从 WMS 发出出仓任务
        /// </summary>
        StockOutTask = 2,

        /// <summary>
        /// 移库任务：从 WMS 发出移仓任务
        /// </summary>
        StockMoveTask = 3,

        /// <summary>
        /// 盘点任务：从 WMS 发出盘点任务
        /// </summary>
        StockCheckTask = 4,

        /// <summary>
        /// 收到任务：WCS 收到任务
        /// </summary>
        ReceiveTask = 11,

        /// <summary>
        /// 任务安排：WCS 已经安排
        /// </summary>
        TaskArranged = 12,

        /// <summary>
        /// 正在操作：已经开始执行任务
        /// </summary>
        TaskOperating = 13,

        /// <summary>
        /// 没有货物：货位无货，中止任务
        /// </summary>
        NoGoods = 14,

        /// <summary>
        /// 载货运行：在源位取到货物在执行任务
        /// </summary>
        LoadRunning = 15,

        /// <summary>
        /// 任务暂停：因故暂停，等待运行
        /// </summary>
        TaskSuspend = 16,

        /// <summary>
        /// 操作故障：释放任务，设备故障，货物信息保留
        /// </summary>
        OperateError = 17,

        /// <summary>
        /// 到达站点：提交到达站点，获取新目标位
        /// </summary>
        SiteArrived = 18,

        /// <summary>
        /// 任务完成：提交任务完成
        /// </summary>
        TaskFinish = 19,

        /// <summary>
        /// 货物未知：取的货物不在任务中
        /// </summary>
        GoodsUnknown = 20,

        /// <summary>
        /// 货物损坏：释放任务，提交货物损坏信息，在 WMS 处理
        /// </summary>
        GoodsDamage = 21,

        /// <summary>
        /// 货物损坏：释放任务，提交货物损坏信息，在 WMS 处理
        /// </summary>
        PackDamage = 22,

        /// <summary>
        /// 目标位已满：目标位不能存放
        /// </summary>
        DestinationFill = 23,

        /// <summary>
        /// 目标位无货：目标位没有货物
        /// </summary>
        DestinationFree = 24,
        
        /// <summary>
        /// 未知
        /// </summary>
        None

    }


    /// <summary>
    /// 任务类型
    /// </summary>
    public class WmsParam
    {
        #region 任务类型
        /// <summary>
        /// 获取全部未完成任务信息
        /// </summary>
        public static string GetAllUnFinishTask = "A0";

        /// <summary>
        /// 获取全部入库任务 
        /// </summary>
        public static string GetAllStockInTask = "A1";

        /// <summary>
        /// 获取全部出库任务 
        /// </summary>
        public static string GetAllStockOutTask = "A2";


        /// <summary>
        /// 获取全部移库任务 
        /// </summary>
        public static string GetAllStockMoveTask = "A3";

        /// <summary>
        /// 获取全部盘点任务 
        /// </summary>
        public static string GetAllStockCheckTask = "A4";

        #endregion

        #region 带值参数名

        /// <summary>
        /// 货物二维码：根据货物二维码获取任务信息 
        /// </summary>
        public static string Barcode = "B";

        /// <summary>
        /// 任务UID：根据任务 UID 获取任务信息 
        /// </summary>
        public static string TaskUID = "C";

        /// <summary>
        /// 物货信息 
        /// </summary>
        public static string GoodsInfo = "D";

        /// <summary>
        /// 错误号 
        /// </summary>
        public static string ErrorCode = "E";

        /// <summary>
        /// 区位编码：指定区位编码 
        /// </summary>
        public static string AreaCode = "L";

        /// <summary>
        /// 文字信息 
        /// </summary>
        public static string TextMessage = "M";

        /// <summary>
        /// 执行人 
        /// </summary>
        public static string Operater = "O";

        /// <summary>
        /// 状态 
        /// </summary>
        public static string Status = "S";

        /// <summary>
        /// 位置
        /// </summary>
        public static string From = "F";

        /// <summary>
        ///日期时间 
        /// </summary>
        public static string DateTime = "T";

        /// <summary>
        /// 0 获取大区编号
        /// ? 设置大区编号 
        /// </summary>
        public static string AreaProcess = "PA";

        /// <summary>
        /// 0 获取工作模式 
        /// 1 区位单层 
        /// 2 层叠堆货 
        /// 3 行车层叠 
        /// 4 堆垛货架 
        /// </summary>
        public static string WorkProcess = "PB";

        /// <summary>
        /// ? 针对特殊位置设置层数（区、位、最高层） 
        /// 最后数为层数 
        /// </summary>
        public static string SpecialProcess = "PC";

        /// <summary>
        /// ? 获取可用货物基础资料 
        /// </summary>
        public static string GoodsProcess = "PD";

        /// <summary>
        /// 获取可用设备故障基础资料 
        /// </summary>
        public static string DeviceProcess = "P7";

        /// <summary>
        /// 获取可用货物损坏基础资料 
        /// </summary>
        public static string GooodsDamageProcess = "P9";

        /// <summary>
        /// 获取可用包装破损基础资料
        /// </summary>
        public static string PackDamageProcess = "P10";


        #endregion
    }
}

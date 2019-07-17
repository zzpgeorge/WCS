using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Action;

namespace WCS_phase1.Devices
{
    /// <summary>
    /// 自动行车 Automatic Bridge Crane
    /// </summary>
    public class ABC : Device
    {
        public ABC(string name) : base(name)
        {

        }

        #region 命令状态

        /// <summary>
        /// 命令完成
        /// </summary>
        public static byte CommandFinish = 0x00;

        /// <summary>
        /// 命令执行中
        /// </summary>
        public static byte CommandExecute = 0x01;

        /// <summary>
        /// 设备故障
        /// </summary>
        public static byte DeviceError = 0xFE;

        /// <summary>
        /// 命令错误
        /// </summary>
        public static byte CommandError = 0xFF;

        #endregion

        #region 任务类别

        /// <summary>
        /// 定位任务
        /// </summary>
        public static byte TaskLocate = 0x01;

        /// <summary>
        /// 取货任务
        /// </summary>
        public static byte TaskTake = 0x02;

        /// <summary>
        /// 放货任务
        /// </summary>
        public static byte TaskRelease = 0x03;

        /// <summary>
        /// 复位任务
        /// </summary>
        public static byte TaskRestoration = 0x04;

        #endregion

        #region 货物状态

        /// <summary>
        /// 无货
        /// </summary>
        public static byte GoodsNo = 0x00;

        /// <summary>
        /// 有货
        /// </summary>
        public static byte GoodsYes = 0x01;

        #endregion


        #region 指令解析

        /// <summary>
        /// 获取命令字头
        /// </summary>
        /// <returns></returns>
        public byte[] CommandHead()
        {
            return GetDoubleByte(0);
        }


        /// <summary>
        /// 行车号
        /// </summary>
        /// <returns></returns>
        public byte ABCNum()
        {
            return GetSingleByte(2);
        }

        /// <summary>
        /// 命令状态
        /// </summary>
        /// <returns></returns>
        public byte CommandStatus()
        {
            return GetSingleByte(3);
        }

        /// <summary>
        /// 目标X坐标
        /// </summary>
        /// <returns></returns>
        public byte[] GoodsXsite()
        {
            return GetThridByte(4);
        }

        /// <summary>
        /// 目标Y坐标
        /// </summary>
        /// <returns></returns>
        public byte[] GoodsYsite()
        {
            return GetDoubleByte(7);
        }

        /// <summary>
        /// 目标Z坐标
        /// </summary>
        /// <returns></returns>
        public byte[] GoodsZsite()
        {
            return GetDoubleByte(9);
        }

        /// <summary>
        /// 当前任务
        /// </summary>
        /// <returns></returns>
        public byte CurrentTask()
        {
            return GetSingleByte(11);
        }

        /// <summary>
        /// 当前X坐标
        /// </summary>
        /// <returns></returns>
        public byte[] CurrentXsite()
        {
            return GetThridByte(12);
        }

        /// <summary>
        /// 当前Y坐标
        /// </summary>
        /// <returns></returns>
        public byte[] CurrentYsite()
        {
            return GetDoubleByte(15);
        }

        /// <summary>
        /// 当前Z坐标
        /// </summary>
        /// <returns></returns>
        public byte[] CurrentZsite()
        {
            return GetDoubleByte(17);
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <returns></returns>
        public byte FinishTask()
        {
            return GetSingleByte(19);
        }

        /// <summary>
        /// 货物状态
        /// </summary>
        /// <returns></returns>
        public byte GoodsStatus()
        {
            return GetSingleByte(20);
        }

        /// <summary>
        /// 故障信息
        /// </summary>
        /// <returns></returns>
        public byte ErrorMessage()
        {
            return GetSingleByte(21);
        }

        #endregion

    }
}

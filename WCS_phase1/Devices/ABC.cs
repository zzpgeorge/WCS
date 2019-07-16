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
        public static byte CommandFinish = 0x00;
        public static byte CommandReceive = 0x01;

        public ABC(string name) : base(name)
        {

        }


        #region 需求功能

        //定位移动

        //夹取货物

        //释放货物

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
            return GetThridByte(7);
        }


        #endregion
    }
}

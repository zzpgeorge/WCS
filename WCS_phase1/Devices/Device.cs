using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Action;

namespace WCS_phase1.Devices
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class Device
    {
        internal string _name;

        internal byte[] bData;

        public Device(string name)
        {
            _name = name;
            bData = new byte[0];
        }

        /// <summary>
        /// 设备是否在线
        /// </summary>
        /// <returns></returns>
        internal bool IsAlive()
        {
            return DataControl._mSocket.IsAlive(_name);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        internal void Refresh()
        {
            bData = DataControl._mSocket.GetByteArr(_name);
        }

        /// <summary>
        /// 截取对应位置的数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        internal byte[] GetByteBegin(int start,int lenght)
        {
            Refresh();
            if (bData.Length < (start + lenght)) return new byte[lenght];
            return bData.Skip(start).Take(lenght).ToArray();
        }


        /// <summary>
        /// 获取单字节数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal byte GetSingleByte(int index)
        {
            Refresh();
            if (bData.Length < index) return new byte();
            return bData[index];
        }

        /// <summary>
        /// 获取两个字节数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal byte[] GetDoubleByte(int index)
        {
            if (bData.Length < (index + 2)) return new byte[]{};
            return GetByteBegin(index,2);
        }

        /// <summary>
        /// 获取三个字节数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal byte[] GetThridByte(int index)
        {
            if (bData.Length < (index+3)) return new byte[]{};
            return GetByteBegin(index, 3);
        }
    }
}

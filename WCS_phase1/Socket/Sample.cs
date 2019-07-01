using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//1.使用AsyncTcp命名空间
using AsyncTcp;

namespace WCS_phase1.Socket
{
    class Sample
    {
        public Sample()
        {
            //2.添加连接设备信息
            //添加设备名称，IP,端口   处理设备连接是根据名称来区分不同设备的信息
            bool resule = ClinetMaster.AddClient("AGV01", "127.0.0.1", 2000);

            //3.获取AGV01的信息
            Device device =  ClinetMaster.GetDeviceData("AGV01");

            Console.WriteLine("AGV01" + device.Name +  device.Sdata + device.UpDateTime);


            //4.获取所有设备信息
            List<Device> devices = ClinetMaster.GetDeviceList();

        }
    }
}

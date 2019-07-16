using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCS_phase1.Action;
using WCS_phase1.Devices;
using WCS_phase1.Http;
namespace WCS_phase1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DataControl dataControl;

        public MainWindow()
        {
            InitializeComponent();
            dataControl = new DataControl();

        }

        private void BtnTask_Click(object sender, RoutedEventArgs e)
        {
            TaskControl taskControl = new TaskControl();
            taskControl.Run_InInitial();
            taskControl.Run_ItemDevice();
            taskControl.Run_LinkDevice();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataControl._mHttp.DoPost("http://10.9.31.101/wms.php");
        }
    }
}

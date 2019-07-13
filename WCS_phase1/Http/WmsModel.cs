using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Http
{
    /// <summary>
    /// 接收WMS返回的信息类
    /// </summary>
    [Serializable]
    public class WmsModel
    {       
        
        /// <summary>
        /// 任务ID
        /// </summary>
        public string Task_UID { get; set; }
        
        /// <summary>
        /// 任务类型
        /// </summary>
        public WmsStatus Task_type { get; set; }
        
        /// <summary>
        /// 条形码
        /// </summary>
        public string Barcode { get; set; }
        
        /// <summary>
        /// 来源货位
        /// </summary>
        public string W_S_Loc { get; set; }
        
        /// <summary>
        /// 目标货位
        /// </summary>
        public string W_D_Loc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// 点位配置资讯    WCS_CONFIG_LOC
    /// </summary>
    public class WCS_CONFIG_LOC
    {
        /// <summary>
        /// WMS回馈位置
        /// </summary>
        public String WMS_LOC { get; set; }

        /// <summary>
        /// 固定辊台位置
        /// </summary>
        public String FRT_LOC { get; set; }

        /// <summary>
        /// 摆渡车定位
        /// </summary>
        public String ARF_LOC { get; set; }

        /// <summary>
        /// 运输车辊台①定位
        /// </summary>
        public String RGV_LOC_1 { get; set; }

        /// <summary>
        /// 运输车辊台②定位
        /// </summary>
        public String RGV_LOC_2 { get; set; }

        /// <summary>
        /// 行车轨道定位
        /// </summary>
        public String ABC_LOC_TRACK { get; set; }

        /// <summary>
        /// 行车库存定位
        /// </summary>
        public String ABC_LOC_STOCK { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }
    }
}
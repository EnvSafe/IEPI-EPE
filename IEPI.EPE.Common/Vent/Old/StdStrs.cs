using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.VentDesign
{
    /// <summary>
    /// 提供标准字符串代码，用于应用程序的资源本地化
    /// <para>正确使用此类的方法是：在您的程序中建立资源程序集，并使用此类提供的字符串为您的资源项命名。</para>
    /// </summary>
    public static class StdStrs
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string Succ = "Succ";

        #region 参数

        /// <summary>
        /// 最大爆炸压力
        /// </summary>
        public const string P_max = "P_max";
        /// <summary>
        /// 最大爆炸压力上升速率指数
        /// </summary>
        public const string K_st = "K_st";
        /// <summary>
        /// 爆炸等级
        /// </summary>
        public const string Explos = "Explos";

        /// <summary>
        /// 长径比
        /// </summary>
        public const string LDR = "LDR";
        /// <summary>
        /// 容器体积
        /// </summary>
        public const string V = "V";
        /// <summary>
        /// 容器高度
        /// </summary>
        public const string L = "L";
        /// <summary>
        /// 泄压效率
        /// </summary>
        public const string E_F = "E_F";
        /// <summary>
        /// 容器设计强度
        /// </summary>
        public const string P = "P";
        /// <summary>
        /// 操作压力
        /// </summary>
        public const string P_op = "P_op";
        /// <summary>
        /// 静开启压力
        /// </summary>
        public const string P_stat = "P_stat";
        /// <summary>
        /// 动开启压力
        /// </summary>
        public const string P_dyn = "P_dyn";
        /// <summary>
        /// 最大泄爆压力
        /// </summary>
        public const string P_red_max = "P_red_max";

        /// <summary>
        /// 进料空气流量
        /// </summary>
        public const string Q = "Q";
        /// <summary>
        /// 进料空气输送速度
        /// </summary>
        public const string u_L = "u_L";
        /// <summary>
        /// 进料管径
        /// </summary>
        public const string D_F = "D_F";

        #endregion
    }
}

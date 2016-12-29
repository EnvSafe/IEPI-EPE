using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    /// <summary>
    /// 用于有效容积计算的容器
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// 泄压口上边缘距离容器箱体底部的距离，m
        /// </summary>
        double VentExitUpperDistance { get; }
        /// <summary>
        /// 泄压口下边缘距离容器箱体顶部的距离，m
        /// </summary>
        double VentExitLowerDistance { get; }
        
        /// <summary>
        /// 主体截面
        /// </summary>
        ISection BodySection { get; }
        /// <summary>
        /// 主体高度, m
        /// </summary>
        double BodyHeight { get; }
        
        /// <summary>
        /// 获取容器的有效容积，m³
        /// <para>有效容积是指，爆炸火焰传播的容积</para>
        /// </summary>
        /// <returns></returns>
        double GetEffectVolumn();
        /// <summary>
        /// 获取火焰体积，m³
        /// </summary>
        /// <returns></returns>
        double GetFireVolumn();
        /// <summary>
        /// 有效火焰传播距离,m
        /// </summary>
        /// <returns></returns>
        double GetEffectLen();

    }

    /// <summary>
    /// 为容器类型提供基础拓展
    /// </summary>
    public static class ContainerExtension
    {
        /// <summary>
        /// 获取有效长径比
        /// </summary>
        /// <returns></returns>
        public static double GetEffectL2D(this IContainer self)
        {
            return self.GetEffectLen() / self.GetEffectD();
        }
        /// <summary>
        /// 获取有效直径，m
        /// </summary>
        /// <returns></returns>
        public static double GetEffectD(this IContainer self)
        {
            return Math.Sqrt(4.0 * self.GetEffectArea() / Math.PI);
        }
        /// <summary>
        /// 获取有效横截面积，m2
        /// </summary>
        /// <returns></returns>
        public static double GetEffectArea(this IContainer self)
        {
            return self.GetFireVolumn() / self.GetEffectLen();
        }

    }
}

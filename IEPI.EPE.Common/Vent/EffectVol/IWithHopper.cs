using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    /// <summary>
    /// 带有下料斗的容器
    /// </summary>
    public interface IWithHopper: IContainer
    {
        /// <summary>
        /// 卸料斗高度，m
        /// </summary>
        double HopperHeight { get; }

        /// <summary>
        /// 卸料斗上截面
        /// </summary>
        ISection HopperUpSection { get; }
        /// <summary>
        /// 卸料斗下截面
        /// </summary>
        ISection HopperLowSection { get; }
    }

    /// <summary>
    /// 为带斗容器类型提供基础拓展
    /// </summary>
    public static class WithHopperExtension
    {
        /// <summary>
        /// 获取卸料斗体积, m3
        /// </summary>
        /// <returns></returns>
        public static double GetHopperVolumn(this IWithHopper self)
        {
            var s1 = self.HopperLowSection.GetArea();
            var s2 = self.HopperUpSection.GetArea();
            return (s1 + s2 + Math.Sqrt(s1 * s2)) * self.HopperHeight / 3.0;
        }
    }
}

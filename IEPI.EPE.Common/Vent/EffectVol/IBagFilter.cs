using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    /// <summary>
    /// 布袋除尘器接口约束
    /// </summary>
    public interface IBagFilter : IContainer
    {
        /// <summary>
        /// 布袋直径，m
        /// </summary>
        double BagDiameter { get; }
        /// <summary>
        /// 布袋长度，m
        /// </summary>
        double BagLength { get; }
        /// <summary>
        /// 布袋数量
        /// </summary>
        int BagCount { get; }

        /// <summary>
        /// 净室尺度，m
        /// </summary>
        double DimensionCleanChamber { get; }
        /// <summary>
        /// 脏室尺度，m
        /// </summary>
        double DimensionDirtyChamber { get; }
    }
    /// <summary>
    /// 为布袋除尘器提供基础拓展
    /// </summary>
    public static class BagFilterExtension
    {
        /// <summary>
        /// 获取单个布袋体积，m3
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetSingleBagVolumn(this IBagFilter self)
        {
            return Math.PI * self.BagDiameter * self.BagDiameter * self.BagLength / 4.0;
        }
        /// <summary>
        /// 获取布袋总体积，m3
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetTotalBagVolumn(this IBagFilter self)
        {
            return self.BagCount * self.GetSingleBagVolumn();
        }
    }
}

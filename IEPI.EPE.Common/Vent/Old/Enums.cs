using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*提供通用枚举*/

namespace IEPI.EPE.VentDesign
{
    [Obsolete("版本2.2之前的接口已停止维护")]
    public enum ContainerShape
    {
        Unknown = 0,
        Cylindrical = 1,
        CylHopper = 2,
        Rectangular = 3,
        RectHopper = 4
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public enum VentPosition
    {
        Unknown = 0,
        Top = 1,
        SideDefault = 2,
        SideHigh = 3,
        SideLow = 4,
    }

    /// <summary>
    /// 承载爆炸压力的主体
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public enum Subject
    {
        /// <summary>
        /// 未知主体
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 容器、筒仓或设备
        /// </summary>
        Container = 1,
        /// <summary>
        /// 管道
        /// </summary>
        Pipe = 2,
        /// <summary>
        /// 建筑物
        /// </summary>
        Building = 3,
        ContainerWithPipes = 4
    }

    /// <summary>
    /// 枚举筒仓的进料方式
    /// </summary>
    //[Obsolete("版本2.2之前的接口已停止维护")]
    public enum FeedingWay
    {
        /// <summary>
        /// 未指定的进料方式
        /// </summary>
        None = 0,
        /// <summary>
        /// 气动轴向进料
        /// </summary>
        PneumaticAxial = 1,
        /// <summary>
        /// 气动切向进料
        /// </summary>
        PneumaticTangential = 2,
        /// <summary>
        /// 自由落体
        /// </summary>
        FreeFalling = 3
    }

    /// <summary>
    /// 枚举泄爆片的形状
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public enum RaptureDiskShape
    {
        /// <summary>
        /// 圆形泄爆片
        /// </summary>
        Circle = 0,
        /// <summary>
        /// 矩形泄爆片
        /// </summary>
        Rectangle = 1
    }

    /// <summary>
    /// 表示粉尘的爆炸等级
    /// </summary>
    //[Obsolete("版本2.2之前的接口已停止维护")]
    public enum ExplosionLevel
    {
        Unknown = 0,
        St1 = 1,
        St2 = 2,
        St3 = 3,
        St4 = 4
    }
}

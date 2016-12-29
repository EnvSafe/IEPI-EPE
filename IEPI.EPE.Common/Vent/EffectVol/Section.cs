using Heroius.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    /// <summary>
    /// 排料口
    /// </summary>
    public interface ISection: INotifyAllPropertiesChanged
    {
        /// <summary>
        /// 计算排料口面积
        /// </summary>
        /// <returns></returns>
        double GetArea();
    }

    /// <summary>
    /// 圆形排料口
    /// </summary>
    public class CircleSection : ISection
    {
        /// <summary>
        /// 直径，m
        /// </summary>
        public double Diameter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 计算圆形排料口面积，m2
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            return Math.PI * Math.Pow(Diameter / 2.0, 2.0);
        }

        public void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Diameter"));
            }
        }
    }
    /// <summary>
    /// 矩形排料口
    /// </summary>
    public class SquareSection : ISection
    {
        /// <summary>
        /// 长，m
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 宽，m
        /// </summary>
        public double Width { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 计算矩形排料口面积，m2
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            return Length * Width;
        }

        public void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Length"));
                PropertyChanged(this, new PropertyChangedEventArgs("Width"));
            }
        }
    }

    /// <summary>
    /// 列举当前所有可用的截面类型
    /// </summary>
    public enum SectionType
    {
        /// <summary>
        /// 圆形截面
        /// </summary>
        Circle,
        /// <summary>
        /// 矩形截面
        /// </summary>
        Square
    }
}

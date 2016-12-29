using Heroius.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    /// <summary>
    /// 卧式布袋除尘器
    /// </summary>
    public class HorizontalBagFilter: IWithHopper, IBagFilter, INotifyAllPropertiesChanged
    {
        /// <summary>
        /// 卧式布袋除尘器
        /// </summary>
        /// <param name="DischargeSectionType">卸料口截面类型</param>
        public HorizontalBagFilter(SectionType DischargeSectionType)
        {
            HopperUpSection = new SquareSection();
            switch (DischargeSectionType)
            {
                case SectionType.Circle:
                    HopperLowSection = new CircleSection();
                    break;
                case SectionType.Square:
                default:
                    HopperLowSection = new SquareSection();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseAllPropertiesChangedEvent()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("BodyHeight"));
            PropertyChanged(this, new PropertyChangedEventArgs("HopperHeight"));
            HopperLowSection.RaiseAllPropertiesChangedEvent();
            HopperUpSection.RaiseAllPropertiesChangedEvent();
            PropertyChanged(this, new PropertyChangedEventArgs("HopperHeight"));
            PropertyChanged(this, new PropertyChangedEventArgs("VentExitLowerDistance"));
            PropertyChanged(this, new PropertyChangedEventArgs("VentExitUpperDistance"));
            PropertyChanged(this, new PropertyChangedEventArgs("DimensionCleanChamber"));
            PropertyChanged(this, new PropertyChangedEventArgs("BagDiameter"));
            PropertyChanged(this, new PropertyChangedEventArgs("BagLength"));
            PropertyChanged(this, new PropertyChangedEventArgs("BagCount"));
        }

        #region Container

        /// <summary>
        /// 获取有效体积
        /// </summary>
        /// <returns></returns>
        public double GetEffectVolumn()
        {
            return DimensionDirtyChamber * BodyHeight * (BodySection as SquareSection).Length - this.GetTotalBagVolumn() + this.GetHopperVolumn();
        }
        /// <summary>
        /// 获取火焰体积，m³
        /// </summary>
        /// <returns></returns>
        public double GetFireVolumn()
        {
            var v1 = VentExitUpperDistance + HopperHeight / 3.0;
            var v2 = VentExitLowerDistance;
            if (v1 > v2) return DimensionDirtyChamber * (BodySection as SquareSection).Length * VentExitUpperDistance + this.GetHopperVolumn() / 3.0;
            else return DimensionDirtyChamber * (BodySection as SquareSection).Length * VentExitLowerDistance;
        }
        /// <summary>
        /// 有效火焰传播距离,m
        /// </summary>
        /// <returns></returns>
        public double GetEffectLen()
        {
            var v1 = VentExitUpperDistance + HopperHeight / 3.0;
            var v2 = VentExitLowerDistance;
            return Math.Max(v1, v2);
        }

        /// <summary>
        /// 除尘器截面
        /// </summary>
        public ISection BodySection { get { return HopperUpSection; } }

        /// <summary>
        /// 泄压口上边缘距离容器箱体底部的距离，m
        /// </summary>
        public double VentExitUpperDistance { get; set; }
        /// <summary>
        /// 泄压口下边缘距离容器箱体顶部的距离,m
        /// </summary>
        public double VentExitLowerDistance { get; set; }
        /// <summary>
        /// 箱体高度，m
        /// </summary>
        public double BodyHeight { get; set; }

        #endregion

        #region BagFilter

        /// <summary>
        /// 净室高度，m
        /// </summary>
        public double DimensionCleanChamber { get; set; }
        /// <summary>
        /// 脏室高度，m
        /// </summary>
        public double DimensionDirtyChamber { get { return (BodySection as SquareSection).Width - DimensionCleanChamber; } set { (BodySection as SquareSection).Width = value + DimensionCleanChamber; } }

        /// <summary>
        /// 布袋直径，m
        /// </summary>
        public double BagDiameter { get; set; }
        /// <summary>
        /// 布袋长度，m
        /// </summary>
        public double BagLength { get; set; }
        /// <summary>
        /// 布袋数量
        /// </summary>
        public int BagCount { get; set; }

        #endregion

        #region Hopper

        /// <summary>
        /// 卸料斗高度，m
        /// </summary>
        public double HopperHeight { get; set; }

        /// <summary>
        /// 卸料斗上截面
        /// </summary>
        public ISection HopperUpSection { get; set; }
        /// <summary>
        /// 卸料斗下截面
        /// </summary>
        public ISection HopperLowSection { get; set; }

        #endregion
    }
}

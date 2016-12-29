using Heroius.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    public class HopperContainer : IWithHopper, INotifyAllPropertiesChanged
    {
        public HopperContainer(SectionType BodySectionType, SectionType DischargeSectionType)
        {
            switch (BodySectionType)
            {
                case SectionType.Circle:
                    HopperUpSection = new CircleSection();
                    break;
                case SectionType.Square:
                default:
                    HopperUpSection = new SquareSection();
                    break;
            }
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

        public double BodyHeight { get; set; }

        public ISection BodySection { get { return HopperUpSection; } }

        public double HopperHeight { get; set; }

        public ISection HopperLowSection { get; set; }

        public ISection HopperUpSection { get; set; }

        public double VentExitLowerDistance { get; set; }

        public double VentExitUpperDistance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public double GetEffectLen()
        {
            return Math.Max(VentExitUpperDistance + HopperHeight / 3.0, VentExitLowerDistance);
        }

        public double GetEffectVolumn()
        {
            return BodySection.GetArea() * BodyHeight + this.GetHopperVolumn();
        }

        public double GetFireVolumn()
        {
            var v2 = VentExitUpperDistance + HopperHeight / 3.0;
            if (v2 > VentExitLowerDistance) return VentExitUpperDistance * BodySection.GetArea() + this.GetHopperVolumn() / 3.0;
            else return VentExitLowerDistance * BodySection.GetArea();
        }

        public void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("BodyHeight"));
                PropertyChanged(this, new PropertyChangedEventArgs("HopperHeight"));
                HopperLowSection.RaiseAllPropertiesChangedEvent();
                HopperUpSection.RaiseAllPropertiesChangedEvent();
                PropertyChanged(this, new PropertyChangedEventArgs("HopperHeight"));
                PropertyChanged(this, new PropertyChangedEventArgs("VentExitLowerDistance"));
                PropertyChanged(this, new PropertyChangedEventArgs("VentExitUpperDistance"));
            }
        }
    }
}

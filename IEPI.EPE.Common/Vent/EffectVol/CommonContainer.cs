using Heroius.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent.EffectVol
{
    public class CommonContainer : IContainer, INotifyAllPropertiesChanged
    {
        public CommonContainer(SectionType BodySectionType)
        {
            switch (BodySectionType)
            {
                case SectionType.Circle:
                    BodySection = new CircleSection();
                    break;
                case SectionType.Square:
                default:
                    BodySection = new SquareSection();
                    break;
            }
        }

        public double BodyHeight { get; set; }

        public ISection BodySection { get; set; }

        public double VentExitLowerDistance { get; set; }

        public double VentExitUpperDistance { get; set; }

        public double GetEffectLen()
        {
            return Math.Max(VentExitLowerDistance, VentExitUpperDistance);
        }

        public double GetEffectVolumn()
        {
            return BodySection.GetArea() * BodyHeight;
        }

        public double GetFireVolumn()
        {
            return GetEffectLen() * BodySection.GetArea();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseAllPropertiesChangedEvent()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("BodyHeight"));
                PropertyChanged(this, new PropertyChangedEventArgs("VentExitLowerDistance"));
                PropertyChanged(this, new PropertyChangedEventArgs("VentExitUpperDistance"));
                BodySection.RaiseAllPropertiesChangedEvent();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.VentDesign.VDI.No3673_2002
{
    [Obsolete("版本2.2之前的接口已停止维护")]
    public class ReliefAreaGenerator : IReliefArea
    {
        public double ReliefArea(double Pmax, double Kst, double Pred, double Pstat, double V, double HDRatio)
        {
            Pmax *= 10;
            Kst *= 10;
            Pred *= 10;
            Pstat *= 10;
            if (Pstat < 0.1)
                Pstat = 0.1;
            double B1 = 3.264e-5 * Pmax * Kst * Math.Pow(Pred, -0.569);
            double B2 = 0.27 * (Pstat - 0.1) * Math.Pow(Pred, -0.5);
            double B = (B1 + B2) * Math.Pow(V, 0.753);
            double A = 0;
            if (Pred < 1.5)
            {
                double C = -4.305 * Math.Log10(Pred) + 0.758;
                A = B * (1 + C * Math.Log10(HDRatio));
            }
            else
            {
                A = B;
            }
            return A;
        }

        public double ReliefAreaOfSoli(double Pmax, double Kst, double Pred, double Pstat, double V, double H, double Df, double HDRatio, FeedingWay Feeding)
        {
            double Dz, Y, X;
            switch (Feeding)
            {
                case FeedingWay.PneumaticAxial:
                    Pmax *= 10;
                    Kst *= 10;
                    Pred *= 10;
                    Pstat *= 10;
                    Dz = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                    Y = 1.0715 * Math.Pow(Pred, -1.27);
                    X = (8.6 * Math.Log10(Pred) - 6.0) / Dz - 5.5 * Math.Log10(Pred) + 3.7;
                    X *= 0.011 * Kst * Df;
                    double A = 0;
                    if (H > 10)
                        A = 0.1 * H * X * (1 + Y * Math.Log10(HDRatio));
                    else
                        A = X * (1 + Y * Math.Log10(HDRatio));
                    return A;
                case FeedingWay.PneumaticTangential:
                    Pmax *= 10;
                    Kst *= 10;
                    Pred *= 10;
                    Pstat *= 10;
                    double k;
                    if (Pred >= 0.1 && Pred <= 1)
                        k = 1;
                    else if (Pred > 1 && Pred <= 1.7)
                        k = 2;
                    else
                    {
                        throw new Exception("容器设计强度Pred不在有效范围内");
                    }
                    Dz = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                    Y = 0.166 * Math.Pow(Math.E, Kst / 129.0) * Math.Pow(Pred, -1.27 / k);
                    X = (8.6 * Math.Log10(Pred) / k - Kst / 44.0 - 0.513) / Dz - 5.5 * Math.Log10(Pred) / k + Kst / 69 + 0.191;
                    X *= 0.011 * Kst * Df;
                    return X * (1 + Y * Math.Log10(HDRatio));
                default:
                    throw new Exception("未实现的进料方式！");
            }
        }
    }
}

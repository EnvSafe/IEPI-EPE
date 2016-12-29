using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.VentDesign.EN.No14491_2006
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
            if (Pred < 1.5 && Pred >= 0.1)
            {
                double C = -4.305 * Math.Log10(Pred) + 0.758;
                A = B * (1 + C * Math.Log10(HDRatio));
            }
            else if (Pred >= 1.5 && Pred <= 2.0)
            {
                A = B;
            }
            else
            {
                throw new Exception("Pred>0.2MPa,超出有效范围");
            }
            return A;
        }

        public double ReliefAreaOfSoli(double Pmax, double Kst, double Pred, double Pstat, double V, double H, double Df, double HDRatio, FeedingWay Feeding)
        {
            return ReliefArea(Pmax, Kst, Pred, Pstat, V, HDRatio);
        }
    }

    #region LDR

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class LDRObject_Cylindrical : LDRBase, ILDR
    {
        public LDRObject_Cylindrical(double H, double D)
        {
            this.DefineParams("H", "D");
            this.H = H;
            this.D = D;
        }

        #region 参数表

        public double H
        {
            get { return this.Parameters[0].Value; }
            set { this.Parameters[0].Value = value; }
        }

        public double D
        {
            get { return this.Parameters[1].Value; }
            set { this.Parameters[1].Value = value; }
        }

        #endregion

        public double GetHDR()
        {
            double Veff = Math.PI * D * D * 0.25 * H;
            double Aeff = Veff / H;
            double Deff = Math.Pow(4 * Aeff / Math.PI, 0.5);
            return H / Deff;
        }
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class LDRObject_CylindricHopper : LDRBase, ILDR
    {
        public LDRObject_CylindricHopper(double H1, double D1, double H2, double D2)
        {
            this.DefineParams("H1", "D1", "H2", "D2");
            this.H1 = H1; this.D1 = D1;
            this.H2 = H2; this.D2 = D2;
        }

        #region 参数表
        public double H1
        {
            get { return this.Parameters[0].Value; }
            set { this.Parameters[0].Value = value; }
        }
        public double D1
        {
            get { return this.Parameters[1].Value; }
            set { this.Parameters[1].Value = value; }
        }
        public double H2
        {
            get { return this.Parameters[2].Value; }
            set { this.Parameters[2].Value = value; }
        }
        public double D2
        {
            get { return this.Parameters[3].Value; }
            set { this.Parameters[3].Value = value; }
        }
        #endregion

        public double GetHDR()
        {
            double Vc = 0.25 * Math.PI * D1 * D1 * H1;
            double Vh = 1.0 / 3.0 * Math.PI * H2 * (D1 * D1 + D1 * D2 + D2 * D2) / 12.0;
            double Veff = Vc + Vh;
            double h = H1 + 1.0 / 3.0 * H2;
            double Aeff = Veff / h;
            double Deff = Math.Pow(4.0 * Aeff / Math.PI, 0.5);
            return h / Deff;
        }
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class LDRObject_RecHopper : LDRBase, ILDR
    {
        public LDRObject_RecHopper(double H1, double H2, double a1, double a2, double b1, double b2)
        {
            this.DefineParams("H1", "H2", "a1", "a2", "b1", "b2");
            this.H1 = H1; this.H2 = H2; this.a1 = a1; this.a2 = a2; this.b1 = b1; this.b2 = b2;
        }

        #region 参数表
        public double H1
        {
            get { return this.Parameters[0].Value; }
            set { this.Parameters[0].Value = value; }
        }
        public double H2
        {
            get { return this.Parameters[1].Value; }
            set { this.Parameters[1].Value = value; }
        }
        public double a1
        {
            get { return this.Parameters[2].Value; }
            set { this.Parameters[2].Value = value; }
        }
        public double a2
        {
            get { return this.Parameters[3].Value; }
            set { this.Parameters[3].Value = value; }
        }
        public double b1
        {
            get { return this.Parameters[4].Value; }
            set { this.Parameters[4].Value = value; }
        }
        public double b2
        {
            get { return this.Parameters[5].Value; }
            set { this.Parameters[5].Value = value; }
        }
        #endregion

        public double GetHDR()
        {
            double Vr = a1 * b1 * H1;
            double Vh = H2 * a2 * (b1 - b2) / 2.0 + H2 * b2 * (a1 - a2) / 2.0 + H2 * (a1 - a2) * (b1 - b2) / 3.0 + a2 * b2 * H2;
            double Veff = Vr + Vh / 3.0;
            double h = H1 + H2 / 3.0;
            double Aeff = Veff / h;
            double Deff = Math.Pow(Aeff, 0.5);
            return h / Deff;
        }
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class LDRObject_RecHopperClosed : LDRBase, ILDR
    {
        public LDRObject_RecHopperClosed(double H, double a, double b)
        {
            this.DefineParams("H1", "a", "b");
            this.H = H; this.a = a; this.b = b;
        }

        #region 参数表
        public double H
        {
            get { return this.Parameters[0].Value; }
            set { this.Parameters[0].Value = value; }
        }
        public double a
        {
            get { return this.Parameters[1].Value; }
            set { this.Parameters[1].Value = value; }
        }
        public double b
        {
            get { return this.Parameters[2].Value; }
            set { this.Parameters[2].Value = value; }
        }
        #endregion

        public double GetHDR()
        {
            double Veff = a * b * H;
            double Aeff = Veff / H;
            double Deff = 2.0 * Aeff / (a + b);
            return H / Deff;
        }
    }

    #endregion
}

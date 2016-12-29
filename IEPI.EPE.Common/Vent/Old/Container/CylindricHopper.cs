// Type: IEPI.EPE.VentDesign.CylindricHopper
// Assembly: IEPI.EPE.VentDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 640E6988-C52D-4A3F-B5C7-56FB453DA09F
// Assembly location: C:\Users\Heroius\Desktop\Temp\reverse\EPE\IEPI.EPE.VentDesign.dll

using System;

namespace IEPI.EPE.VentDesign
{
  public class CylindricHopper : HDR, ILDR
  {
    public double H1
    {
      get
      {
        return this.Parameters[0].Value;
      }
      set
      {
        this.Parameters[0].Value = value;
      }
    }

    public double D1
    {
      get
      {
        return this.Parameters[1].Value;
      }
      set
      {
        this.Parameters[1].Value = value;
      }
    }

    public double H2
    {
      get
      {
        return this.Parameters[2].Value;
      }
      set
      {
        this.Parameters[2].Value = value;
      }
    }

    public double D2
    {
      get
      {
        return this.Parameters[3].Value;
      }
      set
      {
        this.Parameters[3].Value = value;
      }
    }

    public CylindricHopper(double H1, double D1, double H2, double D2)
    {
      this.DefineParams("H1", "D1", "H2", "D2");
      this.H1 = H1;
      this.D1 = D1;
      this.H2 = H2;
      this.D2 = D2;
    }

    public double GetHDR()
    {
      double num1 = Math.PI / 4.0 * this.D1 * this.D1 * this.H1 + Math.PI / 3.0 * this.H2 * (this.D1 * this.D1 + this.D1 * this.D2 + this.D2 * this.D2) / 12.0;
      double num2 = this.H1 + 1.0 / 3.0 * this.H2;
      double num3 = Math.Pow(4.0 * (num1 / num2) / Math.PI, 0.5);
      return num2 / num3;
    }
  }
}

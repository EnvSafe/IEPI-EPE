// Type: IEPI.EPE.VentDesign.RecHopper
// Assembly: IEPI.EPE.VentDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 640E6988-C52D-4A3F-B5C7-56FB453DA09F
// Assembly location: C:\Users\Heroius\Desktop\Temp\reverse\EPE\IEPI.EPE.VentDesign.dll

using System;

namespace IEPI.EPE.VentDesign
{
  public class RecHopper : HDR, ILDR
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

    public double H2
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

    public double a1
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

    public double a2
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

    public double b1
    {
      get
      {
        return this.Parameters[4].Value;
      }
      set
      {
        this.Parameters[4].Value = value;
      }
    }

    public double b2
    {
      get
      {
        return this.Parameters[5].Value;
      }
      set
      {
        this.Parameters[5].Value = value;
      }
    }

    public RecHopper(double H1, double H2, double a1, double a2, double b1, double b2)
    {
      this.DefineParams("H1", "H2", "a1", "a2", "b1", "b2");
      this.H1 = H1;
      this.H2 = H2;
      this.a1 = a1;
      this.a2 = a2;
      this.b1 = b1;
      this.b2 = b2;
    }

    public double GetHDR()
    {
      double num1 = this.a1 * this.b1 * this.H1 + (this.H2 * this.a2 * (this.b1 - this.b2) / 2.0 + this.H2 * this.b2 * (this.a1 - this.a2) / 2.0 + this.H2 * (this.a1 - this.a2) * (this.b1 - this.b2) / 3.0 + this.a2 * this.b2 * this.H2) / 3.0;
      double num2 = this.H1 + this.H2 / 3.0;
      double num3 = Math.Pow(num1 / num2, 0.5);
      return num2 / num3;
    }
  }
}

// Type: IEPI.EPE.VentDesign.Cylindrical
// Assembly: IEPI.EPE.VentDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 640E6988-C52D-4A3F-B5C7-56FB453DA09F
// Assembly location: C:\Users\Heroius\Desktop\Temp\reverse\EPE\IEPI.EPE.VentDesign.dll

using System;

namespace IEPI.EPE.VentDesign
{
  public class Cylindrical : HDR, ILDR
  {
    public double H
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

    public double D
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

    public Cylindrical(double H, double D)
    {
      this.DefineParams("H", "D");
      this.H = H;
      this.D = D;
    }

    public double GetHDR()
    {
      return this.H / Math.Pow(4.0 * (Math.PI * this.D * this.D * 0.25 * this.H / this.H) / Math.PI, 0.5);
    }
  }
}

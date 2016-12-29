// Type: IEPI.EPE.VentDesign.RecHopperClosed
// Assembly: IEPI.EPE.VentDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 640E6988-C52D-4A3F-B5C7-56FB453DA09F
// Assembly location: C:\Users\Heroius\Desktop\Temp\reverse\EPE\IEPI.EPE.VentDesign.dll

namespace IEPI.EPE.VentDesign
{
  public class RecHopperClosed : HDR, ILDR
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

    public double a
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

    public double b
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

    public RecHopperClosed(double H, double a, double b)
    {
      this.DefineParams("H1", "a", "b");
      this.H = H;
      this.a = a;
      this.b = b;
    }

    public double GetHDR()
    {
      return this.H / (2.0 * (this.a * this.b * this.H / this.H) / (this.a + this.b));
    }
  }
}

// Type: IEPI.EPE.VentDesign.HDR
// Assembly: IEPI.EPE.VentDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 640E6988-C52D-4A3F-B5C7-56FB453DA09F
// Assembly location: C:\Users\Heroius\Desktop\Temp\reverse\EPE\IEPI.EPE.VentDesign.dll


namespace IEPI.EPE.VentDesign
{
  public abstract class HDR
  {
    public ParamCollection<double> Parameters;

    public void DefineParams(params string[] Names)
    {
      if (this.Parameters == null)
        this.Parameters = new ParamCollection<double>();
      foreach (string Name in Names)
        this.Parameters.Add(Name, 0.0);
    }

    public void SetParam(string Name, double Value)
    {
      this.Parameters.SetParam(Name, Value);
    }

    public double GetParam(string Name)
    {
      return Parameters.GetParam(Name).Value;
    }
  }
}

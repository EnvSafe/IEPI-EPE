using System;
using System.Drawing;
using ZedGraph;
using System.Windows.Forms;

namespace IEPI.EPE.VentDesign.NFPA.No68_2007
{
    /// <summary>
    /// 表示用于计算NFPA68 2007泄压面积的图版
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class ReliefAreaPane
    {
        /// <summary>
        /// 实例化一个选取泄压面积参数的图版
        /// </summary>
        /// <param name="Container">图版的容器</param>
        /// <param name="LaterProcess">选取值时触发的拓展函数</param>
        public ReliefAreaPane(Panel Container, PickValue LaterProcess)
        {
            //初始化ZedGraph控件
            ZgMe = new ZedGraphControl();
            GpMe = ZgMe.GraphPane;
            GpMe.Title.FontSpec.Size = 16;
            GpMe.XAxis.MajorTic.IsOpposite = GpMe.YAxis.MajorTic.IsOpposite =
            GpMe.XAxis.MinorTic.IsOpposite = GpMe.YAxis.MinorTic.IsOpposite = false;
            GpMe.XAxis.MajorTic.IsOutside = GpMe.YAxis.MajorTic.IsOutside =
            GpMe.XAxis.MinorTic.IsOutside = GpMe.YAxis.MinorTic.IsOutside = false;
            GpMe.XAxis.MajorGrid.IsVisible = GpMe.YAxis.MajorGrid.IsVisible =
            GpMe.XAxis.MinorGrid.IsVisible = GpMe.YAxis.MinorGrid.IsVisible = true;
            ZgMe.IsShowContextMenu = false;
            ZgMe.IsShowPointValues = true;
            ZgMe.IsEnableZoom = ZgMe.IsEnableWheelZoom = false;
            ZgMe.IsZoomOnMouseCenter = false;
            ZgMe.IsEnableHPan = ZgMe.IsEnableVPan = false;
            ZgMe.MouseClick += new MouseEventHandler(ZgMe_MouseClick);
            if (LaterProcess == null) this.LaterProcess = Empty;
            else this.LaterProcess = LaterProcess;
            //定位
            Container.Controls.Add(ZgMe);
            ZgMe.Dock = DockStyle.Fill;
            //
            Picking = false;
            //初始化曲线集集
            AllGraphs = new PureLineCollection[11];
            AllGraphs[0] = new PureLineCollection("Vent Size For Dusts(50≤Kst≤300)", 300, 50, "Kst", "0", 0.05, 0, "FactorA1", "0.00");
            AllGraphs[0].Add("Pstat=0.5 bar", FillPairKFP(0.5, 50, 300, 0.1), Color.Orange);
            AllGraphs[0].Add("Pstat=0.4 bar", FillPairKFP(0.4, 50, 300, 0.1), Color.Blue);
            AllGraphs[0].Add("Pstat=0.3 bar", FillPairKFP(0.3, 50, 300, 0.1), Color.DarkGreen);
            AllGraphs[0].Add("Pstat=0.2 bar", FillPairKFP(0.2, 50, 300, 0.1), Color.DarkRed);
            AllGraphs[0].Add("Pstat=0.1 bar", FillPairKFP(0.1, 50, 300, 0.1), Color.HotPink);
            AllGraphs[0].Add("Pstat=0.07 bar", FillPairKFP(0.07, 50, 300, 0.1), Color.DarkOrchid);
            AllGraphs[0].Add("Pstat=0.0 bar", FillPairKFP(0, 50, 300, 0.1), Color.DarkMagenta);

            AllGraphs[1] = new PureLineCollection("Vent Size For Dusts(300≤Kst≤800)", 800, 300, "Kst", "0", 0.13, 0.03, "FactorA2", "0.00");
            AllGraphs[1].Add("Pstat=0.5 bar", FillPairKFP(0.5, 300, 800, 0.1), Color.Orange);
            AllGraphs[1].Add("Pstat=0.4 bar", FillPairKFP(0.4, 300, 800, 0.1), Color.Blue);
            AllGraphs[1].Add("Pstat=0.3 bar", FillPairKFP(0.3, 300, 800, 0.1), Color.DarkGreen);
            AllGraphs[1].Add("Pstat=0.2 bar", FillPairKFP(0.2, 300, 800, 0.1), Color.DarkRed);
            AllGraphs[1].Add("Pstat=0.1 bar", FillPairKFP(0.1, 300, 800, 0.1), Color.HotPink);
            AllGraphs[1].Add("Pstat=0.07 bar", FillPairKFP(0.07, 300, 800, 0.1), Color.DarkOrchid);
            AllGraphs[1].Add("Pstat=0.0 bar", FillPairKFP(0, 300, 800, 0.1), Color.DarkMagenta);

            AllGraphs[2] = new PureLineCollection("Venting Sizing for Dusts (0-10m^3 )", 10, 0, "Volume(m^3)", "0", 6, 0, "FactorB1", "0");
            AllGraphs[2].Add("", FillPairKFP(0, 10, 0.1), Color.DarkRed);

            AllGraphs[3] = new PureLineCollection("Venting Sizing for Dusts(10-100m^3)", 100, 10, "Volume(m^3)", "0", 32, 0, "FactorB2", "0");
            AllGraphs[3].Add("", FillPairKFP(10, 100, 0.1), Color.DarkRed);

            AllGraphs[4] = new PureLineCollection("Venting Sizing for Dusts(100-1000m^3)", 1000, 100, "Volume(m^3)", "0", 200, 0, "FactorB3", "0");
            AllGraphs[4].Add("", FillPairKFP(100, 1000, 0.1), Color.DarkRed);

            AllGraphs[5] = new PureLineCollection("Venting Sizing for Dusts(1000-10000m^3)", 10000, 1000, "Volume(m^3)", "0", 1000, 0, "FactorB4", "0");
            AllGraphs[5].Add("", FillPairKFP(1000, 10000, 0.1), Color.DarkRed);

            AllGraphs[6] = new PureLineCollection("Venting Sizing for Dusts(Π<0.05)", 0.05, 0, "Π=Pred/Pmax", "0.000", 14, 4, "FactorC1", "0");
            AllGraphs[6].Add("", FillPairC(0, 0.05, 0.0001), Color.DarkGreen);

            AllGraphs[7] = new PureLineCollection("Venting Sizing for Dusts(0.05≤Π<0.2)", 0.2, 0.05, "Π=Pred/Pmax", "0.000", 4.4, 2, "FactorC2", "0.0");
            AllGraphs[7].Add("", FillPairC(0.05, 0.2, 0.0001), Color.DarkGreen);

            AllGraphs[8] = new PureLineCollection("Venting Sizing for Dusts(Π≥0.2)", 1, 0.2, "Π=Pred/Pmax", "0.0", 2, 0, "FactorC3", "0.0");
            AllGraphs[8].Add("", FillPairC(0.2, 1, 0.0001), Color.DarkGreen);

            AllGraphs[9] = new PureLineCollection("Venting Sizing for Dusts—Option1.", 1, 0, "Pred,bar", "0.0", 2.8, 1, "FactorD1", "0.0");
            AllGraphs[9].Add("L/D=6", FillPairD1(6, 0.05, 1, 0.001), Color.Green);
            AllGraphs[9].Add("L/D=5", FillPairD1(5, 0.05, 1, 0.001), Color.Blue);
            AllGraphs[9].Add("L/D=4", FillPairD1(4, 0.05, 1, 0.001), Color.Black);
            AllGraphs[9].Add("L/D=3.5", FillPairD1(3.5, 0.05, 1, 0.001), Color.Orange);
            AllGraphs[9].Add("L/D=3", FillPairD1(3, 0.05, 1, 0.001), Color.Yellow);
            AllGraphs[9].Add("L/D=2.5", FillPairD1(2.5, 0.05, 1, 0.001), Color.Red);
            AllGraphs[9].Add("L/D=2.25", FillPairD1(2.25, 0.05, 1, 0.001), Color.Magenta);

            AllGraphs[10] = new PureLineCollection("Venting Sizing for Dusts—Option2.", 6, 2, "L/D", "0.0", 2.8, 1, "FactorD2", "0.0");
            AllGraphs[10].Add("Pred=0.1 bar", FillPairD2(0.1, 2, 6, 0.01), Color.Green);
            AllGraphs[10].Add("Pred=0.2 bar", FillPairD2(0.2, 2, 6, 0.01), Color.Blue);
            AllGraphs[10].Add("Pred=0.3 bar", FillPairD2(0.3, 2, 6, 0.01), Color.Black);
            AllGraphs[10].Add("Pred=0.5 bar", FillPairD2(0.5, 2, 6, 0.01), Color.Orange);
            AllGraphs[10].Add("Pred=0.75 bar", FillPairD2(0.75, 2, 6, 0.01), Color.Yellow);
            AllGraphs[10].Add("Pred=1 bar", FillPairD2(1, 2, 6, 0.01), Color.Red);
            //使用默认曲线集
            Select(0);
        }
        public ZedGraphControl ZgMe;
        public GraphPane GpMe;
        //刷新图版中显示的曲线集
        void RefreshPane()
        {
            //重新加载曲线
            GpMe.CurveList.Clear();
            GpMe.CurveList.AddRange(_SelectedLines.Lines);

            //调整画布布局
            GpMe.Title.Text = _SelectedLines.Title;

            GpMe.XAxis.Scale.Max = _SelectedLines.XMax;
            GpMe.XAxis.Scale.Min = _SelectedLines.XMin;
            GpMe.XAxis.Title.Text = _SelectedLines.XLabel;
            double XSpace = (_SelectedLines.XMax - _SelectedLines.XMin) / 20;
            GpMe.XAxis.Scale.MajorStep = XSpace * 4;
            GpMe.XAxis.Scale.MinorStep = XSpace * 2;
            GpMe.XAxis.Scale.Format = _SelectedLines.XFormat;

            GpMe.YAxis.Scale.Max = _SelectedLines.YMax;
            GpMe.YAxis.Scale.Min = _SelectedLines.YMin;
            GpMe.YAxis.Title.Text = _SelectedLines.YLabel;
            double YSpace = (_SelectedLines.YMax - _SelectedLines.YMin) / 20;
            GpMe.YAxis.Scale.MajorStep = YSpace * 4;
            GpMe.YAxis.Scale.MinorStep = YSpace * 2;
            GpMe.YAxis.Scale.Format = _SelectedLines.YFormat;

            //重绘图形
            ZgMe.AxisChange();
            ZgMe.Invalidate();
            ZgMe.Update();
        }
        /// <summary>
        /// 指明当前图版是否可以使用鼠标取值
        /// </summary>
        public bool Picking;
        /// <summary>
        /// 定义取值拓展函数的格式
        /// </summary>
        /// <param name="x">取到的x轴值</param>
        /// <param name="y">取到的y轴值</param>
        public delegate void PickValue(double x, double y);
        //空处理
        void Empty(double x, double y) { /* Do nothing */ }
        PickValue LaterProcess;
        void ZgMe_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Picking)
            {
                double x, y;
                GpMe.ReverseTransform(e.Location, out x, out y);
                LaterProcess(x, y);
            }
        }

        #region 曲线集集
        public void Select(int Index)
        {
            SelectedLines = AllGraphs[Index];
        }
        PureLineCollection _SelectedLines;
        public PureLineCollection SelectedLines
        {
            get { return _SelectedLines; }
            set
            {
                _SelectedLines = value;
                RefreshPane();
            }
        }
        PureLineCollection[] AllGraphs;
        public PureLineCollection FactorA_1 { get { return AllGraphs[0]; } }
        public PureLineCollection FactorA_2 { get { return AllGraphs[1]; } }
        public PureLineCollection FactorB_1 { get { return AllGraphs[2]; } }
        public PureLineCollection FactorB_2 { get { return AllGraphs[3]; } }
        public PureLineCollection FactorB_3 { get { return AllGraphs[4]; } }
        public PureLineCollection FactorB_4 { get { return AllGraphs[5]; } }
        public PureLineCollection FactorC_1 { get { return AllGraphs[6]; } }
        public PureLineCollection FactorC_2 { get { return AllGraphs[7]; } }
        public PureLineCollection FactorC_3 { get { return AllGraphs[8]; } }
        public PureLineCollection FactorD_1 { get { return AllGraphs[9]; } }
        public PureLineCollection FactorD_2 { get { return AllGraphs[10]; } }
        #endregion

        #region 公式：制作曲线点对表
        PointPairList FillPairKFP(double Pstat, double Start, double End, double Step)
        {
            PointPairList AimPairs = new PointPairList();
            for (double Kst = Start; Kst < End; Kst += Step)
            {
                double fA1 = 1e-4 * (1.0 + 1.54 * Math.Pow(Pstat, 4.0 / 3.0)) * Kst;
                AimPairs.Add(Kst, fA1);
            }
            return AimPairs;
        }
        PointPairList FillPairD1(double LDRatio, double Start, double End, double Step)
        {
            PointPairList AimPairs = new PointPairList();
            for (double Pred = Start; Pred < End; Pred += Step)
            {
                double D1 = 1 + 0.6 * Math.Pow(LDRatio - 2, 0.75) * Math.Exp(-0.95 * Pred * Pred);
                AimPairs.Add(Pred, D1);
            }
            return AimPairs;
        }
        PointPairList FillPairD2(double Pred, double Start, double End, double Step)
        {
            PointPairList AimPairs = new PointPairList();
            for (double LDRatio = Start; LDRatio < End; LDRatio += Step)
            {
                double D2 = 1 + 0.6 * Math.Pow(LDRatio - 2, 0.75) * Math.Exp(-0.95 * Pred * Pred);
                AimPairs.Add(LDRatio, D2);
            }
            return AimPairs;

        }
        PointPairList FillPairKFP(double Start, double End, double Step)
        {
            PointPairList AimPairs = new PointPairList();
            for (double V = Start; V < End; V += 0.1)
            {
                double Vy = Math.Pow(V, 0.75);
                AimPairs.Add(V, Vy);
            }
            return AimPairs;
        }
        PointPairList FillPairC(double Start, double End, double Step)
        {
            PointPairList AimPairs = new PointPairList();
            for (double Π = Start; Π < End; Π += Step)
            {
                double C = Math.Pow(1 / Π - 1, 0.5);
                AimPairs.Add(Π, C);
            }
            return AimPairs;
        }
        #endregion

        /// <summary>
        /// 使用提供的参数计算泄压面积
        /// </summary>
        /// <param name="A">参数a</param>
        /// <param name="B">参数b</param>
        /// <param name="C">参数c</param>
        /// <param name="D">参数d</param>
        /// <returns>符合NFPA68的泄压面积</returns>
        public double GetReliefArea(double A, double B, double C, double D)
        {
            return A * B * C * D;
        }
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class ReliefAreaGenerator : IReliefArea
    {
        public double ReliefArea(double Pmax, double Kst, double Pred, double Pstat, double V, double HDRatio)
        {
            Pmax *= 10;
            Kst *= 10;
            Pred *= 10;
            Pstat *= 10;
            double Av0 = 0;
            Av0 = 1e-4 * (1 + 1.54 * Math.Pow(Pstat, 0.75)) * Kst * Math.Pow(V, 0.75) * Math.Pow(Pmax / Pred - 1, 0.5);
            double AV1 = 0;
            if (HDRatio <= 2)
                AV1 = Av0;
            else
                AV1 = Av0 * (1 + 0.6 * Math.Pow(HDRatio - 2, 0.75) * Math.Exp(-0.95 * Pred * Pred));
            return AV1;
        }

        public double ReliefAreaOfSoli(double Pmax, double Kst, double Pred, double Pstat, double V, double H, double Df, double HDRatio, FeedingWay Feeding)
        {
            return ReliefArea(Pmax, Kst, Pred, Pstat, V, HDRatio);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*提供抽象基类、通用类型及接口*/

namespace IEPI.EPE.VentDesign
{
    #region 通用类型

    /// <summary>
    /// 代表一个目标容器，以反映在计算时需要的相关参数
    /// <para>此类作为基类，对于不同的计算标准，应构建相应子类并提供计算方法</para>
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public abstract class ContainerBase
    {
        /// <summary>
        /// 有效长径比
        /// </summary>
        public double LDR
        {
            get
            {
                if (_LDR == -1) throw new BadParamException(StdStrs.LDR, ParamExceptionType.TBD);
                return _LDR;
            }
            set
            {
                _LDR = value;
            }
        }
        protected double _LDR = -1;

        /// <summary>
        /// 容积，m³
        /// </summary>
        public double V
        {
            get { if (_V == -1) throw new BadParamException(StdStrs.V, ParamExceptionType.TBD); return _V; }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.V, ParamExceptionType.IL);
                _V = value; }
        }
        protected double _V = -1;
        /// <summary>
        /// 容器高度，m
        /// </summary>
        public double L
        {
            get
            {
                if (_L == -1) throw new BadParamException(StdStrs.L, ParamExceptionType.TBD);
                return _L;
            }
            set 
            {
                if (value <= 0) throw new BadParamException(StdStrs.L, ParamExceptionType.IL);
                _L = value; }
        }
        protected double _L = -1;
        /// <summary>
        /// 参考圆面积，m²
        /// </summary>
        public double Ax
        { get { return V / L; } }
        /// <summary>
        /// 当量直径，m
        /// </summary>
        public double D_E
        { get { return 2 * Math.Sqrt(Ax / Math.PI); } }

        /// <summary>
        /// 容器设计强度，MPa
        /// </summary>
        public double P
        {
            get { if (_P == -1)throw new BadParamException(StdStrs.P, ParamExceptionType.TBD); return _P; }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.P, ParamExceptionType.IL);
                _P = value; }
        }
        protected double _P = -1;
        /// <summary>
        /// 操作压力，MPa
        /// </summary>
        public double P_op
        {
            get
            {
                if (_P_op == -1) throw new BadParamException(StdStrs.P_op, ParamExceptionType.TBD); return _P_op;
            }
            set {
                //if (value <= 0) throw new BadParamException(StdStrs.P_op, ParamExceptionType.IL);
                _P_op = value; }
        }
        protected double _P_op = -1;
        /// <summary>
        /// 静开启压力，MPa
        /// </summary>
        public double P_stat
        {
            get { if (_P_stat == -1)throw new BadParamException(StdStrs.P_stat, ParamExceptionType.TBD); return _P_stat; }
            set { if (value <= 0) throw new BadParamException(StdStrs.P_stat, ParamExceptionType.IL); _P_stat = value; }
        }
        protected double _P_stat = -1;
        /// <summary>
        /// 动开启压力，MPa
        /// </summary>
        public double P_dyn
        {
            get { if (_P_dyn == -1)throw new BadParamException(StdStrs.P_dyn, ParamExceptionType.TBD); return _P_dyn; }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.P_dyn, ParamExceptionType.IL);
                _P_dyn = value; }
        }
        protected double _P_dyn = -1;
        /// <summary>
        /// 最大泄爆压力，MPa
        /// </summary>
        public double P_red_max
        {
            get
            {
                if (_P_red_max == -1) throw new BadParamException(StdStrs.P_red_max, ParamExceptionType.TBD);
                return _P_red_max;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.P_red_max, ParamExceptionType.IL);
                _P_red_max = value; }
        }
        protected double _P_red_max = -1;
        /// <summary>
        /// 泄压效率
        /// </summary>
        public double E_F
        {
            get { return _E_F; }
            set
            {
                if (value < 0 || value >1) throw new BadParamException(StdStrs.E_F, ParamExceptionType.IL);
                _E_F = value;
            }
        }
        protected double _E_F = 1;
        /// <summary>
        /// 泄压面积，m²
        /// </summary>
        public double A { get { return _A; } }
        protected double _A;
        /// <summary>
        /// 有效泄压面积，m²
        /// </summary>
        public double A_w
        { get { return A * E_F; } }
        /// <summary>
        /// 内容粉尘
        /// </summary>
        public Dust Content = new Dust();

        #region 进料容器相关参数

        /// <summary>
        /// 进料方式
        /// </summary>
        public FeedingWay Feed = FeedingWay.None;
        /// <summary>
        /// 气力输送管径，m
        /// </summary>
        public double D_F
        {
            get
            {
                if (_D_F == -1) throw new BadParamException(StdStrs.D_F, ParamExceptionType.TBD); return _D_F;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.D_F, ParamExceptionType.IL);
                _D_F = value; }
        }
        protected double _D_F = -1;
        /// <summary>
        /// 空气流量，m³/h
        /// </summary>
        public double Q
        {
            get
            {
                if (_Q == -1) throw new BadParamException(StdStrs.Q, ParamExceptionType.TBD); return _Q;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.Q, ParamExceptionType.IL);
                _Q = value; }
        }
        protected double _Q = -1;
        /// <summary>
        /// 空气输送速度，m/s
        /// </summary>
        public double u_L
        {
            get
            {
                if (_u_L == -1) throw new BadParamException(StdStrs.u_L, ParamExceptionType.TBD); return _u_L;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.u_L, ParamExceptionType.IL);
                _u_L = value; }
        }
        protected double _u_L = -1;

        #endregion
    }

    /// <summary>
    /// 代表某种粉尘，以反映此粉尘的爆炸相关参数
    /// </summary>
    public class Dust
    {
        /// <summary>
        /// 粉尘名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        string _Name = null;
        /// <summary>
        /// 爆炸指数：最大爆炸压力
        /// </summary>
        public double P_max
        {
            get
            {
                if (_P_max == -1) throw new BadParamException(StdStrs.P_max, ParamExceptionType.TBD);
                return _P_max;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.P_max, ParamExceptionType.IL);
                _P_max = value; }
        }
        double _P_max = -1;
        /// <summary>
        /// 爆炸指数：压力上升速率指数
        /// </summary>
        public double K_st
        {
            get
            {
                if (_K_st == -1) throw new BadParamException(StdStrs.K_st, ParamExceptionType.TBD);
                return _K_st;
            }
            set {
                if (value <= 0) throw new BadParamException(StdStrs.K_st, ParamExceptionType.IL);
                _K_st = value; }
        }
        double _K_st = -1;
        /// <summary>
        /// 爆炸等级
        /// </summary>
        public ExplosionLevel Explos = ExplosionLevel.Unknown;
    }

    #endregion

    #region 泄压面积计算支持

    /// <summary>
    /// 规范各标准通用的泄压面积计算的专用接口
    /// </summary>
    [Obsolete("版本2.2之前的接口已停止维护")]
    public interface IReliefArea
    {
        /// <summary>
        /// 计算一般容器泄压面积
        /// </summary>
        /// <param name="Pmax"></param>
        /// <param name="Kst"></param>
        /// <param name="Pred"></param>
        /// <param name="Pstat"></param>
        /// <param name="V"></param>
        /// <param name="HDRatio"></param>
        /// <returns></returns>
        double ReliefArea(double Pmax, double Kst, double Pred, double Pstat, double V, double HDRatio);
        /// <summary>
        /// 计算料仓泄压面积
        /// </summary>
        /// <param name="Pmax"></param>
        /// <param name="Kst"></param>
        /// <param name="Pred"></param>
        /// <param name="Pstat"></param>
        /// <param name="V"></param>
        /// <param name="H"></param>
        /// <param name="Df"></param>
        /// <param name="HDRatio"></param>
        /// <param name="Feeding"></param>
        /// <returns></returns>
        double ReliefAreaOfSoli(double Pmax, double Kst, double Pred, double Pstat, double V, double H, double Df, double HDRatio, FeedingWay Feeding);
    }

    #endregion

    #region 长径比计算支持

    [Obsolete("版本2.2之前的接口已停止维护")]
    public abstract class LDRBase
    {
        public ParamCollection<double> Parameters;

        public void DefineParams(params string[] Names)
        {
            if (Parameters == null) Parameters = new ParamCollection<double>();
            foreach (string Name in Names)
                Parameters.Add(Name, 0);
        }

        public void SetParam(string Name, double Value)
        {
            Parameters.SetParam(Name, Value);
        }
        public double GetParam(string Name)
        {
            return Parameters.GetParam(Name).Value;
        }
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public interface ILDR
    { double GetHDR(); }

    #endregion
}

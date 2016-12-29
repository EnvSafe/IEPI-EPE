using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEPI.EPE.VentDesign.GB.No15605_2008
{
    [Obsolete("版本2.2之前的接口已停止维护")]
    public sealed class RA_Container : ContainerBase
    {
        /// <summary>
        /// 直接设置有效长径比
        /// <para>注意：此方法将不检查设置的值是否合法，因此无法保证程序的运行结果</para>
        /// <para>调用此方法时应确定输入的值确实合法，应避免传入由用户直接输入的值</para>
        /// </summary>
        /// <param name="LDR">长径比值</param>
        public void GenerateLDR(double LDR)
        { _LDR = LDR; }

        /// <summary>
        /// 计算有效长径比，保存在LDR属性中
        /// </summary>
        /// <param name="Veff">有效火焰传播体积，m³</param>
        /// <param name="Leff">有效火焰传播距离，m</param>
        public double GenerateLDR(double Veff, double Leff)
        {
            double Aeff = Veff / Leff;
            double Deff = 2 * Math.Sqrt(Aeff / Math.PI);
            _LDR = Leff / Deff;
            return _LDR;
        }

        #region 计算泄爆面积

        /// <summary>
        /// 计算出泄爆面积，保存在A属性中。
        /// <para>实际应用时应考虑到泄压效率，所以推荐使用A_w而非A</para>
        /// </summary>
        /// <param name="Check">是否校验参数</param>
        /// <param name="AutoCorrect">自动取舍，在允许范围内修改计算参数使计算得以进行，参见标准</param>
        /// <returns>操作结果</returns>
        public CheckInfo GenerateReliefArea(bool Check, bool AutoCorrect)
        {
            CheckInfo ResultInfo;
            try
            {
                if (Check) ResultInfo = Check_All();
                else ResultInfo = new CheckInfo(true, "未校验");
                if (ResultInfo.Status)
                {
                    double Dz, Y, X;
                    double Pstat, Pred, Pmax, Kst;
                    switch (Feed)
                    {
                        case FeedingWay.None:
                            if (Check) ResultInfo = Check_None(AutoCorrect, out Pstat, out Pred, out Pmax);
                            else
                            {
                                Pmax = Content.P_max; Pred = P_red_max; Pstat = P_stat;
                            }
                            if (ResultInfo.Status)
                            {
                                double B1 = 0.0008805* Pmax * Content.K_st * Math.Pow(Pred, -0.569);
                                double B2 = 0.854 * (Pstat - 0.01) * Math.Pow(Pred, -0.5);
                                double B = (B1 + B2) * Math.Pow(V, 0.753);
                                if (Pred < 0.15)
                                {
                                    double C = -4.305 * Math.Log10(Pred) - 3.547;
                                    _A = B * (1.0 + C * Math.Log10(LDR));
                                }
                                else
                                { _A = B; }
                            }
                            break;
                        case FeedingWay.PneumaticAxial:
                            if (Check) ResultInfo = Check_Axis(AutoCorrect, out Pred);
                            else
                            {
                                Pred = P_red_max;
                            }
                            if (ResultInfo.Status)
                            {
                                Dz = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                                Y = 0.0575 * Math.Pow(Pred, -1.27);
                                X = (8.6 * Math.Log10(Pred) + 2.6) / Dz - 5.5 * Math.Log10(Pred) - 1.8;
                                X *= 0.11 * Content.K_st * D_F;
                                if (L > 10.0)
                                    _A = 0.1 * L * X * (1.0 + Y * Math.Log10(LDR));
                                else
                                    _A = X * (1.0 + Y * Math.Log10(LDR));
                            }
                            break;
                        case FeedingWay.PneumaticTangential:
                            if (Check) ResultInfo = Check_Tang(AutoCorrect, out Kst);
                            else
                            {
                                Kst = Content.K_st;
                            }
                            if (ResultInfo.Status)
                            {
                                double k;
                                if (P_red_max >= 0.01 && P_red_max <= 0.1)
                                    k = 1;
                                else //if (P_red_max > 0.1 && P_red_max <= 0.17)
                                    k = 2;
                                Dz = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                                Y = 0.166 * Math.Pow(Math.E, Kst / 12.9) * Math.Pow(P_red_max * 10.0, -1.27 / k);
                                X = (8.6 * (1.0 + Math.Log10(P_red_max)) / k - Kst / 4.4 - 0.513) / Dz
                                   - 5.5 * (1.0 + Math.Log10(P_red_max)) / k + Kst / 6.9 + 0.191;
                                X *= 0.11 * Kst * D_F;
                                _A = X * (1.0 + Y * Math.Log10(LDR));
                            }
                            break;
                        case FeedingWay.FreeFalling:
                            //
                            goto case FeedingWay.PneumaticAxial;
                    }
                }
            }
            catch(BadParamException bex)
            {
                string des;
                switch (bex.Type)
                {
                    case ParamExceptionType.NA:
                        des = string.Format("{0}:OUTRANGE", bex.Message);
                        break;
                    case ParamExceptionType.TBD:
                        des = string.Format("{0}:NA", bex.Message);
                        break;
                    default:
                    case ParamExceptionType.IL:
                        des = string.Format("{0}:ILLEGAL", bex.Message);
                        break;
                }
                ResultInfo = new CheckInfo(false, des);
            }
            catch (Exception ex) { ResultInfo = new CheckInfo(false, ex.Message); }
            return ResultInfo;
        }

        /// <summary>
        /// 检查容器计算公式可用性
        /// </summary>
        /// <returns>可用性信息</returns>
        public CheckInfo Check_All()
        {
            bool Status = true;
            string Description = string.Empty;
            if (P_op > 0.02)
            {
                Status = false;
                Description = string.Format("{0}:>0.02", StdStrs.P_op);
                return new CheckInfo(Status, Description);
            }
            switch (Content.Explos)
            {
                case ExplosionLevel.St1:
                    if (Content.P_max > 1)
                    {
                        Status = false;
                        Description = string.Format("{0}:>1", StdStrs.P_max);
                    }
                    break;
                case ExplosionLevel.St2:
                    if (Content.P_max > 1)
                    {
                        Status = false;
                        Description = string.Format("{0}:>1", StdStrs.P_max);
                    }
                    break;
                case ExplosionLevel.St3:
                    if (Content.P_max > 1.2)
                    {
                        Status = false;
                        Description = string.Format("{0}:>1.2", StdStrs.P_max);
                    }
                    break;
                case ExplosionLevel.St4:
                    Status = false;
                    Description = string.Format("{0}:St4", StdStrs.Explos);
                    break;
                default: 
                    return new CheckInfo(string.Format("{0}:-", StdStrs.Explos), ParamExceptionType.TBD);
            }
            return new CheckInfo(Status, Description);
        }

        /// <summary>
        /// 检查标准情形计算公式可用性
        /// </summary>
        /// <param name="AutoCorrect">指示是否自动调整可重取的值</param>
        /// <param name="Temp_P_stat">重取的静开启压力值，当选择AutoCorrect并返回true时，应使用此值代替P_stat</param>
        /// <param name="Temp_P_red_max">重取的最大泄爆压力值，当选择AutoCorrect并返回true时，应使用此值代替P_red_max</param>
        /// <param name="Temp_P_max">重取的粉尘爆炸指数，当选择AutoCorrect并返回true时，应使用此值代替Content.P_max</param>
        /// <returns>可用性信息</returns>
        public CheckInfo Check_None(bool AutoCorrect, out double Temp_P_stat,
            out double Temp_P_red_max, out double Temp_P_max)
        {
            Temp_P_max = Content.P_max;
            Temp_P_red_max = P_red_max;
            Temp_P_stat = P_stat;

            StringBuilder sb = new StringBuilder();

            //容器容积
            if (V < 0.1 || V > 10000) 
                return new CheckInfo(false, string.Format("{0}: <0.1 >10000", StdStrs.V));
            //静开启压力
            if (P_stat < 0.01 || P_stat > 0.1)
            {
                if (AutoCorrect) { Temp_P_stat = 0.01; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.P_stat, 0.01)); }
                else return new CheckInfo(false, string.Format("{0}:<0.01 >0.1", StdStrs.P_stat));
            }
            //最大泄爆压力
            if (P_red_max < 0.01 || P_red_max > 0.2)
            {
                if (AutoCorrect) { Temp_P_red_max = 0.01; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.P_red_max, 0.01)); }
                else return new CheckInfo(false, string.Format("{0}:<0.01 >0.2", StdStrs.P_red_max));
            }
            if (P_red_max <= P_stat)
                return new CheckInfo(false, string.Format("{0}: <={1}", StdStrs.P_red_max, StdStrs.P_stat));
            //长径比
            if (LDR > 20)
                return new CheckInfo(false, string.Format("{0}: >20", StdStrs.LDR));
            //粉尘爆炸指数
            if (Content.K_st >= 1 && Content.K_st <= 30)
            {
                if (Content.P_max < 0.5 || Content.P_max > 1)
                {
                    if (AutoCorrect) { Temp_P_max = 0.5; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.P_max, 0.5)); }
                    else return new CheckInfo(false, string.Format("{0}: <0.5 >1", StdStrs.P_max));
                }
            }
            if (Content.K_st > 30 && Content.K_st <= 80)
            {
                if (Content.P_max < 0.5 || Content.P_max > 1.2)
                {
                    if (AutoCorrect) { Temp_P_max = 0.5; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.P_max, 0.5)); }
                    else return new CheckInfo(false, string.Format("{0}: <0.5 >1.2", StdStrs.P_max));
                }
            }

            return new CheckInfo(true, sb.ToString());
        }

        /// <summary>
        /// 检查轴向中心进料容器计算公式的可用性
        /// </summary>
        /// <param name="AutoCorrect">指示是否自动调整可重取的值</param>
        /// <param name="Temp_P_red_max">重取的最大泄爆压力值，当选择AutoCorrect并返回true时，应使用此值代替P_red_max</param>
        /// <returns>可用性信息</returns>
        public CheckInfo Check_Axis(bool AutoCorrect, out double Temp_P_red_max)
        {
            Temp_P_red_max = P_red_max;
            StringBuilder sb = new StringBuilder();

            if (V < 5 || V > 10000) return new CheckInfo(false, string.Format("{0}:<5 >10000", StdStrs.V));
            if (u_L > 40) return new CheckInfo(false, string.Format("{0}:>40", StdStrs.u_L));
            if (Q > 2500)
            {
                if (AutoCorrect && P >= 0.025 && Q <= 5000)
                { Temp_P_red_max = 0.01; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.P_red_max, 0.01)); }
                else return new CheckInfo(false, string.Format("{0}:>2500", StdStrs.Q));
            }
            if (D_F > 0.3) return new CheckInfo(false, string.Format("{0}:>0.3", StdStrs.D_F));
            if (P_stat > 0.01) return new CheckInfo(false, string.Format("{0}:>0.01", StdStrs.P_stat));
            if (P_red_max <= 0.01 || P_red_max > 0.2 || P_stat > P_red_max) return new CheckInfo(false, string.Format("{0}:<=0.01 >0.2 <={1}", StdStrs.P_red_max, StdStrs.P_stat));
            if (Content.P_max > 0.9) return new CheckInfo(false, string.Format("{0}: >0.9", StdStrs.P_max));
            if (Content.K_st < 5 || Content.K_st > 30) return new CheckInfo(false, string.Format("{0}:<5 >30", StdStrs.K_st));

            return new CheckInfo(true, sb.ToString());
        }

        /// <summary>
        /// 检查切向进料容器计算公式的可用性
        /// </summary>
        /// <param name="AutoCorrect">指示是否自动调整可重取的值</param>
        /// <param name="Temp_K_st">重取的爆炸指数：压力上升指数，当选择AutoCorrect并返回true时，应使用此值代替Content.K_st</param>
        /// <returns>可用性信息</returns>
        public CheckInfo Check_Tang(bool AutoCorrect, out double Temp_K_st)
        {
            Temp_K_st = Content.K_st;
            StringBuilder sb = new StringBuilder();

            if (D_F > 0.2) return new CheckInfo(false, string.Format("{0}:>0.2",StdStrs.D_F));
            if (V < 6 || V > 120) return new CheckInfo(false, string.Format("{0}:<6 >120", StdStrs.V));
            if (LDR < 1 || LDR > 5) return new CheckInfo(false, string.Format("{0}:<1 >5", StdStrs.LDR));
            if (u_L > 30) return new CheckInfo(false, string.Format("{0}:>30", StdStrs.u_L));
            if (Q > 2500) return new CheckInfo(false, string.Format("{0}:>2500", StdStrs.Q));
            if (P_stat > 0.01) return new CheckInfo(false, string.Format("{0}:>0.01", StdStrs.P_stat));
            if (P_red_max <= 0.01 || P_red_max > 0.17) return new CheckInfo(false, string.Format("{0}:<=0.01 >0.17", StdStrs.P_red_max));
            if (Content.P_max > 0.9) return new CheckInfo(false, string.Format("{0}:>0.9", StdStrs.P_max));
            if (Content.K_st > 22) return new CheckInfo(false, string.Format("{0}:>22", StdStrs.K_st));
            if (Content.K_st < 10)
            {
                if (AutoCorrect) { Temp_K_st = 10; sb.AppendLine(string.Format("{0} 纠正为 {1}", StdStrs.K_st, 10)); }
                else return new CheckInfo(false, string.Format("{0}:<10", StdStrs.K_st));
            }

            return new CheckInfo(true, sb.ToString());
        }

        #endregion
    }

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class ReliefAreaGenerator : IReliefArea
    {
        public double ReliefArea(double Pmax, double Kst, double Pred, double Pstat, double V, double HDRatio)
        {
            if (Pstat < 0.01)
                Pstat = 0.01;
            double num1 = (0.0008805 * Pmax * Kst * Math.Pow(Pred, -0.569) + 0.854 * (Pstat - 0.01) * Math.Pow(Pred, -0.5)) * Math.Pow(V, 0.753);
            double num2;
            if (Pred < 0.15)
            {
                double num3 = -4.305 * Math.Log10(Pred) - 3.547;
                num2 = num1 * (1.0 + num3 * Math.Log10(HDRatio));
            }
            else
                num2 = num1;
            return num2;
        }

        public double ReliefAreaOfSoli(double Pmax, double Kst, double Pred, double Pstat, double V, double H, double Df, double HDRatio, FeedingWay Feeding)
        {
            switch (Feeding)
            {
                case FeedingWay.PneumaticAxial:
                    double num1 = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                    double num2 = 23.0 / 400.0 * Math.Pow(Pred, -1.27);
                    double num3 = ((8.6 * Math.Log10(Pred) + 2.6) / num1 - 5.5 * Math.Log10(Pred) - 1.8) * (0.11 * Kst * Df);
                    return H <= 10.0 ? num3 * (1.0 + num2 * Math.Log10(HDRatio)) : 0.1 * H * num3 * (1.0 + num2 * Math.Log10(HDRatio));
                case FeedingWay.PneumaticTangential:
                    double num4;
                    if (Pred >= 0.01 && Pred <= 0.1)
                    {
                        num4 = 1.0;
                    }
                    else
                    {
                        if (Pred <= 0.1 || Pred > 0.17)
                            throw new Exception("容器设计强度Pred不在有效范围内");
                        num4 = 2.0;
                    }
                    double num5 = Math.Pow(V * 4.0 / Math.PI, 1.0 / 3.0);
                    double num6 = 0.166 * Math.Pow(Math.E, Kst / 12.9) * Math.Pow(Pred * 10.0, -1.27 / num4);
                    return ((8.6 * (1.0 + Math.Log10(Pred)) / num4 - Kst / 4.4 - 0.513) / num5 - 5.5 * (1.0 + Math.Log10(Pred)) / num4 + Kst / 6.9 + 0.191) * (0.11 * Kst * Df) * (1.0 + num6 * Math.Log10(HDRatio));
                default:
                    throw new Exception("未实现的进料方式！");
            }
        }
    }
}

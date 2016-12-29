using Heroius.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IEPI.EPE.Vent.GB
{
    /// <summary>
    /// GB/T 15605-2008 计算上下文
    /// </summary>
    public class Context15605_2008 : Vent.Context
    {
        public Context15605_2008()
        {
        }

        /// <summary>
        /// 设计目标类型
        /// </summary>
        public TargetObject ObjectType
        {
            get
            {
                if (ObjectEntity is ContainerEntity)
                {
                    return TargetObject.Container;
                }
                else if (ObjectEntity is PipeEntity)
                {
                    return TargetObject.Pipe;
                }
                else if (ObjectEntity is BuildingEntity)
                {
                    return TargetObject.Building;
                }
                else return TargetObject.System;
            }
        }

        /// <summary>
        /// 当前计算参数实体
        /// </summary>
        public Entity15605_2008 ObjectEntity { get; set; }

        /// <summary>
        /// 对当前参数执行标准适用性验证
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        PredicateResult Predicate_StandardFit(Context c)
        {
            var context = c as Context15605_2008;
            var entity = context.ObjectEntity as Entity15605_2008;
            if (entity.pstat >= entity.p) return new PredicateResult(101);
            if (entity.p < entity.predmax) return new PredicateResult(102);

            return PredicateResult.Succeed;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <returns></returns>
        public CalcResult CalculateVentArea()
        {
            CalcResult r = new CalcResult(1);
            var prer = Predicate_StandardFit(this);
            if (prer.IsSucceed)
            {
                Utility.DoTransform(prer.Transforms);
                var c = ObjectEntity.Calculate();
                Utility.UndoTransform(prer.Transforms);
                r.MergeRemarks(c.Remarks);
                r.Succeed = c.Succeed;
                if (c.Succeed)
                {
                    r.MergeRemarks(prer.Transforms.Select(item => $"已转换: {item.Description}"));
                    r.CopyResultFrom(c);
                }
            }
            else
            {
                r.Succeed = false;
                r.Remarks.Add(prer.Description);
            }
            return r;
        }
    }

    /// <summary>
    /// 目标计算对象
    /// </summary>#
    public enum TargetObject
    {
        /// <summary>
        /// 容器、筒仓、设备
        /// </summary>
        Container,
        /// <summary>
        /// 管道
        /// </summary>
        Pipe,
        /// <summary>
        /// 建筑
        /// </summary>
        Building,
        /// <summary>
        /// 容器-管道混合系统
        /// </summary>
        System
    }

    public static class TargetObjectExtension
    {
        /// <summary>
        /// 从目标计算类型生成相应数据实体
        /// </summary>
        /// <param name="objtype"></param>
        /// <returns></returns>
        public static Entity15605_2008 GenEntity(this TargetObject objtype)
        {
            switch (objtype)
            {
                default:
                case TargetObject.Container:
                    return new ContainerEntity();
                case TargetObject.Pipe:
                    return new PipeEntity();
                case TargetObject.Building:
                    return new BuildingEntity();
                case TargetObject.System:
                    return new SystemEntity();
            }
        }
    }

    #region Object Entities

    /// <summary>
    /// 公共计算实体（目标）
    /// </summary>
    public abstract class Entity15605_2008 : Entity, INotifyAllPropertiesChanged
    {
        #region Properties

        /// <summary>
        /// 设计压力, MPa
        /// </summary>
        public double p { get; set; }

        /// <summary>
        /// 泄爆装置 静开启压力, MPa
        /// </summary>
        public double pstat { get; set; }
        /// <summary>
        /// 泄爆装置 动开启压力, MPa
        /// </summary>
        public double pdyn { get; set; } //never used

        /// <summary>
        /// 粉尘 最大爆炸压力, MPa
        /// </summary>
        public double pmax { get; set; }
        /// <summary>
        /// 粉尘 最大升压指数, MPa.m/s
        /// </summary>
        public double kst { get; set; }
        /// <summary>
        /// 获取 粉尘 爆炸等级
        /// </summary>
        public VentDesign.ExplosionLevel St { get { return Utility.GetStByKmax(kst); } }

        /// <summary>
        /// 容器-粉尘 最大泄爆压力, MPa
        /// </summary>
        public double predmax { get; set; }

        #endregion

        /// <summary>
        /// 必须由实体类实现的执行泄压面积计算的方法
        /// </summary>
        /// <returns></returns>
        public abstract CalcResult Calculate();

        public string GenXml()
        {
            return $"<Common><p>{p}</p><pstat>{pstat}</pstat><pdyn>{pdyn}</pdyn><pmax>{pmax}</pmax><kst>{kst}</kst><predmax>{predmax}</predmax></Common>";
        }

        public void ReadXml(XElement element)
        {
            p = element.Element("p").Value.As<double>();
            pstat = element.Element("pstat").Value.As<double>();
            pdyn = element.Element("pdyn").Value.As<double>();
            pmax = element.Element("pmax").Value.As<double>();
            kst = element.Element("kst").Value.As<double>();
            predmax = element.Element("predmax").Value.As<double>();
        }

        public void RaiseAllPropertiesChangedEvent()
        {
            RaisePropertyChangedEvent("p");
            RaisePropertyChangedEvent("pstat");
            RaisePropertyChangedEvent("pdyn");
            RaisePropertyChangedEvent("pmax");
            RaisePropertyChangedEvent("kst");
            RaisePropertyChangedEvent("St");
            RaisePropertyChangedEvent("predmax");
        }
    }

    public class ContainerEntity : Entity15605_2008
    {
        /// <summary>
        /// 有效容积, m^3
        /// <para>容器中参与爆炸的体积</para>
        /// <para>对于除尘器，是脏室容积去掉布袋等障碍物体积</para>
        /// </summary>
        public double V { get; set; }
        /// <summary>
        /// 操作压力, MPa
        /// </summary>
        public double poperation { get; set; }
        /// <summary>
        /// 长径比
        /// <para>有效长径比</para>
        /// </summary>
        public double L2DE { get; set; }

        /// <summary>
        /// 正常算法的验证
        /// </summary>
        /// <returns></returns>
        PredicateResult Predicate_Normal()
        {
            //第5.2节
            if (poperation > 0.02)
            {
                return new PredicateResult(107);
            }
            switch (St)
            {
                case VentDesign.ExplosionLevel.St1:
                case VentDesign.ExplosionLevel.St2:
                    {
                        if (pmax > 1)
                        {
                            return new PredicateResult(108);
                        }
                    }
                    break;
                case VentDesign.ExplosionLevel.St3:
                    {
                        if (pmax > 1.2)
                        {
                            return new PredicateResult(110);
                        }
                    }
                    break;
                case VentDesign.ExplosionLevel.Unknown:
                case VentDesign.ExplosionLevel.St4:
                default:
                    return new PredicateResult(109);
            }

            var r = PredicateResult.Succeed;

            // 常规公式的验证
            if (V < 0.1 || V > 10000)
            {
                return new PredicateResult(111);
            }
            if (predmax < 0.01 || predmax > 0.2)
            {
                return new PredicateResult(112);
            }
            //pstat
            if (pstat > 0.1)
            {
                return new PredicateResult(113);
            }
            if (pstat < 0.01)
            {
                r.AddTransform(2, (t) => { t.Cache = pstat; pstat = 0.01; }, (t) => { pstat = t.Cache.As<double>(); });
            }
            //kst pmax
            if (kst < 1)
            {
                r.AddTransform(3, (t) => { t.Cache = kst; kst = 1; }, (t) => { kst = t.Cache.As<double>(); });
            }
            if (kst <= 30) //对于kst过小转换的情况，继续进入此段判断
            {
                if (pmax < 0.5)
                {
                    r.AddTransform(4, (t) => { t.Cache = pmax; pmax = 0.5; }, (t) => { pmax = t.Cache.As<double>(); });
                }
                else if (pmax > 1)
                {
                    return new PredicateResult(115);
                }
            }
            else if (kst <= 80)
            {
                if (pmax < 0.5)
                {
                    r.AddTransform(4,
                        (t) => { t.Cache = pmax; pmax = 0.5; },
                        (t) => { pmax = t.Cache.As<double>(); });
                }
                else if (pmax > 1.2)
                {
                    return new PredicateResult(115);
                }
            }
            else
            {
                return new PredicateResult(114);
            }
            //长径比
            if (L2DE > 20)
            {
                return new PredicateResult(116);
            }
            return r;
        }

        /// <summary>
        /// 使用常规方法计算（式4 5），不包含验证
        /// </summary>
        /// <returns></returns>
        CalcResult Calculate_Normal()
        {
            CalcResult r = new CalcResult(1);
            r.Succeed = true;

            var B = (0.0008805 * pmax * kst * Math.Pow(predmax, -0.569) + 0.854 * (pstat - 0.01) * Math.Pow(predmax, -0.5)) * Math.Pow(V, 0.753);

            if (predmax < 0.15)
            {
                var C = -4.305 * Math.Log10(predmax) - 3.547;
                r.Results[0] = B * (1 + C * Math.Log10(L2DE));
            }
            else
            {
                r.Results[0] = B;
            }

            return r;
        }

        #region 料仓

        /// <summary>
        /// 容器高度, m
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 容器进料管管径, m
        /// </summary>
        public double Df { get; set; }
        /// <summary>
        /// 空气输送速度, m/s
        /// </summary>
        public double uL { get; set; }
        /// <summary>
        /// 空气流量, m^3/h
        /// </summary>
        public double Q { get; set; }

        /// <summary>
        /// 料仓的进料方式
        /// </summary>
        public VentDesign.FeedingWay Feed { get; set; }

        /// <summary>
        /// 轴向中心进料
        /// </summary>
        /// <returns></returns>
        PredicateResult Predicate_AxisFeed()
        {
            var r = PredicateResult.Succeed;
            if (V < 5 || V > 10000)
            {
                return new PredicateResult(117);
            }
            if (uL > 40)
            {
                return new PredicateResult(118);
            }
            if (Q > 2500)
            {
                if (p > 0.025 && Q <= 5000)
                {
                    r.AddTransform(5, (t) => { t.Cache = predmax; predmax = 0.01; }, (t) => { predmax = t.Cache.As<double>(); });
                }
                else return new PredicateResult(119);
            }
            if (Df > 0.3)
            {
                return new PredicateResult(120);
            }
            if (pstat > 0.01)
            {
                return new PredicateResult(121);
            }
            if (predmax <= 0.01 || predmax > 0.2)
            {
                return new PredicateResult(122);
            }
            if (pmax > 0.9)
            {
                return new PredicateResult(123);
            }
            if (kst < 5 || kst > 30)
            {
                return new PredicateResult(124);
            }

            return r;
        }
        /// <summary>
        /// 轴向中心进料
        /// </summary>
        /// <returns></returns>
        CalcResult Calculate_AxisFeed()
        {
            CalcResult r = new CalcResult(1);
            r.Succeed = true;

            double Dz = Math.Pow(4 * V / Math.PI, 1 / 3);
            double X = ((8.6 * Math.Log10(predmax) + 2.6) / Dz - 5.5 * Math.Log10(predmax) - 1.8) * 0.11 * kst * Df;
            double Y = 0.0575 * Math.Pow(predmax, -1.27);
            if (Height <= 10)
            {
                r.Results[0] = X * (1 + Y * Math.Log10(L2DE));
            }
            else
            {
                r.Results[0] = 0.1 * Height * X * (1 + Y * Math.Log10(L2DE));
            }
            return r;
        }

        /// <summary>
        /// 切向进料
        /// </summary>
        /// <returns></returns>
        PredicateResult Predicate_TanFeed()
        {
            var r = PredicateResult.Succeed;

            if (Df > 0.2)
            {
                return new PredicateResult(125);
            }
            if (V < 6 || V > 120)
            {
                return new PredicateResult(126);
            }
            if (L2DE < 1 || L2DE > 5)
            {
                return new PredicateResult(127);
            }
            if (uL > 30)
            {
                return new PredicateResult(128);
            }
            if (Q > 2500)
            {
                return new PredicateResult(119);
            }
            if (pstat > 0.01)
            {
                return new PredicateResult(121);
            }
            if (pmax > 0.9)
            {
                return new PredicateResult(123);
            }
            if (kst > 22)
            {
                return new PredicateResult(129);
            }
            if (kst < 10)
            {
                r.AddTransform(6, (t) => { t.Cache = kst; kst = 10; }, (t) => { kst = t.Cache.As<double>(); });
            }
            return r;
        }
        /// <summary>
        /// 切向进料
        /// </summary>
        /// <returns></returns>
        CalcResult Calculate_TanFeed()
        {
            CalcResult r = new CalcResult(1);
            r.Succeed = true;

            double Dz = Math.Pow(4 * V / Math.PI, 1 / 3);
            double k = double.NaN;
            if (predmax >= 0.01 && predmax <= 0.1)
            {
                k = 1;
            }
            else if (predmax <= 0.17)
            {
                k = 2;
            }
            double X = ((8.6 * (1 + Math.Log10(predmax)) / k - kst / 4.4 - 0.513) / Dz - 5.5 * (1 + Math.Log10(predmax)) / k + kst / 6.9 + 0.191) * 0.11 * kst * Df;
            double Y = 0.166 * Math.Exp(kst / 12.9) * Math.Pow(10 * predmax, -1.27 / k);

            r.Results[0] = X * (1 + Y * Math.Log10(L2DE));
            return r;
        }

        #endregion

        #region 考虑泄压导管

        /// <summary>
        /// 是否考虑泄压导管
        /// </summary>
        public bool ConsiderDuct { get; set; }
        /// <summary>
        /// 泄压导管长度，仅在考虑泄压导管时使用
        /// </summary>
        public double Lduct { get; set; }
        /// <summary>
        /// 泄压导管试算过程迭代中的predmax降低步长
        /// </summary>
        public double DuctStep { get; set; }

        /// <summary>
        /// 泄压导管的计算公式适用性验证
        /// </summary>
        /// <returns></returns>
        PredicateResult Predicate_Duct()
        {
            var r = PredicateResult.Succeed;
            if (V < 0.1 || V > 10000)
            {
                return new PredicateResult(130);
            }
            if (pstat > 0.1)
            {
                return new PredicateResult(131);
            }
            if (pstat < 0.01)
            {
                r.AddTransform(2, (t) => { t.Cache = pstat; pstat = 0.01; }, (t) => { pstat = t.Cache.As<double>(); });
            }
            if (predmax <= 0.01 || predmax > 0.2)
            {
                return new PredicateResult(112);
            }
            if (kst > 80)
            {
                return new PredicateResult(114);
            }
            if (kst < 1)
            {
                r.AddTransform(3, (t) => { t.Cache = kst; kst = 1; }, (t) => { kst = t.Cache.As<double>(); });
            }
            if (pmax > 1.2)
            {
                return new PredicateResult(115);
            }
            if (pmax < 0.5)
            {
                r.AddTransform(4, (t) => { t.Cache = pmax; pmax = 0.5; }, (t) => { pmax = t.Cache.As<double>(); });
            }
            return r;
        }
        /// <summary>
        /// 计算考虑泄压导管的提升后最大泄爆压力
        /// </summary>
        /// <param name="A">不考虑泻压导管时的泄爆面积</param>
        /// <returns></returns>
        CalcResult Calculate_Duct(double A)
        {
            CalcResult r = new CalcResult(1);
            r.Succeed = true;

            var l = 1.947 * Math.Pow(predmax, -0.37);
            if (Lduct < l)
            {
                l = Lduct;
            }
            else
            {
                r.Remarks.Add("已截取显著影响的泄压导管长度");
            }
            double v1 = predmax * (1 + 17.3 * Math.Pow(A * Math.Pow(V, -0.753), 1.6) * l);
            double v6 = 0.1 * (0.0586 * l + 1.023) * Math.Pow(10 * predmax, 0.981 - 0.01907 * l);
            if (L2DE == 1)
            {
                r.Results[0] = v1;
            }
            else if (L2DE == 6)
            {
                r.Results[0] = v6;
            }
            else
            {
                r.Results[0] = 0.2 * (v1 - v6) * (1 - L2DE) + v1;
            }

            return r;
        }

        #endregion

        /// <summary>
        /// 不考虑泄压导管的情况下，进行一次计算
        /// </summary>
        /// <returns></returns>
        CalcResult Calculate_WithoutDuct()
        {
            CalcResult r = new CalcResult(1);
            //一般方法验证
            var p_n = Predicate_Normal();
            if (p_n.IsSucceed)
            {
                r.Remarks.Add("采用一般方法进行计算");
                //三步走
                Utility.DoTransform(p_n.Transforms);
                var c_n = Calculate_Normal();
                Utility.UndoTransform(p_n.Transforms);
                r.MergeRemarks(c_n.Remarks);
                r.Succeed = c_n.Succeed;
                if (c_n.Succeed)
                {
                    r.MergeRemarks(p_n.Transforms.Select(item => $"已转换: {item.Description}"));
                    r.Results[0] = c_n.Results[0];
                }
            }
            else
            {
                //考虑附录中的方法
                r.Remarks.Add($"一般计算方法不适用: {p_n.Description}");
                switch (Feed)
                {
                    case VentDesign.FeedingWay.PneumaticTangential:
                        {
                            var p_t = Predicate_TanFeed();
                            if (p_t.IsSucceed)
                            {
                                r.Remarks.Add("采用切向进料计算");
                                Utility.DoTransform(p_t.Transforms);
                                var c_t = Calculate_TanFeed();
                                Utility.UndoTransform(p_t.Transforms);
                                r.MergeRemarks(c_t.Remarks);
                                r.Succeed = c_t.Succeed;
                                if (c_t.Succeed)
                                {
                                    r.MergeRemarks(p_t.Transforms.Select(item => $"已转换: {item.Description}"));
                                    r.Results[0] = c_t.Results[0];
                                }
                            }
                            else
                            {
                                r.Remarks.Add($"切向进料不适用:{p_t.Description}");
                            }
                        }
                        break;
                    default:
                        {
                            var p_a = Predicate_AxisFeed();
                            if (p_a.IsSucceed)
                            {
                                r.Remarks.Add("采用轴向中心/自由落体进料计算");
                                Utility.DoTransform(p_a.Transforms);
                                var c_a = Calculate_AxisFeed();
                                Utility.UndoTransform(p_a.Transforms);
                                r.MergeRemarks(c_a.Remarks);
                                r.Succeed = c_a.Succeed;
                                if (c_a.Succeed)
                                {
                                    r.MergeRemarks(p_a.Transforms.Select(item => $"已转换: {item.Description}"));
                                    r.Results[0] = c_a.Results[0];
                                }
                            }
                            else
                            {
                                r.Remarks.Add($"轴向进料不适用:{p_a.Description}");
                            }
                        }
                        break;
                }
            }

            return r;
        }

        /// <summary>
        /// 综合计算
        /// </summary>
        /// <returns></returns>
        public override CalcResult Calculate()
        {
            if (ConsiderDuct)
            {
                CalcResult r = new CalcResult(1);

                //考虑导管的最大泄爆压力
                double pp = p;
                //在迭代中变化的predmax取值
                double pmod = predmax;
                //缓存原始数值
                double cache = predmax;
                while (pp >= p)
                {
                    r.Remarks.Add($"设不考虑泄压导管的最大泄爆压力为: {pmod}");
                    predmax = pmod;
                    //计算面积
                    var c_wd = Calculate_WithoutDuct();
                    r.MergeRemarks(c_wd.Remarks);
                    if (!c_wd.Succeed)
                    {
                        r.Succeed = false;
                        return r;
                    }
                    r.Results[0] = c_wd.Results[0];
                    r.Remarks.Add($"    无导管泄压面积: {c_wd.Results[0]}");
                    //校验面积是否可用
                    var c_pp = Calculate_Duct(c_wd.Results[0]);
                    r.MergeRemarks(c_pp.Remarks);
                    if (!c_pp.Succeed)
                    {
                        r.Succeed = false;
                        return r;
                    }
                    pp = c_pp.Results[0];
                    r.Remarks.Add($"    抬升的泄爆压力为: {pp}");
                    pmod -= DuctStep;
                }
                predmax = cache;

                r.Succeed = true;
                return r;
            }
            else
            {
                return Calculate_WithoutDuct();
            }
        }

        public new string GenXml()
        {
            return $"<Container>{base.GenXml()}<spec><V>{V}</V><poperation>{poperation}</poperation><L2DE>{L2DE}</L2DE></spec><silo><Height>{Height}</Height><Df>{Df}</Df><uL>{uL}</uL><Q>{Q}</Q><Feed>{Feed.ToString()}</Feed></silo><duct><ConsiderDuct>{ConsiderDuct}</ConsiderDuct><Lduct>{Lduct}</Lduct><DuctStep>{DuctStep}</DuctStep></duct></Container>";
        }

        public new void ReadXml(XElement element)
        {
            base.ReadXml(element.Element("Common"));
            V = element.Element("spec").Element("V").Value.As<double>();
            poperation = element.Element("spec").Element("poperation").Value.As<double>();
            L2DE = element.Element("spec").Element("L2DE").Value.As<double>();
            Height = element.Element("silo").Element("Height").Value.As<double>();
            Df = element.Element("silo").Element("Df").Value.As<double>();
            uL = element.Element("silo").Element("uL").Value.As<double>();
            Q = element.Element("silo").Element("Q").Value.As<double>();
            Feed = element.Element("silo").Element("Feed").Value.As<VentDesign.FeedingWay>();
            ConsiderDuct = element.Element("duct").Element("ConsiderDuct").Value.As<bool>();
            Lduct = element.Element("duct").Element("Lduct").Value.As<double>();
            DuctStep = element.Element("duct").Element("DuctStep").Value.As<double>();
        }

        public new void RaiseAllPropertiesChangedEvent()
        {
            base.RaiseAllPropertiesChangedEvent();
            RaisePropertyChangedEvent("V");
            RaisePropertyChangedEvent("poperation");
            RaisePropertyChangedEvent("L2DE");
            RaisePropertyChangedEvent("Height");
            RaisePropertyChangedEvent("Df");
            RaisePropertyChangedEvent("uL");
            RaisePropertyChangedEvent("Q");
            RaisePropertyChangedEvent("Feed");
            RaisePropertyChangedEvent("ConsiderDuct");
            RaisePropertyChangedEvent("Lduct");
            RaisePropertyChangedEvent("DuctStep");
        }
    }
    public class PipeEntity : Entity15605_2008
    {
        /// <summary>
        /// 直径
        /// </summary>
        public double d { get; set; }
        /// <summary>
        /// 获取 面积
        /// </summary>
        public double A { get { return Math.PI * d * d / 4; } }
        /// <summary>
        /// 管段长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 相连设备上泄压装置的静开启压力
        /// </summary>
        public double pstat_con { get; set; }

        /// <summary>
        /// 管道验证
        /// </summary>
        /// <returns></returns>
        PredicateResult Predicate_Fit()
        {
            if (pstat > pstat_con)
            {
                return new PredicateResult(103);
            }
            return PredicateResult.Succeed;
        }

        CalcResult Calculate_Internal()
        {
            var r = new CalcResult(1) { Succeed = true };
            r.Results[0] = A;
            return r;
        }

        public override CalcResult Calculate()
        {
            CalcResult r = new CalcResult(1);

            var p_f = Predicate_Fit();
            if (p_f.IsSucceed)
            {
                Utility.DoTransform(p_f.Transforms);
                var c = Calculate_Internal();
                Utility.UndoTransform(p_f.Transforms);
                r.Succeed = c.Succeed;
                r.MergeRemarks(c.Remarks);
                if (r.Succeed)
                {
                    r.MergeRemarks(p_f.Transforms.Select(item => $"已转换: {item.Description}"));
                    r.Results[0] = c.Results[0];
                }
            }
            else
            {
                r.Succeed = false;
                r.Remarks.Add($"管道计算不适用: {p_f.Description}");
            }
            return r;
        }

        public new string GenXml()
        {
            return $"<Pipe>{base.GenXml()}<spec><d>{d}</d><Length>{Length}</Length><pstat_con>{pstat_con}</pstat_con></spec></Pipe>";
        }

        public new void ReadXml(XElement element)
        {
            base.ReadXml(element.Element("Common"));
            d = element.Element("spec").Element("d").Value.As<double>();
            Length = element.Element("spec").Element("Length").Value.As<double>();
            pstat_con = element.Element("spec").Element("pstat_con").Value.As<double>();
        }

        public new void RaiseAllPropertiesChangedEvent()
        {
            base.RaiseAllPropertiesChangedEvent();
            RaisePropertyChangedEvent("d");
            RaisePropertyChangedEvent("Length");
            RaisePropertyChangedEvent("pstat_con");
            RaisePropertyChangedEvent("A");
        }
    }
    public class BuildingEntity : Entity15605_2008
    {
        /// <summary>
        /// 建筑物水平尺度1
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 建筑物水平尺度2
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 建筑物高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 获取 建筑物体积
        /// </summary>
        public double V { get { return Length * Width * Height; } }

        PredicateResult Predicate()
        {
            if (predmax < 0.002 || predmax > 0.01)
            {
                return new PredicateResult(132);
            }
            if (pstat >= predmax / 2)
            {
                return new PredicateResult(133);
            }
            if (kst < 1 || kst > 30)
            {
                return new PredicateResult(134);
            }
            if (pmax < 0.5 || pmax > 1)
            {
                return new PredicateResult(135);
            }

            return PredicateResult.Succeed;
        }

        CalcResult Calculate_Internal()
        {
            CalcResult r = new CalcResult(1);
            r.Succeed = true;

            var C = -4.305 * Math.Log10(predmax) - 3.547;
            var L3 = Math.Max(Width, Length);
            var De = 2 * Math.Sqrt(Width * Length / Math.PI);
            r.Results[0] = 0.0008805 * pmax * kst * Math.Pow(predmax, -0.569) * Math.Pow(V, 0.753) * (1 + C * Math.Log10(L3 / De));

            return r;
        }

        public override CalcResult Calculate()
        {
            CalcResult r = new CalcResult(1);

            var p_f = Predicate();
            if (p_f.IsSucceed)
            {
                Utility.DoTransform(p_f.Transforms);
                var c = Calculate_Internal();
                Utility.UndoTransform(p_f.Transforms);
                r.Succeed = c.Succeed;
                r.MergeRemarks(c.Remarks);
                if (r.Succeed)
                {
                    r.MergeRemarks(p_f.Transforms.Select(item => $"已转换: {item.Description}"));
                    r.Results[0] = c.Results[0];
                }
            }
            else
            {
                r.Succeed = false;
                r.Remarks.Add($"建筑物计算不适用: {p_f.Description}");
            }
            return r;
        }

        public new string GenXml() { return $"<Building>{base.GenXml()}<spec><Length>{Length}</Length><Width>{Width}</Width><Height>{Height}</Height></spec></Building>"; }

        public new void ReadXml(XElement element)
        {
            base.ReadXml(element.Element("Common"));
            Length = element.Element("spec").Element("Length").Value.As<double>();
            Width = element.Element("spec").Element("Width").Value.As<double>();
            Height = element.Element("spec").Element("Height").Value.As<double>();
        }

        public new void RaiseAllPropertiesChangedEvent()
        {
            base.RaiseAllPropertiesChangedEvent();
            RaisePropertyChangedEvent("Length");
            RaisePropertyChangedEvent("Width");
            RaisePropertyChangedEvent("Height");
            RaisePropertyChangedEvent("V");
        }
    }
    public class SystemEntity : Entity15605_2008
    {
        /// <summary>
        /// 组合系统中的受保护容器1
        /// </summary>
        public ContainerEntity Container1 { get; set; } = new ContainerEntity();
        /// <summary>
        /// 组合系统中的受保护容器2
        /// </summary>
        public ContainerEntity Container2 { get; set; } = new ContainerEntity();
        /// <summary>
        /// 管道
        /// </summary>
        public PipeEntity Pipe { get; set; } = new PipeEntity();

        /// <summary>
        /// 组合系统验证
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        PredicateResult Predicate_Fit()
        {
            if (Pipe.d <= 0.3)
            {
                if (Pipe.d < 0.3 || Pipe.Length > 6)
                {
                    return new PredicateResult(105);
                }
                else
                {
                    if (Pipe.pstat >= 0.02) return new PredicateResult(106);
                    if (Math.Abs(Container1.V - Container2.V) / Math.Max(Container1.V, Container2.V) > 0.1)
                    {
                        var r = PredicateResult.Succeed;
                        r.AddTransform(1,
                            (t) =>
                            {
                                t.Cache = new double[] { Container1.predmax, Container2.predmax };
                                Container1.predmax = 0.1; Container2.predmax = 0.1;
                            },
                            (t) => { Container1.predmax = (t.Cache as double[])[0]; Container2.predmax = (t.Cache as double[])[1]; });
                        return r;
                    }
                }
            }
            else
            {
                return new PredicateResult(104);
            }

            return PredicateResult.Succeed;
        }

        CalcResult Calculate_Internal()
        {
            CalcResult r = new CalcResult(3);

            var c1 = Container1.Calculate();
            r.MergeRemarks(c1.Remarks);
            if (!c1.Succeed)
            {
                r.Succeed = false;
                return r;
            }
            var c2 = Container2.Calculate();
            r.MergeRemarks(c2.Remarks);
            if (!c2.Succeed)
            {
                r.Succeed = false;
                return r;
            }
            var cp = Pipe.Calculate();
            r.MergeRemarks(cp.Remarks);
            if (!cp.Succeed)
            {
                r.Succeed = false;
                return r;
            }

            r.Results[0] = c1.Results[0];
            r.Results[1] = cp.Results[0];
            r.Results[2] = c2.Results[0];
            r.Succeed = true;
            return r;
        }
        public override CalcResult Calculate()
        {
            CalcResult r = new CalcResult(3);

            var p_f = Predicate_Fit();
            if (p_f.IsSucceed)
            {
                Utility.DoTransform(p_f.Transforms);
                var c = Calculate_Internal();
                Utility.UndoTransform(p_f.Transforms);
                r.Succeed = c.Succeed;
                r.MergeRemarks(c.Remarks);
                if (r.Succeed)
                {
                    r.MergeRemarks(p_f.Transforms.Select(item => $"已转换: {item.Description}"));
                    r.Results[0] = c.Results[0];
                    r.Results[1] = c.Results[1];
                    r.Results[2] = c.Results[2];
                }
            }
            else
            {
                r.Succeed = false;
                r.Remarks.Add($"组合系统计算不适用: {p_f.Description}");
            }
            return r;
        }

        public new string GenXml() { return $"<System>{Container1.GenXml()}{Pipe.GenXml()}{Container2.GenXml()}</System>"; }

        public new void ReadXml(XElement element)
        {
            Container1.ReadXml(element.Element("Container"));
            Pipe.ReadXml(element.Element("Pipe"));
            Container2.ReadXml(element.Elements("Container").ElementAt(1));
        }

        public new void RaiseAllPropertiesChangedEvent()
        {
            Container1.RaiseAllPropertiesChangedEvent();
            Pipe.RaiseAllPropertiesChangedEvent();
            Container2.RaiseAllPropertiesChangedEvent();
        }
    }

    #endregion
}

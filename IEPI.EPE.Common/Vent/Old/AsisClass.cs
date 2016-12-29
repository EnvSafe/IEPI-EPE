using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Drawing;

namespace IEPI.EPE.VentDesign
{
    #region 可用性检查支持

    /// <summary>
    /// 代表一次检查的结果，常用作检查方法的返回值
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class CheckInfo
    {
        /// <summary>
        /// 代表一次成功的检查结果
        /// </summary>
        public CheckInfo() : this(true, StdStrs.Succ) { }
        /// <summary>
        /// 代表一次失败的检查结果
        /// </summary>
        /// <param name="Description">失败明细描述</param>
        /// <param name="FailType">检查失败类型</param>
        public CheckInfo(string Description, ParamExceptionType FailType) : this(false, Description, FailType) { }
        /// <summary>
        /// 代表一次适用性检查的结果，常用作检查方法的返回值
        /// </summary>
        /// <param name="Status">适用性检查结果状态</param>
        /// <param name="Description">状态明细描述</param>
        public CheckInfo(bool Status, string Description) : this(Status, Description, ParamExceptionType.NA) { }
        /// <summary>
        /// 代表一次检查的结果，常用作检查方法的返回值
        /// </summary>
        /// <param name="Status">检查结果状态</param>
        /// <param name="Description">状态明细描述</param>
        /// <param name="FailType">检查失败类型</param>
        public CheckInfo(bool Status, string Description, ParamExceptionType FailType)
        {
            _Status = Status;
            _Description = Description;
            _FailType = FailType;
        }

        /// <summary>
        /// 获取此次检查的结果状态
        /// </summary>
        public bool Status
        { get { return _Status; } internal set { _Status = value; } }
        bool _Status;

        /// <summary>
        /// 获取此次结果状态的详细描述
        /// </summary>
        public string Description
        { get { return _Description; } }
        string _Description;

        /// <summary>
        /// 获取检查失败的原因类型
        /// </summary>
        public ParamExceptionType FailType
        { get { return _FailType; } }
        ParamExceptionType _FailType;
    }

    /// <summary>
    /// 参数计算时异常
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class BadParamException : Exception
    {
        /// <summary>
        /// 参数计算时异常
        /// </summary>
        /// <param name="ParamDescription">参数描述</param>
        /// <param name="Type">异常类型</param>
        public BadParamException(string ParamDescription, ParamExceptionType Type):base(ParamDescription)
        { this.Type = Type; }
        /// <summary>
        /// 异常类型
        /// </summary>
        public readonly ParamExceptionType Type;
    }

    /// <summary>
    /// 参数异常的类型
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public enum ParamExceptionType
    {
        /// <summary>
        /// 参数的值不在适用范围内
        /// </summary>
        NA,
        /// <summary>
        /// 参数的值尚未有效定义
        /// </summary>
        TBD,
        /// <summary>
        /// 参数赋值非法
        /// </summary>
        IL
    }

    #endregion

    #region 参数对、参数集合

    public class Param
    {
        public string Name;
        public object Value;
    }

    public class ParamCollection
    {
        public ParamCollection()
        { Parameters = new List<Param>(); }

        List<Param> Parameters;

        public Param this[int Index]
        {
            get { return Parameters[Index]; }
            set { Parameters[Index] = value; }
        }

        public Param Add(string Name, object Value)
        {
            Param NewParam = new Param();
            NewParam.Name = Name;
            NewParam.Value = Value;
            Parameters.Add(NewParam);
            return NewParam;
        }

        public void Remove(string Name)
        {
            Parameters.Remove(GetParam(Name));
        }

        public Param GetParam(string Name)
        {
            for (int i = 0; i < Parameters.Count; i++)
                if (Parameters[i].Name == Name)
                    return Parameters[i];
            throw new Exception("找不到名为" + Name + "的参数！");
        }

        public void SetParam(string Name, object Value)
        {
            for (int i = 0; i < Parameters.Count; i++)
                if (Parameters[i].Name == Name)
                {
                    Parameters[i].Value = Value;
                }
            throw new Exception("找不到名为" + Name + "的参数！");
        }
    }

    /// <summary>
    /// 代表一个拥有名称的值参数
    /// </summary>
    public class Param<T>
    {
        public string Name;
        public T Value;
    }

    public class ParamCollection<T>
    {
        public ParamCollection()
        { Parameters = new List<Param<T>>(); }

        List<Param<T>> Parameters;

        public Param<T> this[int Index]
        {
            get { return Parameters[Index]; }
            set { Parameters[Index] = value; }
        }

        public Param<T> Add(string Name, T Value)
        {
            Param<T> NewParam = new Param<T>();
            NewParam.Name = Name;
            NewParam.Value = Value;
            Parameters.Add(NewParam);
            return NewParam;
        }

        public void Remove(string Name)
        {
            Parameters.Remove(GetParam(Name));
        }

        public Param<T> GetParam(string Name)
        {
            for (int i = 0; i < Parameters.Count; i++)
                if (Parameters[i].Name == Name)
                    return Parameters[i];
            throw new Exception("找不到名为" + Name + "的参数！");
        }

        public void SetParam(string Name, T Value)
        {
            for (int i = 0; i < Parameters.Count; i++)
                if (Parameters[i].Name == Name)
                {
                    Parameters[i].Value = Value;
                }
            throw new Exception("找不到名为" + Name + "的参数！");
        }
    }

    #endregion

    #region 提供有关绘图方面的支持

    /// <summary>
    /// 表示一个无符号曲线集，用于某时刻显示一组曲线
    /// </summary>

    [Obsolete("版本2.2之前的接口已停止维护")]
    public class PureLineCollection
    {
        /// <summary>
        /// 实例化一个无符号曲线集，用于某时刻显示一组曲线
        /// </summary>
        /// <param name="Title">曲线集的标题</param>
        /// <param name="XMax">x轴的最大显示边界</param>
        /// <param name="XMin">x轴的最小显示边界</param>
        /// <param name="XLabel">x轴的标签</param>
        /// <param name="XFormat">x轴数据的格式</param>
        /// <param name="YMax">y轴的最大显示边界</param>
        /// <param name="YMin">y轴的最小显示边界</param>
        /// <param name="YLabel">y轴的标签</param>
        /// <param name="YFormat">y轴数据的格式</param>
        public PureLineCollection(string Title, double XMax, double XMin, string XLabel, string XFormat, double YMax, double YMin, string YLabel, string YFormat)
        {
            this.Title = Title;
            this.XMax = XMax; this.XMin = XMin; this.XLabel = XLabel; this.XFormat = XFormat;
            this.YMax = YMax; this.YMin = YMin; this.YLabel = YLabel; this.YFormat = YFormat;
            Lines = new List<LineItem>();
        }
        public readonly string Title;
        public readonly double XMax; public readonly double XMin; public readonly string XLabel; public readonly string XFormat;
        public readonly double YMax; public readonly double YMin; public readonly string YLabel; public readonly string YFormat;
        public List<LineItem> Lines;

        /// <summary>
        /// 添加一条曲线到曲线集
        /// </summary>
        /// <param name="Legend">曲线的图例文本</param>
        /// <param name="Dots">曲线的点集</param>
        /// <param name="CurveColor">曲线的颜色</param>
        /// <returns>被添加的曲线实例</returns>
        public LineItem Add(string Legend, PointPairList Dots, Color CurveColor)
        {
            LineItem NewLine = new LineItem(Legend, Dots, CurveColor, SymbolType.None);
            NewLine.Line.Width = 1.5F;
            Lines.Add(NewLine);
            return NewLine;
        }
    }

    #endregion
}

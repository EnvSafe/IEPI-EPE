using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Heroius.Extension;

namespace IEPI.EPE.Vent
{
    /// <summary>
    /// 作为计算上下文的基础类型
    /// </summary>
    public abstract class Context : ObservableEntity
    {
    }

    /// <summary>
    /// 作为目标实体的基础类型
    /// </summary>
    public abstract class Entity : ObservableEntity
    {
        //note: 持有公用的计算参数
    }

    #region Predication

    /// <summary>
    /// 验证标准或计算方法适用条件是否通过
    /// </summary>
    /// <param name="context">计算上下文</param>
    /// <returns></returns>
    public delegate PredicateResult PredicateItem(Context context);

    /// <summary>
    /// 配合PredicateItem，作为验证的结果，指示未通过的条件
    /// </summary>
    public class PredicateResult
    {
        public PredicateResult(int code)
        {
            Code = code;
            Description = codeMapping[code];
        }

        /// <summary>
        /// 检验结果编码
        /// </summary>
        public int Code { get; private set; }
        /// <summary>
        /// shortcut Code == 0
        /// </summary>
        public bool IsSucceed { get { return Code == 0; } }
        /// <summary>
        /// 检验结果说明
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 检验过程中生成的转换请求
        /// <para>往往在检验通过时，才考虑转换</para>
        /// </summary>
        public ObservableCollection<TransformOperation> Transforms { get; private set; } = new ObservableCollection<TransformOperation>();

        /// <summary>
        /// shortcut to add transform
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="To"></param>
        /// <param name="Back"></param>
        internal void AddTransform(int Code, Action<TransformOperation> To, Action<TransformOperation> Back)
        {
            TransformOperation t = new TransformOperation(Code);
            t.TransformTo = To;
            t.TransformBack = Back;
            Transforms.Add(t);
        }

        #region Predefined

        /// <summary>
        /// 获取 成功结果的一个新实例
        /// </summary>
        public static PredicateResult Succeed { get { return new PredicateResult(0); } }

        /// <summary>
        /// 代码-描述映射字典
        /// </summary>
        static Dictionary<int, string> codeMapping = new Dictionary<int, string>() {
            { 0 ,  "成功" },
            {101, "设计压力应大于泄爆设备静开启压力" },
            {102, "设计压力不得小于最大泄爆压力" },
            {103, "管道泄压装置的静开启压力应不大于相连设备的泄压装置静开启压力" },
            {104, "连接管道工程直径DN>300的组合系统，应向专家咨询" },
            {105, "仅DN300且长度不超过6m的管道采用泄压方法" },
            {106, "管道泄压装置静开启压力应小于0.02MPa" },
            {107, "容器正常操作压力不得超过0.02MPa" },
            {108, "St1/St2的粉尘最大爆炸压力应不大于1MPa" },
            {109, "不适用的St等级" },
            {110, "St3的粉尘最大爆炸压力应不大于1.2MPa" },
            {111, "容器容积超出适用范围" },
            {112, "最大泄爆压力超出适用范围" },
            {113, "静开启压力不可大于0.1MPa" },
            {114, "粉尘爆炸指数不应大于80MPa.m/s" },
            {115, "粉尘最大爆炸压力过大" },
            {116, "长径比不应大于20" },
            {117, "中心进料料仓容积超出适用范围[5,10000]m^3" },
            {118, "中心进料料仓进料速度不应大于40m/s" },
            {119, "中心进料料仓空气流量应小于等于2500m^3/h" },
            {120, "中心进料料仓进料直径不应大于0.3m" },
            {121, "静开启压力不应大于0.01MPa" },
            {122, "最大泄爆压力不得超出适用范围(0.01, 0.2]MPa" },
            {123, "粉尘最大爆炸压力不应大于0.9MPa" },
            {124, "粉尘爆炸指数不得超出使用范围[5, 30]MPa.m/s" },
            {125, "切向进料料仓进料直径不应大于0.2m"  },
            {126, "切向进料料仓容积超出适用范围[6,120]m^3" },
            {127, "长径比超出适用范围[1,5]" },
            {128, "中心进料料仓进料速度不应大于40m/s" },
            {129, "粉尘爆炸指数不得超出使用范围[10, 22]MPa.m/s" },
            {130, "泄压导管计算中容积超出适用范围[0.1,10000]m^3" },
            {131, "静开启压力不应大于0.1MPa" },
            {132, "建筑物最大泄爆压力适用范围为[0.002, 0.01]MPa" },
            {133, "建筑物设计压力应小于最大泄爆压力的一半" },
            {134, "粉尘爆炸指数不应超出适用范围[1, 30]MPa.m/s" },
            {135, "粉尘最大爆炸压力不应超出适用范围[0.5,1]MPa" }
        };

        #endregion
    }

    #endregion

    /// <summary>
    /// 计算的结果
    /// </summary>
    public class CalcResult
    {
        public CalcResult(int ResultsLength)
        {
            Results = new double[ResultsLength];
        }

        /// <summary>
        /// 泄压面积, m^2
        /// <para>对于包含多个值的情况，是构成复杂系统的计算，结果顺序为 容器1 - 管道 - 容器2</para>
        /// </summary>
        public double[] Results { get; private set; } 

        /// <summary>
        /// 备注
        /// </summary>
        public ObservableCollection<string> Remarks { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// 计算成功标记
        /// </summary>
        public bool Succeed { get; internal set; }

        /// <summary>
        /// 向当前备注中加入现有备注
        /// </summary>
        /// <param name="remarks"></param>
        internal void MergeRemarks(IEnumerable<string> remarks)
        {
            foreach (var item in remarks)
            {
                Remarks.Add(item);
            }
        }

        /// <summary>
        /// copy results from existing instance
        /// <para>warning: this method will reallocate results array, do not use unless you have to change the array length</para>
        /// </summary>
        /// <param name="r"></param>
        internal void CopyResultFrom(CalcResult r)
        {
            Results = new double[r.Results.Length];
            for (int i = 0; i < r.Results.Length; i++)
            {
                Results[i] = r.Results[i];
            }
        }
    }

    /// <summary>
    /// 数据临时转换项
    /// </summary>
    public class TransformOperation
    {
        public TransformOperation(int code)
        {
            Code = code;
            Description = codeMapping[code];
        }

        /// <summary>
        /// 内部专用 识别码
        /// </summary>
        internal int Code { get; set; }

        /// <summary>
        /// 缓存转换前的值
        /// </summary>
        public object Cache { get; set; }
        /// <summary>
        /// 正向转换
        /// </summary>
        public Action<TransformOperation> TransformTo { get; set; }
        /// <summary>
        /// 逆向转换
        /// </summary>
        public Action<TransformOperation> TransformBack { get; set; }

        /// <summary>
        /// 转换操作说明
        /// </summary>
        public string Description { get; internal set; }

        static Dictionary<int, string> codeMapping = new Dictionary<int, string>()
        {
            {1, "容器容积不同时，采用predmax<=0.1MPa进行计算" },
            {2, "静开启压力小于应用范围，取最小值0.01MPa" },
            {3, "粉尘爆炸指数小于应用范围，取最小值1MPa.m/s" },
            {4, "粉尘最大爆炸压力小于应用范围，取最小值 0.5MPa" },
            {5, "轴向中心送料风量大于2500m^3/h时，若设计压力>0.025MPa，则将predmax取0.01MPa" },
            {6, "切向进料时，若Kst小于10MPa.m/s则取10MPa.m/s" }
        };
    }
}

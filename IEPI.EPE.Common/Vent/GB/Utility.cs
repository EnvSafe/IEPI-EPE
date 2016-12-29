using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IEPI.EPE.Vent
{
    /// <summary>
    /// 提供基于标准GB的通用工具
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 通过爆炸指数Kmax值将粉尘爆炸猛烈程度分为三级
        /// </summary>
        /// <param name="Kmax">爆炸指数</param>
        /// <returns>粉尘爆炸猛烈等级</returns>
        public static VentDesign.ExplosionLevel GetStByKmax(double Kmax)
        {
            if (Kmax > 30)
            {
                return VentDesign.ExplosionLevel.St3;
            }
            else if (Kmax > 20)
            {
                return VentDesign.ExplosionLevel.St2;
            }
            else if (Kmax > 0)
            {
                return VentDesign.ExplosionLevel.St1;
            }
            else return VentDesign.ExplosionLevel.Unknown;
        }

        /// <summary>
        /// 通过截面积计算当量直径
        /// </summary>
        /// <param name="Astar"></param>
        /// <returns></returns>
        public static double GetDEua(double Astar)
        {
            return 2 * Math.Sqrt(Astar / Math.PI);
        }

        /// <summary>
        /// 对特定上下文执行给定的验证
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="predicates">验证</param>
        /// <returns></returns>
        public static PredicateResult Predicate(Context context, ObservableCollection<PredicateItem> predicates)
        {
            for (int i = 0; i < predicates.Count; i++)
            {
                var r = predicates[i].Invoke(context);
                if (r.Code == 0) continue;
                else return r;
            }
            return PredicateResult.Succeed;
        }

        /// <summary>
        /// 尝试向列表中加入不重复的转换操作
        /// </summary>
        /// <param name="list"></param>
        /// <param name="instance"></param>
        public static void TryAddUniqTransformOperation(List<TransformOperation> list, TransformOperation instance)
        {
            if (list.Count(i=>i.Code == instance.Code)>0)
            {
                return;
            }
            else
            {
                list.Add(instance);
            }
        }
        /// <summary>
        /// 尝试向列表中加入不重复的转换操作
        /// </summary>
        /// <param name="list"></param>
        /// <param name="instances"></param>
        public static void TryAddUniqTransformOperation(List<TransformOperation> list, IEnumerable<TransformOperation> instances)
        {
            foreach (var item in instances)
            {
                TryAddUniqTransformOperation(list, item);
            }
        }

        /// <summary>
        /// 执行转换集合的操作
        /// </summary>
        /// <param name="list"></param>
        public static void DoTransform(IEnumerable<TransformOperation> list)
        {
            foreach (var item in list)
            {
                item.TransformTo.Invoke(item);
            }
        }
        /// <summary>
        /// 逆向转换操作
        /// </summary>
        /// <param name="list"></param>
        public static void UndoTransform(IEnumerable<TransformOperation> list)
        {
            foreach (var item in list)
            {
                item.TransformBack.Invoke(item);
            }
        }
    }
}

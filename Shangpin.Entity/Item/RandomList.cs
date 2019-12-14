using System;
using System.Collections.Generic;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 随机给LIST集合数据排序返回新的集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomList<T>
    {

        public static IList<T> GetRandomList(IList<T> obj)
        {
            IList<T> newlist = new List<T>();
            Random rd = new Random();
            foreach (var item in obj)
            {
                newlist.Insert(rd.Next(newlist.Count), item);
            }
            return newlist;
        }
    }     
}

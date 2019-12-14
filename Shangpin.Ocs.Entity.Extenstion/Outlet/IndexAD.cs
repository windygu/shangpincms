using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public static class IndexAD
    {
        public static string TopAD = "width:1680,Height:390,Length:300";
        public static string PageVerticalTodayAD = "width:300,Height:620,Length:150";
        public static string PageVerticalAD = "width:220,Height:460,Length:150";
        public static string PageHorizontalAD = "width:460,Height:220,Length:150";

        public static string GetLimitContion(int position)
        {
            string tmp = string.Empty;
            switch (position)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    tmp = TopAD;
                    break;
                case 10:
                    tmp = PageVerticalTodayAD;
                    break;
                case 11:
                case 12:
                case 13:
                    tmp = PageVerticalAD;
                    break;
                case 14:
                case 15:
                    tmp = PageHorizontalAD;
                    break;

            }
            return tmp;
        }


        public static string GetPositionName(int position)
        {
            string name = string.Empty;
            switch (position)
            {
                case (int)ADPosition.ADZero:
                    name = "今日新开广告";
                    break;
                case (int)ADPosition.ADOne:
                    name = "正在进行广告一";
                    break;
                case (int)ADPosition.ADTwo:
                    name = "正在进行广告二";
                    break;
                case (int)ADPosition.ADThree:
                    name = "正在进行广告三";
                    break;
                case (int)ADPosition.ADFour:
                    name = "正在进行广告四";
                    break;
            }
            return name;
        }

        public static List<AD> ADList()
        {
            List<AD> value = new List<AD>();
            //value.Add(new AD { Name = "顶部广告一", Position = (int)ADPosition.One });
            //value.Add(new AD { Name = "顶部广告二", Position = (int)ADPosition.Two });
            //value.Add(new AD { Name = "顶部广告三", Position = (int)ADPosition.Three });
            //value.Add(new AD { Name = "顶部广告四", Position = (int)ADPosition.Four });
            //value.Add(new AD { Name = "顶部广告五", Position = (int)ADPosition.Five });
            value.Add(new AD { Name = "今日新开广告", Position = (int)ADPosition.ADZero });
            value.Add(new AD { Name = "正在进行广告一", Position = (int)ADPosition.ADOne });
            value.Add(new AD { Name = "正在进行广告二", Position = (int)ADPosition.ADTwo });
            value.Add(new AD { Name = "正在进行广告三", Position = (int)ADPosition.ADThree });
            value.Add(new AD { Name = "正在进行广告四", Position = (int)ADPosition.ADFour });
            //value.Add(new AD { Name = "正在进行广告五", Position = (int)ADPosition.ADFive });
            return value;
        }
        public static int PagePosition() { return (int)ADPosition.PagePosition; }
    }

    public class AD
    {
        public string Name { get; set; }
        public int Position { get; set; }
    }


    /// <summary>
    /// 页面位置号
    /// </summary>
    public enum ADPosition
    {
        //当前页面内的具体的广告位置号
        One = 1,//顶部横版
        Two = 2,//顶部横版
        Three = 3,//顶部横版
        Four = 4,//顶部横版
        Five = 5,//顶部横版
        ADZero = 10,//今日新开的竖版广告
        ADOne = 11,//竖版
        ADTwo = 12,//竖版
        ADThree = 13,//竖版
        ADFour = 14,//横版
        ADFive = 15,//备用
        PagePosition = 2000 //当前页面号
    }
}

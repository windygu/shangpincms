namespace Shangpin.Entity.User
{
    /// <summary>
    /// 会员等级
    /// </summary>
    public class CustomerLevel
    {
        /// <summary>
        /// 钻石会员
        /// </summary>
        public const string DiamondLevelNo = "0004";

        /// <summary>
        /// 白金会员
        /// </summary>
        public const string PlatinumLevelNo = "0003";

        /// <summary>
        /// 黄金会员
        /// </summary>
        public const string GoldenLevelNo = "0002";

        /// <summary>
        /// 正式会员
        /// </summary>
        public const string NormalLevelNo = "0001";

        /// <summary>
        /// 注册用户
        /// </summary>
        public const string RegLevelNo = "0000";


        /// <summary>
        /// 钻石会员
        /// </summary>
        public const string DiamondLevelName = "钻石会员";

        /// <summary>
        /// 钻石会员
        /// </summary>
        public const string PlatinumLevelName = "白金会员";
        /// <summary>
        /// 黄金会员
        /// </summary>
        public const string GoldenLevelName = "黄金会员";
        /// <summary>
        /// 正式会员
        /// </summary>
        public const string NormalLevelName = "正式会员";

        /// <summary>
        /// 注册用户
        /// </summary>
        public const string RegLevelName = "注册用户";


        /// <summary>
        /// 黄金会员升级条件
        /// </summary>
        public const decimal GoldAmount = 5000;
        /// <summary>
        /// 白金升级条件
        /// </summary>
        public const decimal PlatinumAmount = 10000;
        /// <summary>
        /// 钻石升级条件
        /// </summary>
        public const decimal DiamondAmount = 30000;

        public const decimal SendCashTicketDiffAmount = 1800;

        /// <summary>
        /// 获取下一个等级
        /// </summary>
        /// <param name="levelNo"></param>
        /// <returns></returns>
        public static string GetNextLevelNo(string levelNo)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return DiamondLevelNo;
                case PlatinumLevelNo:
                    return DiamondLevelNo;
                case GoldenLevelNo:
                    return PlatinumLevelNo;
                case NormalLevelNo:
                    return GoldenLevelNo;
                case RegLevelNo:
                    return NormalLevelNo;
            }
            return null;
        }

        /// <summary>
        /// 获取下一级别name
        /// </summary>
        /// <param name="levelNo"></param>
        /// <returns></returns>
        public static string GetNextLevelName(string levelNo)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return DiamondLevelName;
                case PlatinumLevelNo:
                    return DiamondLevelName;
                case GoldenLevelNo:
                    return PlatinumLevelName;
                case NormalLevelNo:
                    return GoldenLevelName;
                case RegLevelNo:
                    return NormalLevelName;
            }
            return null;
        }

        /// <summary>
        ///  通过等级编号获取等级名称
        /// </summary>
        /// <param name="levelNo"></param>
        /// <returns></returns>
        public static string GetLevelNameByNo(string levelNo)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return DiamondLevelName;
                case PlatinumLevelNo:
                    return PlatinumLevelName;
                case GoldenLevelNo:
                    return GoldenLevelName;
                case NormalLevelNo:
                    return NormalLevelName;
                case RegLevelNo:
                    return RegLevelName;
            }
            return null;


        }

        public static decimal GetLevelAmount(string levelNo,decimal curConsumedAmount)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return DiamondAmount - curConsumedAmount;
                case PlatinumLevelNo:
                    return DiamondAmount - curConsumedAmount;
                case GoldenLevelNo:
                    return PlatinumAmount - curConsumedAmount;
                case NormalLevelNo:
                    return GoldAmount - curConsumedAmount;
                case RegLevelNo:
                    return  GoldAmount - curConsumedAmount;;
            }
            return 0;
        }
        /// <summary>
        /// 得到升级后的级别
        /// </summary>
        /// <param name="levelNo">当前级别</param>
        /// <param name="curConsumedAmount">消费金额</param>
        /// <returns></returns>
        public static string GetUpOrDownLevelNo(string levelNo, decimal curConsumedAmount)
        {
            if (curConsumedAmount >= DiamondAmount)
            {
                return DiamondLevelNo;
            }
            if (curConsumedAmount >= PlatinumAmount)
            {
                return PlatinumLevelNo;
            }
            if (curConsumedAmount >= GoldAmount)
            {
                return GoldenLevelNo;
            }
            return NormalLevelNo;
        }
        /// <summary>
        /// 得到级别的顺序
        /// </summary>
        /// <param name="levelNo"></param>
        /// <returns></returns>
        public static int GetLevelSequence(string levelNo)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return 5;
                case PlatinumLevelNo:
                    return 4;
                case GoldenLevelNo:
                    return 3;
                case NormalLevelNo:
                    return 2;
                case RegLevelNo:
                    return 1;
            }
            return 0;
        }
        /// <summary>
        /// 获取上一个等级
        /// </summary>
        /// <param name="levelNo"></param>
        /// <returns></returns>
        public static string GetLastLevelNo(string levelNo)
        {
            switch (levelNo)
            {
                case DiamondLevelNo:
                    return PlatinumLevelNo;
                case PlatinumLevelNo:
                    return GoldenLevelNo;
                case GoldenLevelNo:
                    return NormalLevelNo;
                case NormalLevelNo:
                    return NormalLevelNo;
                case RegLevelNo:
                    return RegLevelNo;
            }
            return null;
        }
    }


}

namespace Shangpin.Entity.GiftCard
{
    public class GiftCardErrorInfo
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        public const string E1000 = "1000";
        /// <summary>
        /// 参数不全不完整
        /// </summary>
        public const string E1001 = "1001";
        /// <summary>
        /// 礼品卡数据不存在
        /// </summary>
        public const string E1002 = "1002";
        /// <summary>
        /// 数据库操作异常
        /// </summary>
        public const string E1006 = "1006";
        /// <summary>
        /// 电子礼品卡用户和邮箱与领用时不一致
        /// </summary>
        public const string E1005 = "1005";
        /// <summary>
        /// 礼品卡没有进行充值
        /// </summary>
        public const string E1004 = "1004";
        /// <summary>
        /// 礼品卡状态值不符合激活操作
        /// </summary>
        public const string E1003 = "1003";
        /// <summary>
        /// 1007:礼品卡重复激活
        /// </summary>
        public const string E1007 = "1007";
        /// <summary>
        /// 用户卡账户已经存在
        /// </summary>
        public const string E1101 = "1101";
        /// <summary>
        /// 用户卡账户不存在
        /// </summary>
        public const string E1301 = "1301";
        /// <summary>
        /// 原密码输入不正确
        /// </summary>
        public const string E1302 = "1302";
        /// <summary>
        /// 礼品卡消费金额不足
        /// </summary>
        public const string E2003 = "2003";
        /// <summary>
        /// 礼品卡状态不对
        /// </summary>
        public const string E2002 = "2002";
        /// <summary>
        /// Exception
        /// </summary>
        public const string E2100 = "2100";
        /// <summary>
        /// 礼品卡退还—卡不属于用户
        /// </summary>
        public const string E2004 = "2004";
        /// <summary>
        /// 礼品卡退还-没有消费记录
        /// </summary>
        public const string E2005 = "2005";
        /// <summary>
        /// 礼品卡退还-退还金额不对
        /// </summary>
        public const string E2006 = "2006";
        /// <summary>
        /// 礼品卡退还-不存在消费记录
        /// </summary>
        public const string E2007 = "2007";
        /// <summary>
        /// 消费礼品卡状态不对
        /// </summary>
        public const string E2008 = "2008";
        /// <summary>
        /// 根据错误码返回错误信息
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns>返回错误信息</returns>
        public static string GetErrorInfo(string errorCode)
        {
            string errorInfo = "操作失败";
            if (errorCode.Equals(E1000))
            {
                errorInfo = "操作成功";
            }
            else  if (errorCode.Equals(E1001))
            {
                errorInfo = "参数不全不完整";
            }
            else if (errorCode.Equals(E1002))
            {
                errorInfo = "您输入的卡号或密码错误";
            }
            else if (errorCode.Equals(E1006))
            {
                errorInfo = "数据库操作异常";
            }
            else if (errorCode.Equals(E1005))
            {
                errorInfo = "电子礼品卡用户和邮箱与领用时不一致";
            }
            else if (errorCode.Equals(E1004))
            {
                errorInfo = "礼品卡没有进行充值";
            }
            else if (errorCode.Equals(E1003))
            {
                errorInfo = "卡片状态有误，无法绑定";//礼品卡状态值不符合激活操作
            }
            else if (errorCode.Equals(E1101))
            {
                errorInfo = "用户卡账户已经存在";
            }
            else if (errorCode.Equals(E1301))
            {
                errorInfo = "用户卡账户不存在";
            }
            else if (errorCode.Equals(E1302))
            {
                errorInfo = "原密码输入不正确";
            }
            else if (errorCode.Equals(E2003))
            {
                errorInfo = "礼品卡消费金额不足";
            }
            else if (errorCode.Equals(E2002))
            {
                errorInfo = "冻结解冻卡账户状态不对";
            }
            else if (errorCode.Equals("9d1dfdb4"))
            {
                errorInfo = "密码安全级别较低，请重新设置";            
            }
            else if (errorCode.Equals("b3150d97"))
            {
                errorInfo = "支付密码不可与登录密码相同";
            }
            else if (errorCode.Equals(E2100))
            {
                errorInfo = "Exception";
            }
            else if (errorCode.Equals(E2004))
            {
                errorInfo = "礼品卡退还-卡不属于用户";
            }
            else if (errorCode.Equals(E2005))
            {
                errorInfo = "礼品卡退还-没有消费记录";
            }
            else if (errorCode.Equals(E2006))
            {
                errorInfo = "礼品卡退还-退还金额不对";
            }
            else if (errorCode.Equals(E2007))
            {
                errorInfo = "礼品卡退还-不存在消费记录";
            }
            else if (errorCode.Equals(E2008))
            {
                errorInfo = "消费礼品卡状态不对";
            }
            else if (errorCode.Equals(E1007))
            {
                errorInfo = "您输入的卡片已激活，不能重复激活";
            }
            else
            {

            }
            return errorInfo;
        }
        /// <summary>
        /// 根据返回码判断调用接口是否成功
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns>返回调用接口是否成功若成功返回True否则返回False</returns>
        public static bool  IsSuccess(string errorCode)
        {
            bool success = false;
            if (errorCode.Equals(E1000))
            {
                success = true;
            }
            return success;
        }

    }
}

namespace Shangpin.Entity.User
{
    public class JsonDataFormatModel
    {
        /// <summary>
        /// 因为filter 会自动处理授权状态 所以这里只要默认授权即可
        /// </summary>
        private string _authorized = bool.TrueString;

        /// <summary>
        /// 是否授权
        /// </summary>
        public string Authorized
        {
            get { return _authorized; }
            set { _authorized = value; }
        }

        /// <summary>
        /// 只有filter会写这个property
        /// </summary>
        public string LoginUrl { get; set; }

        public dynamic UserData { get; set; }


    }
}

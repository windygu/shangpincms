using System;
using System.Collections.Generic;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item
{
    //体验报告
    public class ExperienceReportInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ProductNo { get; set; }
        public string ProductSize { get; set; }
        public string ReportContent { get; set; }
        public string LevelName { get; set; }
        public GroupUserInfo GUserInfo { get; set; }
        public string NickName { get; set; }
        public string MailAddress { get; set; }
        public int SortValue { get; set; }
        public DateTime DateCreate { get; set; }
        public SWfsUserInfo UserInfoModel { get; set; }
        public IList<SWfsExperienceReportPic> ReportPicList { get; set; }
        
    }

    public class GroupUserInfo
    {
        public string GroupName { get; set; }
        public string GroupNo { get; set; }
        public int SortValue { get; set; }
        public Int16? Gender { get; set; } 
        public SWfsUserGroup GroupInfo { get; set; }
        public string UserId { get; set; }
    }

    public class GroupUserInfoX
    {
        public string GroupName { get; set; }
        public string GroupNo { get; set; }
        public int SortValue { get; set; }
        public Int16? Gender { get; set; }
        public string UserId { get; set; }
    }


    public class ExperienceReportInfoX
    {
        public DateTime DateCreateX { get; set; }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ProductNo { get; set; }
        public string ProductSize { get; set; }
        public string ReportContent { get; set; }
        public string LevelName { get; set; }
        public string GroupName { get; set; }
        public string GroupNo { get; set; }
        public int SortValue { get; set; }
        public Int16? Gender { get; set; } 
        public string NickName { get; set; }
        public string MailAddress { get; set; }
        public DateTime DateCreate { get; set; }
        public Int32 UserInfoId { get; set; }
        public String Height { get; set; }
        public String Weight { get; set; }
        public String Bust { get; set; }
        public String Waistline { get; set; }
        public String Hips { get; set; }
        public String Footlength { get; set; }
        public String FootWidth { get; set; }
        public String CalfSize { get; set; }
        public Int16 FootType { get; set; }
        public String ShoeSize { get; set; }
        public Int16 BodyType { get; set; }
        public String Avatar { get; set; }
        public Int16 AuditStatus { get; set; }
        public DateTime AvatarCreateDate { get; set; }
        public Int16 IsDelete { get; set; }
        public String FootSize { get; set; }
        public Int16 PicType { get; set; }
        public Int16 BindStatus { get; set; }
        public Int16 SyncStatus { get; set; }
        public String WeiBoUserId { get; set; }
        public String ShoulderWidth { get; set; }
        public string PictureFileNo { get; set; }
    }

    public class EPCount
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 对应的条数
        /// </summary>
        public string Count { get; set; }
    }

}

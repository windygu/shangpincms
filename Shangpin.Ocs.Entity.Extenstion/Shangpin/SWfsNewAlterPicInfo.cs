using Shangpin.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 上新活动图实体
    /// </summary>
    public class SWfsNewAlterPicInfo : PagingEntityBase
    {
        public int PictureId { get; set; }
        public string PictureName { get; set; }//图片名称
        public string PcPictureLinkUrl { get; set; }//主站图片链接
        public string MobilePictureLinkUrl { get; set; }//移动端图片链接
        public string BrandNo { get; set; }//品牌编号
        public string BrandEnName { get; set; }//品牌英文名称
        public string BrandCnName { get; set; }//品牌中文名称
        public int Position { get; set; }//轮播位
        public string PcPictureNo { get; set; }//主站图片
        public string MobilePictureNo { get; set; }//移动端图片
        public int MobilePictureType { get; set; }//移动端图片类型
        public short Status { get; set; }//图片状态
        public short DataStatus { get; set; }//数据状态
        public DateTime BeginDate { get; set; }//开始时间
        public string OperatorUserId { get; set; }//创建人
        public DateTime DateCreate { get; set; }//创建时间
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using System.IO;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Text.RegularExpressions;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Xml;
using Shangpin.Ocs.Service.Common;
using Shangpin.Framework.Common.Dapper;
using System.Web;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsFlagShipModuleLinkService
    {
        /// <summary>
        /// 返回空楼层内容对象
        /// </summary>
        /// <returns></returns>
        public SwfsFlagShipModuleLink CreateEmptySwfsFlagShipModuleLink()
        {
            return new SwfsFlagShipModuleLink { DataCreate = DateTime.Now, State = 1, OperatorUserId = PresentationHelper.GetPassport().UserName, DataState = 1, CategoryNo = "", PictureNo = "", LinkUrl = "", LinkTarget = "_blank" };
        }

        /// <summary>
        /// 根据楼层ID获取楼层内容 
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public List<SwfsFlagShipModuleLink> GetAllSwfsFlagShipModuleLinkByModuleId(int ModuleId)
        {
            List<SwfsFlagShipModuleLink> links = null;
            links = DapperUtil.Query<SwfsFlagShipModuleLink>("ComBeziWfs_SwfsFlagShipModuleLink_GetAllSwfsFlagShipModuleLinkByModuleId", new { ModuleId = ModuleId }).FilterList();

            return links ?? new List<SwfsFlagShipModuleLink>();
        }
        /// <summary>
        /// 根据ID获取SwfsFlagShipModuleLink
        /// </summary>
        /// <param name="LinkId"></param>
        /// <returns></returns>
        public SwfsFlagShipModuleLink GetSwfsFlagShipModuleLinkById(int LinkId)
        {

            return DapperUtil.QueryByIdentityWithNoLock<SwfsFlagShipModuleLink>(LinkId);
        }

        /// <summary>
        /// 保存或更新楼层内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SwfsFlagShipModuleLink InsertOrUpdateSwfsFlagShipModuleLink(SwfsFlagShipModuleLink model)
        {
            if (model.LinkId == 0)
            {
                model.LinkId = DapperUtil.Insert<SwfsFlagShipModuleLink>(model, true);
            }
            else
            {
                DapperUtil.Update<SwfsFlagShipModuleLink>(model);
            }
            return model;
        }
        /// <summary>
        /// 配置楼层大小,名称
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPicSizeAndTip()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["pic1"] = "width:258px;height:550px;";
            dic["pic1Name"] = "服装楼层大图尺寸";
            dic["pic1Check"] = "width:258,Height:550,Length:150";
            dic["pic1Tip"] = "尺寸：258*550;格式：jpg ;大小：150K以下";
            dic["pic1Show"] = "width=258&height=550";
            ////
            dic["pic2"] = "width:258px;height:275px;";
            dic["pic2Name"] = "配饰楼层图片尺寸上";
            dic["pic2Check"] = "width:258,Height:275,Length:150";
            dic["pic2Tip"] = "尺寸：258*275;格式：jpg ;大小：150K以下";
            dic["pic2Show"] = "width=258&height=275";
            ///////
            dic["pic3"] = "width:258px;height:275px;";
            dic["pic3Name"] = "配饰楼层图片尺寸下";
            dic["pic3Check"] = "width:258,Height:275,Length:150";
            dic["pic3Tip"] = "尺寸：258*275;格式：jpg ;大小：150K以下";
            dic["pic3Show"] = "width=258&height=275";
            return dic;
        }
        public Dictionary<int, List<SwfsFlagShipModuleLink>> GetFloorLinkDic(int[] floorIds)
        {
            Dictionary<int, List<SwfsFlagShipModuleLink>> dic = new Dictionary<int, List<SwfsFlagShipModuleLink>>();
            foreach (int id in floorIds)
            {
                dic.Add(id, GetAllSwfsFlagShipModuleLinkByModuleId(id) ?? new List<SwfsFlagShipModuleLink>());
            }
            return dic;
        }
        #region 图片处理
        public Dictionary<string, string> SaveImg(HttpPostedFileBase file, string imgInfo)
        {
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            CommonService commonService = new CommonService();
            if (file.ContentLength > 0)
            {
                rsPic = new CommonService().PostImgCustomType(file, imgInfo, new string[] { ".jpg", ".jpeg" }, 2);
            }
            return rsPic;
        }
        #endregion
    }
}

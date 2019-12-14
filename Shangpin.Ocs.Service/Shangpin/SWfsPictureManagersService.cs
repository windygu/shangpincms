using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsPictureManagersService
    {
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="sWfsPictureManager"></param>
        /// <returns></returns>
        public int AddSWfsPictureManager(SWfsPictureManager sWfsPictureManager)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsPictureManager_Add", new
            {
                @PictureName=" ",
                @PictureFileNo = sWfsPictureManager.PictureFileNo,
                @LinkAddress = sWfsPictureManager.LinkAddress,
                @WebSite = 1,
                @Status = 0,
                @Position = 255,
                @OperatorUserId = sWfsPictureManager.OperatorUserId,
                @PagePosition = sWfsPictureManager.PagePosition,
                @DateCreate=DateTime.Now,
                @SubTitle=" ",
            });
        }

        /// <summary>
        /// 删除原来的图片
        /// </summary>
        /// <param name="pagePosition">分类编号</param>
        /// <returns></returns>
        public int DeleteSWfsPictureManager(string pagePosition)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsPictureManager_Delete", new { PagePosition = pagePosition });
        }

    }
}

using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsIndexNewArrivalProductListService
    {
        /// <summary>
        /// 执行添加上新信息
        /// </summary>
        /// <param name="sWfsIndexNewArrival">SWfsIndexNewArrival实体</param>
        /// <returns>返回主键id</returns>
        public int AddSWfsIndexNewArrivalProductList(SWfsIndexNewArrivalProductList sWfsIndexNewArrivalProductList)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrivalProductList_Add", new
            {
                ProductNo = sWfsIndexNewArrivalProductList.ProductNo,
                NewArrivalId = sWfsIndexNewArrivalProductList.NewArrivalId,
                SortValue = sWfsIndexNewArrivalProductList.SortValue,
                CreateDate = DateTime.Now,
                DataState = 0,
                OperateUserId = sWfsIndexNewArrivalProductList.OperateUserId,

            });
        }

        /// <summary>
        /// 删除要上新的产品
        /// </summary>
        /// <param name="productno">产品编号</param>
        /// <param name="newarrayid">上新编号</param>
        /// <returns></returns>
        public int DelSWfsIndexNewArrivalProductListGoods(string productno,string newarrayid)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrivalProductList_DeleteGoods", new { ProductNo = productno, NewArrivalId = newarrayid });
        }


        /// <summary>
        /// 添加要上新的产品（判断重复）
        /// </summary>
        /// <param name="productno">产品编号</param>
        /// <param name="newarrayid">上新编号</param>
        /// <returns></returns>
        public int AddSWfsIndexNewArrivalProductListGoods(string productno, string newarrayid)
        {
           var count=  DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrivalProductList_DeleteGoods", new { ProductNo = productno, NewArrivalId = newarrayid }).FirstOrDefault();

           return count;
        }


        /// <summary>
        /// 重新排序
        /// </summary>
        /// <param name="productno">产品编号</param>
        /// <param name="newarrayid">上新编号</param>
        /// <param name="sort">排序编号</param>
        /// <returns></returns>
        public int UpdateSortSWfsIndexNewArrivalProductListGoods(string productno, string newarrayid,int sort)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrivalProductList_UpdateSort", new { ProductNo = productno, NewArrivalId = newarrayid, SortValue = sort });
        }

    }
}

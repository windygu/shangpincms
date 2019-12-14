using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Dapper;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsIndexHotProductListTempService
    {
        /// <summary>
        /// 根据不同类别获取热门商品信息
        /// </summary>
        /// <param name="categroyNo"></param>
        /// <returns></returns>
        public List<IndexHotProductInfo> GetIndexHotProudecList(string categroyNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CategoryNo", categroyNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<IndexHotProductInfo> list= DapperUtil.Query<IndexHotProductInfo>("ComBeziWfs_SWfsIndexHotProductListTemp_GetIndexHotProudecList", dp).ToList();
            if (list == null)
            {
                list = new List<IndexHotProductInfo>();
            }
            return list;
        }
        /// <summary>
        /// 根据商品编号获取sku价格信息
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public List<IndexHotProductInfo> GetSkuPriceListByProductNo(string productNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("ProductNo", productNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<IndexHotProductInfo> list = DapperUtil.Query<IndexHotProductInfo>("ComBeziWfs_SWfsIndexHotProductListTemp_GetIndexHotSkutListByProduct", dp).ToList();
            if (list == null)
            {
                list = new List<IndexHotProductInfo>();
            }
            return list;
        }
        /// <summary>
        /// 修改热门商品排序值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortValue"></param>
        /// <returns></returns>
        public bool UpdateHotProductSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsIndexHotProductListTemp>(new
            {
                ID = id,
                SortValue = sortValue
            });
        }
        /// <summary>
        /// 删除热门商品信息
        /// </summary>
        /// <param name="idstr"></param>
        public void DelHotProductProduct(string idstr)
        {
            foreach (string id in idstr.Split(','))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("ID", id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                DapperUtil.Execute("ComBeziWfs_SWfsIndexHotProductListTemp_DelHotProductProduct", dp);
            }
        }
        /// <summary>
        /// 查询热门商品列表信息
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="brandNO"></param>
        /// <param name="categoryNo"></param>
        /// <param name="keyword"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IEnumerable<IndexHotProductInfo> GetSWfsProductList(string gender, string brandNO, string categoryNo, string keyword, string starttime, string endtime, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("StartDateShelf", starttime == null ? "" : starttime);
            dic.Add("EndDateShelf", endtime == null ? "" : endtime);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProduct_SelectProductCount", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, StartDateShelf = starttime, EndDateShelf = endtime }).FirstOrDefault();
            return DapperUtil.Query<IndexHotProductInfo>("ComBeziWfs_SWfsProduct_SearchHotProductList", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, pageIndex = pageIndex, pageSize = pageSize });
        }

        /// <summary>
        /// 添加某类别下的热门商品
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="productNoStr"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int AddHotProduct(string categoryNo, string productNoStr, string userid)
        {
            string[] pNoList = productNoStr.Trim().TrimEnd(',').Split(',');
            StringBuilder str = new StringBuilder();
            int count = NewProductMaxSort(categoryNo);//找出最大排序值
            for (int i = 0; i < pNoList.Length; i++)
            {
                int sort = count + pNoList.Length - i;//当前添加的商品的排序值，倒叙
                str.Append("insert into SWfsIndexHotProductListTemp values('" + categoryNo +"','" + pNoList[i] + "'," + sort + ",1,1,'" + System.DateTime.Now + "','" + userid + "','" + userid + "','" + System.DateTime.Now + "');");
            }
            return InsertNewProduct(str.ToString());
        }
        public int InsertNewProduct(string sqlstrs)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sqlstrs);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsIndexHotProductListTemp_InsertProducts", dic).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取某类别下热门商品的最大排序值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int NewProductMaxSort(string id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CategoryNo", id, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<SWfsIndexHotProductListTemp> list = DapperUtil.Query<SWfsIndexHotProductListTemp>("ComBeziWfs_SWfsIndexHotProductListTemp_SelectMaxSort", dp).ToList();
            if (list != null && list.Count() > 0)
            {
                return list.Select(a => a.SortValue).Max();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据类别编号和商品编号获取热门商品信息
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public List<SWfsIndexHotProductListTemp> GetIndexHotProductByProductNo(string categoryNo, string productNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CategoryNo", categoryNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("ProductNo", productNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<SWfsIndexHotProductListTemp> list = DapperUtil.Query<SWfsIndexHotProductListTemp>("ComBeziWfs_SWfsIndexHotProductListTemp_GetIndexHotProductByProductNo", dp).ToList();
            if (list == null)
            {
                list = new List<SWfsIndexHotProductListTemp>();
            }
            return list;
        }
    }
}

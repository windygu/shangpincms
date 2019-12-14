using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateAoLaiSubjectDiscountInfo.Comm;
using CalculateAoLaiSubjectDiscountInfo.DAL;
using CalculateAoLaiSubjectDiscountInfo.Model;
using log4net;

namespace CalculateAoLaiSubjectDiscountInfo.BLL
{
    public class CalculationBLL
    {
        private static ILog log = LogManager.GetLogger("CalculateAoLaiSubjectDiscountInfo_CreateDiscountLogFile_Logger");

        const string sqlSelect = "SELECT DISTINCT {0} FROM SpProduct ( NOLOCK )  AS sp INNER JOIN SpfProductExtend ( NOLOCK ) AS spextend  ON spextend.ProductNo = sp.ProductNo LEFT JOIN SpfSkuExtend AS sku ( NOLOCK ) ON sp.ProductNo = sku.ProductNo LEFT JOIN SpfSkuPrice AS skuPri ( NOLOCK ) ON sku.SkuNo = skuPri.SkuNo" +
 " WHERE sp.ProductNo IN ( SELECT spref.ProductNo  FROM  dbo.SWfsSubjectCategory AS sc ,SWfsSubjectProductRef AS spref" +
 " WHERE sc.CategoryNo=spref.CategoryNo AND SubjectNo='{1}'" +
 " and spref.TypeFlag=0 " +
 " and spref.IsShow=1 " +
 " and spref.ShowTime <= getdate()" +
 ") " +
 "  AND sku.PcShowState = 1 " +
 " AND spextend.TypeFlag = 2 " +
 " AND spextend.[Status] = 1 " +
 " AND spextend.DataState = 1 " +
  " AND sku.DisabledState = 0 " +
  " AND sku.PcSaleState = 2 " +
 " ORDER BY {2} ASC ";

        const string sqlUpdate = "update SWfsSubject set DiscountType={0},DiscountTypeValue={1} where SubjectNo='{2}'";

        public static void Run()
        {
            //查询符合计算条件的专题
            //根据专题折扣类型计算价格或打折信息
            //更新相应的计算结果
            IList<SubModel> list = GetList();
            Run(list);
        }

        private static IList<SubModel> GetList()
        {
            IList<SubModel> list = new List<SubModel>();
            string sql = "select SubjectNo,SubjectName,DiscountType from SWfsSubject ( NOLOCK ) where Status=1 and DateEnd > getdate()";
            SqlDataReader reader = SqlHelper.ExecuteReader(sql);
            while (reader.Read())
            {
                SubModel model = new SubModel();
                model.SubjectNo = reader["SubjectNo"].ToString();
                model.SubjectName = reader["SubjectName"].ToString();
                model.DiscountType = reader["DiscountType"].ToString();
                list.Add(model);
            }
            return list;
        }

        private static void RunOld(IList<SubModel> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    string discountType = "1";
                    string discountTypeValue = "0";
                    string info = string.Empty;
                    string sql = string.Empty;
                    try
                    {
                        if (item.DiscountType.Equals("1") || item.DiscountType.Equals("3")) //1 xx折起    
                        {
                            sql = string.Format(sqlSelect, " CAST(( skuPri.OutletPrice / CASE WHEN skuPri.MarketPrice = 0 THEN 1  END ) * 10 AS DECIMAL(38, 1)) AS DiscountRate ", item.SubjectNo, " DiscountRate ");
                            DataTable dt = SqlHelper.QueryTable(sql);
                            if (dt != null && dt.Rows.Count == 1)
                            {
                                discountType = "3";//全场XX折
                            }
                            if (dt.Rows.Count > 0)
                            {
                                discountTypeValue = dt.Rows[0]["DiscountRate"].ToString();
                            }
                            discountTypeValue = string.IsNullOrWhiteSpace(discountTypeValue) ? "0" : discountTypeValue;
                            info = "全场" + (discountType.Equals("1") ? discountTypeValue + "折起" : discountTypeValue + "折");
                        }
                        else
                        {
                            sql = string.Format(sqlSelect, "cast(skuPri.OutletPrice as  decimal(38,0)) as LimitedVipPrice", item.SubjectNo, " LimitedVipPrice ");
                            DataTable dt = SqlHelper.QueryTable(sql);
                            if (dt != null && dt.Rows.Count == 1)
                            {
                                discountType = "4";//全场XXX元
                            }
                            else
                            {
                                discountType = "5"; //全场XXX元起售
                            }
                            if (dt.Rows.Count > 0)
                            {
                                discountTypeValue = dt.Rows[0]["LimitedVipPrice"].ToString();
                            }
                            discountTypeValue = string.IsNullOrWhiteSpace(discountTypeValue) ? "0" : discountTypeValue;
                            info = "全场" + (discountType.Equals("4") ? discountTypeValue : discountTypeValue + "起售");
                        }
                        sql = string.Format(sqlUpdate, discountType, discountTypeValue, item.SubjectNo);
                        SqlHelper.ExecuteSql(sql);
                        Console.WriteLine(item.SubjectNo + ":" + item.SubjectName + ":" + info);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Log("Calculation", ex.ToString());
                        continue;
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                Console.WriteLine("No Data  Wash Wash Sleep ...");
            }
        }

        private static void Run(IList<SubModel> list)
        {
            List<SubjectProductM> plist = new List<SubjectProductM>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    string discountType = "1";
                    string discountTypeValue = "0";
                    string info = string.Empty;
                    string sql = string.Empty;
                    try
                    {
                        if (item.DiscountType.Equals("1") || item.DiscountType.Equals("3")) //1 xx折起    
                        {
                            plist = GetSubjectProductData(item.SubjectNo, 1);
                            if (plist != null && plist.Count() == 1)
                            {
                                discountType = "3";//全场XX折
                            }
                            if (plist.Count() > 0)
                            {
                                discountTypeValue = plist[0].DiscountRate.ToString("0.000");
                            }
                            discountTypeValue = string.IsNullOrWhiteSpace(discountTypeValue) ? "0" : discountTypeValue;
                            info = "全场" + (discountType.Equals("1") ? discountTypeValue + "折起" : discountTypeValue + "折");
                        }
                        else
                        {
                            plist = GetSubjectProductData(item.SubjectNo, 2);
                            if (plist != null && plist.Count() == 1)
                            {
                                discountType = "4";//全场XXX元
                            }
                            else
                            {
                                discountType = "5"; //全场XXX元起售
                            }
                            if (plist.Count() > 0)
                            {
                                discountTypeValue = plist[0].OutletPrice.ToString();
                            }
                            discountTypeValue = string.IsNullOrWhiteSpace(discountTypeValue) ? "0" : discountTypeValue;
                            info = "全场" + (discountType.Equals("4") ? discountTypeValue : discountTypeValue + "起售");
                        }
                        sql = string.Format(sqlUpdate, discountType, discountTypeValue, item.SubjectNo);
                        SqlHelper.ExecuteSql(sql);
                        Console.WriteLine(item.SubjectNo + ":" + item.SubjectName + ":" + info);
                    }
                    catch (Exception ex)
                    {
                        log.Error("ERROR:计算折扣，异常信息:" + ex.Message);
                        continue;
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                Console.WriteLine("No Data  Wash Wash Sleep ...");
            }
        }

        /// <summary>
        /// 根据活动编号获取活动下所有有效商品
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="type">1折扣 2元</param>
        /// <returns></returns>
        private static List<SubjectProductM> GetSubjectProductData(string subjectNo, int type)
        {
            List<SubjectProductM> productList = new List<SubjectProductM>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pe.ProductNo ");
            sb.Append(" FROM SWfsSubjectProductRef (NOLOCK) AS pr  ");
            sb.Append(" INNER JOIN dbo.SpfProductExtend (NOLOCK) AS pe ON pr.ProductNo = pe.ProductNo ");
            sb.Append(" WHERE pe.DataState = 1 ");
            sb.Append(" AND pe.[Status] = 1 AND pe.TypeFlag = 2 AND pe.IsOutside = 1 AND pr.TypeFlag = 0 ");
            sb.Append(" AND pr.IsShow = 1  AND pr.ShowTime <= GETDATE()");
            sb.Append(" AND ( SELECT COUNT(1) FROM dbo.SpfSkuExtend(NOLOCK) WHERE pr.ProductNo = ProductNo  ");
            sb.Append(" AND DataState = 1 AND DisabledState = 0 AND PcSaleState = 2 AND PcShowState = 1 ) >= 1 ");
            sb.Append(" AND EXISTS (SELECT 1 FROM dbo.SWfsSubjectCategory(NOLOCK)  WHERE  pr.CategoryNo = CategoryNo AND SubjectNo = '{0}' ) ");
            sb.Append(" ORDER BY pr.SortNo ASC , pr.DateCreate DESC");
            string sql = string.Format(sb.ToString(), subjectNo);
            try
            {
                DataTable dt = SqlHelper.QueryTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        SubjectProductM p = new SubjectProductM();
                        p.ProductNo = dt.Rows[i][0].ToString();
                        productList.Add(p);
                    }

                    #region 获取SKU最低奥莱价格
                    List<SpfSkuPriceModel> skuPriceData = new List<SpfSkuPriceModel>();
                    List<string> productArrayNo = new List<string>();
                    productArrayNo = (from p in productList select p.ProductNo).Distinct().ToList();
                    if (productArrayNo != null && productArrayNo.Count() > 0)
                    {
                        #region 最低价格
                        skuPriceData = GetSubjectProductSkuPrice(productArrayNo);
                        #endregion
                    }
                    #endregion

                    foreach (var item in productList)
                    {
                        #region 获取SKU最低奥莱价格
                        if (skuPriceData != null && skuPriceData.Count() > 0)
                        {
                            SpfSkuPriceModel priceModel = skuPriceData.Where(a => a.ProductNo.Equals(item.ProductNo)).FirstOrDefault();
                            if (priceModel != null)
                            {
                                decimal marketprice = priceModel.MarketPrice == 0 ? 1 : priceModel.MarketPrice;
                                item.MarketPrice = priceModel.MarketPrice;
                                item.OutletPrice = priceModel.OutletPrice;
                                item.DiscountRate = (priceModel.OutletPrice / marketprice) * 10;
                            }
                        }
                        #endregion
                    }

                    if (productList != null && productList.Count() > 0)
                    {
                        if (type == 1)
                        {
                            productList = productList.Distinct(new ComparerSubjectProduct()).OrderBy(o => o.DiscountRate).ToList();
                        }
                        else if (type == 2)
                        {
                            productList = productList.Distinct(new ComparerProductData()).OrderBy(o => o.OutletPrice).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR:根据活动编号获取活动下所有有效商品:" + ex.Message);
            }
            return productList;
        }

        /// <summary>
        /// 根据商品编号获取SKU中最低的价格
        /// </summary>
        /// <param name="productNo">商品编号，多个以逗号隔开</param>
        /// <returns></returns>
        public static List<SpfSkuPriceModel> GetSubjectProductSkuPrice(List<string> productNo)
        {
            List<SpfSkuPriceModel> list = new List<SpfSkuPriceModel>();
            StringBuilder pNo = new StringBuilder();
            if (productNo != null && productNo.Count() > 0)
            {
                int num = 0;
                foreach (var item in productNo)
                {
                    num++;
                    pNo.Append("'" + item + "'");
                    if (num < productNo.Count())
                    {
                        pNo.Append(",");
                    }
                }
            }
            if (pNo != null)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT se.ProductNo,se.SkuNo ,sp.MarketPrice ,sp.StandardPrice ,sp.GoldPrice ,sp.PlatinumPrice ,   ");
                sql.Append("sp.DiamondPrice ,sp.OutletPrice ,se.IsPromotion,sp.PromotionPrice ,sp.DiscountShangpin ,sp.DiscountOutlet,se.PcSaleState   ");
                sql.Append("FROM   dbo.SpfSkuExtend(NOLOCK) AS se   ");
                sql.Append("INNER JOIN dbo.SpfSkuPrice(NOLOCK) AS sp ON se.SkuNo = sp.SkuNo ");
                sql.Append("WHERE  se.DataState = 1 AND se.DisabledState = 0 AND se.PcSaleState IN (2,3) AND se.PcShowState = 1 AND se.ProductNo IN ({0})  ");
                sql.Append("ORDER BY sp.OutletPrice ASC,se.SkuId DESC");
                string tempSql = string.Format(sql.ToString(), pNo);
                try
                {
                    DataTable dt = SqlHelper.QueryTable(tempSql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            SpfSkuPriceModel m = new SpfSkuPriceModel();
                            m.ProductNo = dt.Rows[i]["ProductNo"].ToString();
                            m.SkuNo = dt.Rows[i]["SkuNo"].ToString();
                            m.MarketPrice = Convert.ToDecimal(dt.Rows[i]["MarketPrice"].ToString());
                            m.StandardPrice = Convert.ToDecimal(dt.Rows[i]["StandardPrice"].ToString());
                            m.OutletPrice = Convert.ToDecimal(dt.Rows[i]["OutletPrice"].ToString());
                            m.PcSaleState = Convert.ToInt32(dt.Rows[i]["PcSaleState"].ToString());
                            list.Add(m);
                        }
                    }
                    if (list != null && list.Count() > 0)
                    {
                        return list;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("ERROR:根据活动编号获取活动下所有有效商品:" + ex.Message);
                }
            }
            return null;
        }

    }

    #region 比较
    /// <summary>
    /// 排除重复项
    /// </summary>
    public class ComparerSubjectProduct : IEqualityComparer<SubjectProductM>
    {
        public bool Equals(SubjectProductM t1, SubjectProductM t2)
        {
            return (t1.DiscountRate == t2.DiscountRate);
        }
        public int GetHashCode(SubjectProductM t)
        {
            return t.ToString().GetHashCode();
        }
    }
    /// <summary>
    /// 排除重复项
    /// </summary>
    public class ComparerProductData : IEqualityComparer<SubjectProductM>
    {
        public bool Equals(SubjectProductM t1, SubjectProductM t2)
        {
            return (t1.OutletPrice == t2.OutletPrice);
        }
        public int GetHashCode(SubjectProductM t)
        {
            return t.ToString().GetHashCode();
        }
    }
    #endregion
}

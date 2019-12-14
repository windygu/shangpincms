using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsCategoryBrandAliasService
    {
        #region Brand
        public IEnumerable<BrandExtendForAlias> GetAllBrand(int pageIndex, int pageSize, string brandName, string aliasName, out int count)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("BrandName", brandName ?? "");
            dic.Add("AliasName", aliasName ?? "");
            count = DapperUtil.Query<int>("ComBeziWfs_WfsBrand_AliasListCount", dic, new { BrandName = brandName, AliasName = aliasName }).First<int>();
            return DapperUtil.QueryPaging<BrandExtendForAlias>("ComBeziWfs_WfsBrand_AliasList", pageIndex, pageSize, "AliasOrder,brandno  ASC", dic, new { BrandName = brandName, AliasName = aliasName });
        }

        public IEnumerable<BrandExtendForAlias> GetNoBrandAlias(int pageIndex, int pageSize, out int count)
        {
            count = DapperUtil.Query<int>("ComBeziWfs_WfsBrand_NoBrandAliasCount").First<int>();
            return DapperUtil.QueryPaging<BrandExtendForAlias>("ComBeziWfs_WfsBrand_NoBrandAliasList", pageIndex, pageSize, "brandno  ASC");
        }

        public BrandExtendForAlias GetBrandById(int id)
        {
            return DapperUtil.QueryByIdentity<BrandExtendForAlias>(id);
        }

        public int DeleteArias(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsCategoryBrandAlias_DeleteById", new { KeyId = id });
        }

        public int SaveArias(SWfsCategoryBrandAlias alias)
        {
            SWfsCategoryBrandAlias result0 = null;
            if (alias.TypeID == 1)
            {
                result0 = DapperUtil.Query<SWfsCategoryBrandAlias>("ComBeziWfs_SWfsCategoryBrandAlias_CategoryList",
               new { ObjectAlias = alias.ObjectAlias, Gender = alias.Gender, TypeID = alias.TypeID }).FirstOrDefault();
            }
            else
            {
                result0 = DapperUtil.Query<SWfsCategoryBrandAlias>("ComBeziWfs_SWfsCategoryBrandAlias_BrandList",
              new { ObjectAlias = alias.ObjectAlias, Gender = alias.Gender, TypeID = alias.TypeID }).FirstOrDefault();
            }
            if (result0 != null && result0.ObjectNo != alias.ObjectNo && !string.IsNullOrEmpty(alias.ObjectAlias))
                return -1;
            //查询是否存在
            var result = DapperUtil.Query<SWfsCategoryBrandAlias>("ComBeziWfs_SWfsCategoryBrandAlias_List",
                new { ObjectNo = alias.ObjectNo, Gender = alias.Gender, TypeID = alias.TypeID }).FirstOrDefault();
            if (result != null)
            {
                //修改
                alias.KeyID = result.KeyID;
                return DapperUtil.Update<SWfsCategoryBrandAlias>(alias) ? 1 : 0;//修改发布数据
            }
            else
                return DapperUtil.Insert<SWfsCategoryBrandAlias>(alias, true);//增加发布数据

        }
        #endregion

        #region Category
        public IEnumerable<CategoryExtendForAlias> GetAllCategory(int pageIndex, int pageSize, string categoryNo, string categoryName, string aliasName, out int count)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("CategoryName", categoryName ?? "");
            dic.Add("AliasName", aliasName ?? "");
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsCategory_AliasListCount", dic, new { categoryNo = categoryNo, CategoryName = categoryName, AliasName = aliasName }).First<int>();
            return DapperUtil.Query<CategoryExtendForAlias>("ComBeziWfs_SWfsCategory_AliasList", dic, new { prePage = (pageIndex - 1) * pageSize + 1, nextPage = pageIndex * pageSize, categoryNo = categoryNo, CategoryName = categoryName, AliasName = aliasName });
        }

        public IEnumerable<CategoryExtendForAlias> GetNoCategoryAlias(int pageIndex, int pageSize, int gender, out int count)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsCategory_NoCategoryAliasListCount", dic, new { Gender = gender }).First<int>();
            return DapperUtil.Query<CategoryExtendForAlias>("ComBeziWfs_SWfsCategory_NoCategoryAliasList", dic, new { prePage = (pageIndex - 1) * pageSize + 1, nextPage = pageIndex * pageSize, Gender = gender });
        }

        #endregion

    }
}

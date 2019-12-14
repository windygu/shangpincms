using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class AppIndexService
    {

        #region 轮播图管理
        public SWfsNewSubject GetSWfsNewSubjectById(string SubjectNo)
        {
            object param = new { SubjectNo = SubjectNo };
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_GetSWfsNewSubjectById", param).FirstOrDefault();
        }
        public int InsertAppAlterPic(SWfsAppAlterPic entity)
        {
            return DapperUtil.Insert<SWfsAppAlterPic>(entity, false);
        }
        public bool UpdateAppAlterPic(SWfsAppAlterPic entity)
        {
            return DapperUtil.UpdatePartialColumns<SWfsAppAlterPic>(new 
            {
                AppSlterPicId = entity.AppSlterPicId,
                Name=entity.Name,
                PicNo=entity.PicNo,
                RefContent=entity.RefContent,
                AlterType=entity.AlterType,
                RefType = entity.RefType,
                Sort=entity.Sort,
                StartTime = entity.StartTime
            });
        }
        public int DelAppAlterPic(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsAppAlterPic_DelSWfsAppAlterPicById", new { AppSlterPicId = id });
        }
        public SWfsAppAlterPic GetAppAlterPicById(string id)
        {
            return DapperUtil.Query<SWfsAppAlterPic>("ComBeziWfs_SWfsAppAlterPic_GetSWfsAppAlterPicById", new { AppSlterPicId = id }).FirstOrDefault();
        }
        public IEnumerable<SWfsAppAlterPic> AlterLists(int pageIndex, int pageSize, Dictionary<string, object> dic, out int count)
        {
            IEnumerable<SWfsAppAlterPic> list = DapperUtil.Query<SWfsAppAlterPic>("ComBeziWfs_SWfsAppAlterPic_GetSWfsAppAlterPicList", dic, new { AlterType = dic["AlterType"], Name = dic["Name"]+"", RefType = dic["RefType"], Sort = dic["Sort"], beginTime = dic["beginTime"]+"", endTime = dic["endTime"]+"", pageIndex = pageIndex, pageSize = pageSize }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsAppAlterPic_GetSWfsAppAlterPicCount", dic, new { AlterType = dic["AlterType"], Name = dic["Name"]+"", RefType = dic["RefType"], Sort = dic["Sort"], beginTime = dic["beginTime"]+"", endTime = dic["endTime"]+"", pageIndex = pageIndex, pageSize = pageSize }).First<int>();
            return list;
        }
        //查询品牌名称
        public string GetBrandName(string brandNo)
        {
            if (string.IsNullOrEmpty(brandNo))
            {
                return "";
            }
            return DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetAppBrandNameByBrandNo", new { BrandNo = brandNo }).FirstOrDefault();
        }
        public SWfsNewSubject GetSubjectByNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsAppAlterPic_GetAppSubjectByNo", new 
            {
                SubjectNo=subjectNo
            }).FirstOrDefault();
        }
        //验证轮播图是否重复  开始时间是否重复
        public int IsExistSameTimeAlter(string startTime,int sort)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsAppAlterPic_IsExistsSameTimeAlterPic", new 
            {
                StartTime = startTime,
                Sort = sort
            }).FirstOrDefault();
        }
        #endregion


        #region 榜单管理

        //SWfsAppShopCategory

        public void aa(string namea)
        {
            //查询
            Dictionary<string, object> dic = new Dictionary<string, object>();//用来拼接sql语句
            dic.Add("bbb", "0000");
            DapperUtil.Query<string>("ComBeziWfs_Brand_get", dic, new { name = namea });
            DapperUtil.Execute("bbb", dic);//执行sql语句
            DapperUtil.Insert<SWfsProductLabel>(new SWfsProductLabel());//插入数据
            DapperUtil.UpdatePartialColumns<SWfsProductLabel>(new //修改部分字段 到数据库
            {
                LabelId = 12,//第一个一定放主键
                name = '"'
            });
        }

        #endregion


        #region 搜索页商城分类  运营位管理

        /// <summary>
        /// 得到搜索页商城分类全部列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SWfsAppShopCategory> GetAppShopCategoryList()
        {
            return DapperUtil.Query<SWfsAppShopCategory>("ComBeziWfs_SWfsAppShopCategory_GetAppShopCategoryList");
        }

        /// <summary>
        /// 添加/修改搜索页商城分类
        /// 指定位置不存在的insert，存在的update
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateAppShopCategory(SWfsAppShopCategory obj)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsAppShopCategory_Update", new { Sort = obj.Sort, CategoryNo = obj.CategoryNo, PicNo = obj.PicNo, CreateDate = obj.CreateDate });
        }

        public void DeleteAppShopCategoryBySorts(params int[] sorts)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsAppShopCategory_DeleteBySorts", new { Sorts = sorts });
        }

        /// <summary>
        /// 通过位置号sort得到一条记录
        /// </summary>
        /// <returns></returns>
        public SWfsAppOperatingPosition GetAppOperatingPositionBySort(int sort)
        {
            return DapperUtil.Query<SWfsAppOperatingPosition>("ComBeziWfs_SWfsAppOperatingPosition_GetOperatingPositionBySort", new { Sort = sort }).FirstOrDefault();
        }

        /// <summary>
        /// 得到搜索页运营位全部列表
        /// </summary>
        /// <returns></returns>
        public SWfsAppOperatingPosition GetAppOperatingPositionOne()
        {
            return DapperUtil.Query<SWfsAppOperatingPosition>("ComBeziWfs_SWfsAppOperatingPosition_GetOneOperatingPosition").FirstOrDefault();
        }

        /// <summary>
        /// 分页得到搜索页运营位全部列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<SWfsAppOperatingPosition> GetAppOperatingPositionList(int pageIndex, int pageSize, out int count)
        {
            IEnumerable<SWfsAppOperatingPosition> list=DapperUtil.Query<SWfsAppOperatingPosition>("ComBeziWfs_SWfsAppOperatingPosition_GetList", new { pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsAppOperatingPosition_GetCount").First<int>();
            return list;
        }

        /// <summary>
        /// 得到搜索页运营位最大的位置号
        /// </summary>
        /// <returns></returns>
        public int GetAppOperatingPositionMaxSort()
        {
            if (DapperUtil.Query<int>("ComBeziWfs_SWfsAppOperatingPosition_GetCount").First<int>() > 0)
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsAppOperatingPosition_GetMaxSort").First<int>();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加/修改搜索页运营位
        /// 指定位置不存在的insert，存在的update
        /// </summary>
        /// <param name="obj"></param>
        public int UpdateAppOperatingPosition(SWfsAppOperatingPosition obj)
        {
            
            if (obj.OperatingPositionId==0)
            {
                return DapperUtil.Insert<SWfsAppOperatingPosition>(obj, true);
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsAppOperatingPosition>(new
                {
                    OperatingPositionId = obj.OperatingPositionId,
                    PicNo = obj.PicNo,
                    LinkUrl = obj.LinkUrl
                }) ? 1 : 0;
            }
        }

        public int EditeAppOperatingPosition(SWfsAppOperatingPosition obj)
        {
            if (obj.OperatingPositionId!=0)
            {
                return DapperUtil.Update<SWfsAppOperatingPosition>(obj)?1:0;
            }
            return 0;
        }

        public int DeleteAppOperatingPositionBySorts(params int[] sorts)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsAppOperatingPosition_DeleteBySorts", new { Sorts = sorts });
        }

        public int EditePositionSort(int id, int sort)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsAppOperatingPosition_EditePositionSort", new 
            {
                Sort=sort,
                OperatingPositionId=id
            });
        }
        #endregion

        #region 首页
        public List<string> GetHomeAlterPic() 
        {
            List<string> list = new List<string>();
            string AlterOne = DapperUtil.Query<string>("ComBeziWfs_SWfsAppAlterPic_GetSWfsAppHomeAlterPic1").FirstOrDefault();
            string AlterTwo = DapperUtil.Query<string>("ComBeziWfs_SWfsAppAlterPic_GetSWfsAppHomeAlterPic2").FirstOrDefault();
            list.Add(AlterOne);
            list.Add(AlterTwo);
            return list;
        }
        #endregion

        /// <summary>
        /// 根据parentNo得到（状态为有效的）类别列表
        /// </summary>
        /// <param name="parentNo"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategoryList(string parentNo)
        {
            return DapperUtil.Query<Category>("ComBeziWfs_SWfsCategory_GetCategoryListByParentNo", new { ParentNo = parentNo });
        }

        #region 根据热度商品推荐管理
        //获取热度商品

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string GetResponse(string url, string encoding = "")
        {
            string result;
            try
            {
                //创建
                var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                httpWebRequest.KeepAlive = false;
                //访问
                var httpWebResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                Encoding coding = string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
                System.IO.StreamReader sr;
                using (sr = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), coding))
                {
                    result = sr.ReadToEnd();
                }
                //关闭
                httpWebResponse.Close();
                httpWebRequest.Abort();
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("搜索接口地址" + url + "返回数据错误");
            }
            //返回
            return result;
        }
        #endregion

    }



}

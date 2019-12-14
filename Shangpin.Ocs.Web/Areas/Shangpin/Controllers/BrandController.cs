using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Service;
using Shangpin.Ocs.Service.Common;
using System.IO;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.WebMvc;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Framework.Common.Cache;
using System.Web.Script.Serialization;
using System.Text;



namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    [OCSAuthorization]
    public class BrandController : Controller
    {
        private readonly SWfsBrandService brandService;
        private Dictionary<string, string> rsPic = new Dictionary<string, string>();
        public BrandController()
        {
            brandService = new SWfsBrandService();
        }
        //
        // GET: /Shangpin/Brand/

        public ActionResult Index()
        {
            return View();
        }
        #region 索引列表
        /// <summary>
        /// 显示品牌索引列表
        /// </summary>
        /// <param name="TypeId">2：【热门】对应【logo墙】； 1：【旗舰店】对应【广告位】</param>
        /// <param name="BrandView"> BrandView字段 值为 2 处理逻辑为【品牌首页管理】</param>
        /// <param name="PageIndex">起始页</param>
        /// <param name="PageSize">一页显示3条数据</param>
        /// <returns></returns>
        public ActionResult BrandsList(string TypeId, string BrandView, int pageIndex = 1, int pageSize = 10)
        {
            //应对首页品牌改版， BrandView字段 值为 2 品牌首页管理  
            SWfsBrandService brand = new SWfsBrandService();
            if (string.IsNullOrEmpty(BrandView))  // 如果为空，前台不显示品牌管理
            {
                //ViewBag.BrandModuleList = brandService.GetBrandModules(0);

                IList<SWfsBrandModule> list = brand.GetBrandModules(0);
                IList<SWfsBrandModule> newlist = new List<SWfsBrandModule>();

                SWfsBrandModule modue = list.Where(p => p.ModuleNo == "M001").FirstOrDefault();
                newlist.Add(modue);
                SWfsBrandModule modue1 = list.Where(p => p.ModuleNo == "M002").FirstOrDefault();
                newlist.Add(modue1);
                SWfsBrandModule modue2 = list.Where(p => p.ModuleNo == "M010").FirstOrDefault();
                newlist.Add(modue2);
                SWfsBrandModule modue3 = list.Where(p => p.ModuleNo == "M003").FirstOrDefault();
                newlist.Add(modue3);
                SWfsBrandModule modue4 = list.Where(p => p.ModuleNo == "M004").FirstOrDefault();
                newlist.Add(modue4);
                SWfsBrandModule modue5 = list.Where(p => p.ModuleNo == "M005").FirstOrDefault();
                newlist.Add(modue5);
                SWfsBrandModule modue6 = list.Where(p => p.ModuleNo == "M009").FirstOrDefault();
                newlist.Add(modue6);
                SWfsBrandModule modue7 = list.Where(p => p.ModuleNo == "M006").FirstOrDefault();
                newlist.Add(modue7);
                ViewBag.BrandModuleList = newlist;
            }
            else
            {
                if (BrandView == "2" && TypeId == "2")
                    pageSize = 32;
            }

            //列表分页
            int count = 0;
            //IList<SWfsBrandIndexInfo> list = brand.GetBrandIndexList(pageIndex, pageSize, out count);
            //if (string.IsNullOrEmpty(brandView))
            //{
            //    ViewBag.List = BrandDataBind(pageIndex, pageSize, out count);
            //}
            //else
            //{
            //    IEnumerable<SWfsBrandIndexInfo> list = BrandDataBind(pageIndex, pageSize, out count);
            //    if (list != null)
            //    {
            //        ViewBag.List = list.OrderBy(c => c.Sort).ToList();
            //    }
            //}
            IEnumerable<SWfsBrandIndexInfo> _list = BrandDataBind(pageIndex, pageSize, out count);
            ViewBag.List = _list;
            ViewBag.PageIndex = pageIndex;
            ViewBag.totalcount = count;
            ViewBag.currentCount = _list.Count();

            return View();
        }
        public IEnumerable<SWfsBrandIndexInfo> BrandDataBind(int pageIndex, int pageSize, out int count)
        {
            // 如果有值，走的逻辑为 【品牌首页管理】对应的各品牌管理的处理
            string brandView = Request["BrandView"];

            var dic = new Dictionary<string, object>();
            string aa = Request.QueryString["TypeId"];
            ViewBag.KeyWord = Request.QueryString["BrandShowName"] == null ? "" : Request.QueryString["BrandShowName"].Trim();
            dic.Add("BrandShowName", Request.QueryString["BrandShowName"] == null ? "" : Request.QueryString["BrandShowName"].Trim());
            if (Request.QueryString["TypeId"] != "-1" && Request.QueryString["TypeId"] != null && Request.QueryString["TypeId"] != "")
            {
                dic.Add("TypeId", int.Parse(Request.QueryString["TypeId"]));
            }
            else
            {
                dic.Add("TypeId", "");
            }

            if (Request.QueryString["Status"] != "-1" && Request.QueryString["Status"] != null && Request.QueryString["Status"] != "")
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            if (Request.QueryString["ModuleName"] != "-1" && Request.QueryString["ModuleName"] != null && Request.QueryString["ModuleName"] != "")
            {
                dic.Add("ModuleName", Request.QueryString["ModuleName"]);
            }
            else
            {
                dic.Add("ModuleName", "");
            }
            if (string.IsNullOrEmpty(brandView))
            {
                brandView = "0";
            }
            //dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            IEnumerable<SWfsBrandIndexInfo> query = DapperUtil.Query<SWfsBrandIndexInfo>("ComBeziWfs_Brand_SWfsBrandIndex_Listss", dic, new { BrandShowName = dic["BrandShowName"], TypeId = dic["TypeId"], Status = dic["Status"], ModuleName = dic["ModuleName"], BrandView = Convert.ToInt32(brandView), pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_Brand_SWfsBrandIndex_GetBrandIndexTotalCount", dic, new { BrandShowName = dic["BrandShowName"], TypeId = dic["TypeId"], Status = dic["Status"], ModuleName = dic["ModuleName"], BrandView = Convert.ToInt32(brandView) }).First<int>();
            return query;
        }
        #endregion

        #region 专卖店列表
        /// <summary>
        /// 专卖店列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult StoresList(int pageSize = 10, int pageIndex = 1)
        {
            SWfsBrandService brand = new SWfsBrandService();
            int count = 0;
            //IList<SWfsBrandExtendList> list = brand.GetBrandExtendList(pageIndex, pageSize, out count);
            ViewBag.list = DataBind(pageIndex, pageSize, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.totalCount = count;
            #region 获取分类选项列表
            //List<SWfsBrandExtendList> dic = new List<SWfsBrandExtendList>();
            //ViewBag.NaviTypeId = dic;
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("自动", "0");
            //dic.Add("手动", "1");
            //ViewBag.Dic = dic;
            #endregion
            return View();
        }
        //专卖店综合查询
        public IEnumerable<SWfsBrandExtendList> DataBind(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            ViewBag.KeyWord = Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim();
            dic.Add("BrandCnName", Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim());
            if (Request.QueryString["NaviTypeId"] != "-1" && Request.QueryString["NaviTypeId"] != null)
            {
                dic.Add("NaviTypeId", int.Parse(Request.QueryString["NaviTypeId"]));
            }
            else
            {
                dic.Add("NaviTypeId", "");
            }

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null)
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            if (Request.QueryString["storestatus"] != "-1" && Request.QueryString["storestatus"] != null)
            {
                dic.Add("storestatus", int.Parse(Request.QueryString["storestatus"]));
            }
            else
            {
                dic.Add("storestatus", "");
            }
            dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("ZeroToNine", Request.QueryString["ZeroToNine"] != null ? Request.QueryString["ZeroToNine"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            //查询分页集合数据
            IEnumerable<SWfsBrandExtendList> query = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_WfsBrand_Listss", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], pageIndex = pageIndex, pageSize = pageSize, storestatus = dic["storestatus"] });
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_WfsBrand_ListCount", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], storestatus = dic["storestatus"] }).First<int>();
            count = totalCount;
            return query;
        }
        #endregion

        #region 导航是否手动
        /// <summary>
        /// 修改是否手动
        /// </summary>
        /// <param name="BNo">商品编号</param>
        /// <param name="isAuto">是否手动</param>
        /// <returns></returns>
        public int IsAuto(string BNo, string isAuto)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.UpdateIsAuto(BNo, isAuto) < 1)
            {
                return 0;
            }
            return 1;
        }
        #endregion

        #region 店铺编辑(专卖店和旗舰店)
        public int Shop(string BNo1, string isAuto1)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.UpdateShop(BNo1, isAuto1) < 1)
            {
                return 0;
            }
            return 1;
        }
        #endregion

        #region 修改排序字段
        public int UpdateSort(int indexid, int sort)
        {
            SWfsBrandService bll = new SWfsBrandService();
            bool flag = bll.SortUpdate(indexid, sort);
            if (flag)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 修改品牌索引状态
        public ActionResult UpdateStatus(int indexId, int editestatus)
        {
            SWfsBrandService bll = new SWfsBrandService();
            #region 判断类型，提示不能超过显示的个数（针对 品牌首页管理）
            if (editestatus == 1)
            {
                string _brandView = Request.QueryString["BrandView"];
                string _typeId = Request.QueryString["TypeId"];
                int _Count = 0;
                if (!string.IsNullOrEmpty(_brandView))
                {
                    if (_brandView == "2")
                    {
                        if (_typeId == "2")   //【热门品牌】处理显示限制，为128个，如有超出提示
                        {
                            _Count = bll.GetCountShowStatusByBrandView(Convert.ToInt32(_typeId));
                            if (_Count >= 128)
                            {
                                return Content("<script type='text/javascript'>alert('当前显示数已经超过【128】，无法再显示'); history.back();</script>");
                            }

                        }
                        else if (_typeId == "1")  //【旗舰店品牌】处理显示限制，为12个，如有超出提示
                        {
                            _Count = bll.GetCountShowStatusByBrandView(Convert.ToInt32(_typeId));
                            if (_Count >= 12)
                            {
                                return Content("<script type='text/javascript'>alert('当前显示数已经超过【12】，无法再显示'); history.back();</script>");
                            }
                        }
                    }
                }
            }
            #endregion

            bll.UpdateStatus(indexId, editestatus);//?BrandShowName=&ModuleName=-1&TypeId=1&Status=-1

            return Redirect("BrandsList.html?BrandView=" + Request.QueryString["BrandView"] + "&BrandShowName=" + Request.QueryString["BrandShowName"] + "&ModuleName=" + Request.QueryString["ModuleName"] + "&TypeId=" + Request.QueryString["TypeId"] + "&pageindex=" + Request.QueryString["pageindex"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 旗舰店列表
        /// <summary>
        /// 旗舰店
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Flagshipstorelist(int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService brand = new SWfsBrandService();
            int count = 0;
            ViewBag.list = FlagshipStoreDataBind(pageIndex, pageSize, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //旗舰店综合查询
        public IEnumerable<SWfsBrandExtendList> FlagshipStoreDataBind(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            ViewBag.KeyWord = Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim();
            dic.Add("BrandCnName", Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim());
            if (Request.QueryString["NaviTypeId"] != "-1" && Request.QueryString["NaviTypeId"] != null)
            {
                dic.Add("NaviTypeId", int.Parse(Request.QueryString["NaviTypeId"]));
            }
            else
            {
                dic.Add("NaviTypeId", "");
            }

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null)
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }

            if (Request.QueryString["Status1"] != "-1" && Request.QueryString["Status1"] != null)
            {
                dic.Add("Status1", int.Parse(Request.QueryString["Status1"]));
            }
            else
            {
                dic.Add("Status1", "");
            }
            dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("ZeroToNine", Request.QueryString["ZeroToNine"] != null ? Request.QueryString["ZeroToNine"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            //查询分页集合数据
            IEnumerable<SWfsBrandExtendList> query = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_Flagshipstore_Inquiry", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], pageIndex = pageIndex, pageSize = pageSize, Status1 = dic["Status1"] });
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandExtend_Flagshipstore_List", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], Status1 = dic["Status1"] }).First<int>();
            count = totalCount;
            return query;
        }
        #endregion

        #region 全部品牌索引
        //全部品牌列表
        public ActionResult AIIBrandsSelect(int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService bll = new SWfsBrandService();
            int count = 0;
            //IList<SWfsBrandExtendList> list = bll.AIIBrandsList(pageIndex, pageSize, out count);
            ViewBag.list = AllBrandsQueryList(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //全部品牌的综合查询
        public IEnumerable<SWfsBrandExtendList> AllBrandsQueryList(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            //string aa = Request.Form["NaviTypeId"];
            ViewBag.KeyWord = Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim();
            dic.Add("BrandCnName", Request.QueryString["BrandCnName"] == null ? "" : Request.QueryString["BrandCnName"].Trim());
            if (Request.QueryString["NaviTypeId"] != "-1" && Request.QueryString["NaviTypeId"] != null)
            {
                dic.Add("NaviTypeId", int.Parse(Request.QueryString["NaviTypeId"]));
            }
            else
            {
                dic.Add("NaviTypeId", "");
            }

            if (Request.QueryString["BrandTypeId"] != "-1" && Request.QueryString["BrandTypeId"] != null)
            {
                dic.Add("BrandTypeId", int.Parse(Request.QueryString["BrandTypeId"]));
            }
            else
            {
                dic.Add("BrandTypeId", "");
            }
            if (Request.QueryString["storestatus"] != "-1" && Request.QueryString["storestatus"] != null)
            {
                dic.Add("storestatus", int.Parse(Request.QueryString["storestatus"]));
            }
            else
            {
                dic.Add("storestatus", "");
            }

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null)
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("ZeroToNine", Request.QueryString["ZeroToNine"] != null ? Request.QueryString["ZeroToNine"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            //查询分页集合数据
            IEnumerable<SWfsBrandExtendList> query = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_AllBrands_List", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], BrandTypeId = dic["BrandTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], pageIndex = pageIndex, pageSize = pageSize, storestatus = dic["storestatus"] });
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandExtend_AllBrands_Select", dic, new { BrandCnName = dic["BrandCnName"], NaviTypeId = dic["NaviTypeId"], BrandTypeId = dic["BrandTypeId"], Status = dic["Status"], BrandEnName = dic["BrandEnName"], ZeroToNine = dic["ZeroToNine"], storestatus = dic["storestatus"] }).First<int>();
            count = totalCount;
            return query;
        }

        public ActionResult DigitalAccess(string brandName)
        {
            SWfsBrandService bll = new SWfsBrandService();
            var list = bll.DigitalAccess(brandName);
            return View(list);
        }
        #endregion

        #region 品牌索引删除

        public ActionResult BrandDelete(string indexId)
        {
            SWfsBrandService bll = new SWfsBrandService();
            bll.BrandIndexDelete(indexId);
            string _index = Request.QueryString["index"];
            if (!string.IsNullOrEmpty(_index))
            {
                string _pageindex = Request.QueryString["pageindex"];
                if (!string.IsNullOrEmpty(_pageindex))
                {
                    if (_index == "1")
                    {
                        _pageindex = (Convert.ToInt32(_pageindex) - 1).ToString();
                    }
                }
                return Redirect("BrandsList.html?BrandView=" + Request.QueryString["BrandView"] + "&BrandShowName=" + Request.QueryString["BrandShowName"] + "&ModuleName=" + Request.QueryString["ModuleName"] + "&TypeId=" + Request.QueryString["TypeId"] + "&pageindex=" + _pageindex + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));

            }
            else
            {
                return Redirect("BrandsList.html?BrandView=" + Request.QueryString["BrandView"] + "&BrandShowName=" + Request.QueryString["BrandShowName"] + "&ModuleName=" + Request.QueryString["ModuleName"] + "&TypeId=" + Request.QueryString["TypeId"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
            }
        }
        #endregion

        #region 添加专卖店
        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public ActionResult BrandSpecialityStoreEdit(string brandNo = "0")
        {
            SWfsBrandService service = new SWfsBrandService();
            SWfsBrandSpecialityStore obj = service.GetSWfsBrandSpecialityStoreByBrandNo(brandNo);
            if (obj == null)
            {
                return View(new SWfsBrandSpecialityStore { BrandNo = brandNo, SpecialityStoreType = "" });
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult BrandSpecialityStoreEdit(SWfsBrandSpecialityStore obj)
        {
            SWfsBrandService service = new SWfsBrandService();
            CommonService commonService = new CommonService();
            //Dictionary<string, string> rsPic = new Dictionary<string, string>();
            obj.DateCreate = DateTime.Now;
            obj.TemplateId = Request.Form["TemplateId"] != null ? int.Parse(Request.Form["TemplateId"]) : 0;
            obj.SpecialityStoreType = Request.Form["SpecialityStoreType"];
            #region 添加图片
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.SpecialityStorePic = SaveImg(Request.Files["imgfile"], "width:600,Height:250,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.SpecialityStorePic))
                {
                    obj.SpecialityStorePic = "";
                }
            }
            if (string.IsNullOrEmpty(obj.BrandIntroduce))
            {
                obj.BrandIntroduce = "";
            }
            //obj.BrandNo = Request.QueryString["brand"];
            if (obj.BrandIntroduce.Length > 500)
            {
                ViewData["tip"] = new HtmlString("<script>alert('产品介绍不能超过500个字')</script>");
                return View(obj);
            }
            #endregion
            if (obj.SpecialityStoreId == 0)//添加
            {

                if (service.BrandSpecialityStoreInsert(obj) > 0)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='StoresList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");

                }
            }
            else//修改
            {
                if (service.BrandSpecialityStoreUpdate(obj))
                {

                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='StoresList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");

                }
            }
            return View(obj);
        }
        #endregion

        #region 导航
        //导航展示
        public ActionResult NavigateList(string brandName, string brandNo, string storetype)
        {
            SWfsBrandService dal = new SWfsBrandService();
            var list = dal.GetNavigateList(brandNo, brandName);
            if (!string.IsNullOrEmpty(Request.QueryString["storetype"]) && Request.QueryString["storetype"] == "2")
            {
                #region 初始化精品导航
                
                if (list.Count() == 0)
                {
                    //InsertNavigate
                    SWfsBrandNavigation entity = new SWfsBrandNavigation()
                    {
                        NavigationName = "最新",
                        BrandNo = brandNo,
                        ParentId = 0,
                        DateCreate = DateTime.Now,
                        VisibleStatus = 1,
                        Sort = 99,
                        RefType = 0,
                        RefContent = "new"
                    };
                    dal.InsertNavigate(entity);
                    entity.NavigationName = "全部商品";
                    entity.RefContent = "all";
                    entity.Sort = 98;
                    dal.InsertNavigate(entity);
                    list = dal.GetNavigateList(brandNo, brandName);
                }
                else
                {
                    if (list.Where(m => m.RefContent.Equals("new")).Count() == 0)
                    {
                        SWfsBrandNavigation entity = new SWfsBrandNavigation()
                        {
                            NavigationName = "最新",
                            BrandNo = brandNo,
                            ParentId = 0,
                            DateCreate = DateTime.Now,
                            VisibleStatus = 1,
                            Sort = 99,
                            RefType = 0,
                            RefContent = "new"
                        };
                        dal.InsertNavigate(entity);
                        list = dal.GetNavigateList(brandNo, brandName);
                    }
                    if (list.Where(m => m.RefContent.Equals("all")).Count() == 0)
                    {
                        SWfsBrandNavigation entity = new SWfsBrandNavigation()
                        {
                            NavigationName = "全部商品",
                            BrandNo = brandNo,
                            ParentId = 0,
                            DateCreate = DateTime.Now,
                            VisibleStatus = 1,
                            Sort = 98,
                            RefType = 0,
                            RefContent = "all"
                        };
                        dal.InsertNavigate(entity);
                        list = dal.GetNavigateList(brandNo, brandName);
                    }
                }
                #endregion
            }
            ViewBag.NavList = list;
            ViewBag.BrandName = brandName;
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //dal.CreateTree(dal.GetNavigateList(brandNo, brandName), 0, sb);
            //ViewData["htmlcode"] = new HtmlString(sb.ToString());//保存拼接递归的HTML
            //查询品牌
            return View(new SWfsBrandNavigation { BrandNo = brandNo });
        }
        //添加节点
        [HttpPost]
        public ActionResult AddNavigate(SWfsBrandNavigation obj)
        {
            SWfsBrandService dal = new SWfsBrandService();
            obj.DateCreate = DateTime.Now;
            obj.RefContent = "";
            obj.VisibleStatus = 0;
            obj.Sort = dal.GetMaxSortByBrandNoAndPID(obj.BrandNo, obj.ParentId);//获取同类最大的Sort值
            if (dal.InsertNavigate(obj) > 0)
            {
                return Redirect("NavigateList.html?brandNo=" + obj.BrandNo + "&brandName=" + Request.Form["brandName"] + CommonService.GetTimeStamp("&"));
            }
            else
            {
                return Redirect("NavigateList.html?brandNo=" + obj.BrandNo + "&brandName=" + Request.Form["brandName"] + CommonService.GetTimeStamp("&"));
            }
        }
        //修改节点
        public ActionResult EditeNavigate(SWfsBrandNavigation obj)
        {
            SWfsBrandService dal = new SWfsBrandService();
            obj.NavigationId = obj.ParentId;
            if (dal.UpdateNavigateName(obj))
            {
                return Redirect("NavigateList.html?brandNo=" + obj.BrandNo + "&brandName=" + Request.Form["brandName"] + CommonService.GetTimeStamp("&"));
            }
            else
            {
                return Redirect("NavigateList.html?brandNo=" + obj.BrandNo + "&brandName=" + Request.Form["brandName"] + CommonService.GetTimeStamp("&"));
            }

        }
        //删除节点
        public int DelNav(int NavNo)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.DeleteNavigate(NavNo) > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        //显示隐藏
        public int IsShow(int NavNo, short isShow)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.HiddenNavigate(new SWfsBrandNavigation { NavigationId = NavNo, VisibleStatus = isShow }))
            {
                return isShow;
            }
            else
            {
                return -1;
            }
        }
        //移动位置
        public int MoveNavPosition(int navNo1, int sort1, int navNo2, int sort2)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.UpdateNavigatePosition(new SWfsBrandNavigation { NavigationId = navNo1, Sort = sort2 }))
            {
                dal.UpdateNavigatePosition(new SWfsBrandNavigation { NavigationId = navNo2, Sort = sort1 });
                return 1;
            }
            else
            {
                return -0;
            }
        }
        //从属页面加载
        public ActionResult ChangeRelationShip(int id, string brandNo)
        {
            SWfsBrandService dal = new SWfsBrandService();
            IList<SWfsBrandNavigation> newlist = new List<SWfsBrandNavigation>();
            dal.CreateDropDownTree(dal.GetNavigateList(brandNo), newlist, 0);
            ViewBag.NavID = id;
            ViewBag.BrandNo = brandNo;
            return View(newlist);
        }
        //更改从属
        [HttpPost]
        public int ChangeParent(int navNo, int pId, string brandNo)
        {
            SWfsBrandService dal = new SWfsBrandService();
            int sort = dal.GetMaxSortByBrandNoAndPID(brandNo, pId);
            if (dal.UpdateNavigateParent(new SWfsBrandNavigation { NavigationId = navNo, ParentId = pId, Sort = sort }))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        //关联
        public string refTypeStyle(int refType, int navNO, string brandName, string brandNO, int select = 1)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var entity = brandService.BrandPreview(brandNO).FirstOrDefault();
            if (refType == 0)
            {
                sb.Append("<div class='tabNav'>");
                sb.Append("<a href='/Shangpin/Brand/AboutRelationShip.html?navNO=" + navNO + "&select=1&brandName=" + brandName + "&brandNO=" + brandNO + "' " + (select == 1 ? "class='curr'" : null) + ">关联OCS分类</a>");
                if (entity.BrandTypeId != 2)
                    sb.Append("<a href='/Shangpin/Brand/AboutRelationShip.html?navNO=" + navNO + "&select=2&brandName=" + brandName + "&brandNO=" + brandNO + "' " + (select == 2 ? "class='curr'" : null) + ">关联商品</a>");
                sb.Append("<a href='/Shangpin/Brand/AboutRelationShip.html?navNO=" + navNO + "&select=3&brandName=" + brandName + "&brandNO=" + brandNO + "' " + (select == 3 ? "class='curr'" : null) + ">关联链接</a>");
            }
            else
            {
                sb.Append("<div class='tabNav'>");
                sb.Append("<a href='javascript:;' " + (refType == 1 ? "class='curr'" : null) + ">关联OCS分类</a>");
                if (entity.BrandTypeId != 2)
                    sb.Append("<a href='javascript:;' " + (refType == 2 ? "class='curr'" : null) + ">关联商品</a>");
                sb.Append("<a href='javascript:;' " + (refType == 3 ? "class='curr'" : null) + ">关联链接</a>");
            }
            sb.Append("</div >");
            return sb.ToString();
        }
        [HttpGet]
        public ActionResult AboutRelationShip(int navno, int select = 1)
        {
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            if (obj.RefType == 1 || (select == 1 && obj.RefType == 0))//OCS分类
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                IList<OCSInfo> list = dal.GetOCSListByBrandNO(obj.BrandNo, obj.RefContent, sb);
                //获取已经添加过的
                ViewBag.RightAdd = new HtmlString(sb.ToString());
                sb.Length = 0;
                dal.GetOCSTree(list, "ROOT", sb);
                ViewBag.TreeHTML = new HtmlString(sb.ToString());
                sb.Length = 0;
                return View("~/Areas/Shangpin/Views/Brand/AboutOcs.cshtml", obj);
            }
            else if (obj.RefType == 2 || (select == 2 && obj.RefType == 0))//关联商品
            {
                return View("~/Areas/Shangpin/Views/Brand/AboutProduct.cshtml", obj);
            }
            else if (obj.RefType == 3 || (select == 3 && obj.RefType == 0))//关联链接
            {
                return View("~/Areas/Shangpin/Views/Brand/AboutLink.cshtml", obj);
            }
            return View("~/Areas/Shangpin/Views/Brand/AboutOcs.cshtml", obj);//默认ocs分类

        }
        //保存关联链接OCS分类
        [HttpPost]
        public ActionResult AboutRelationShip(SWfsBrandNavigation obj)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            if (string.IsNullOrEmpty(obj.RefContent))
            {
                obj.RefContent = "";
                obj.RefType = 0;
            }
            if (dal.UpdateNavigateRelationShip(obj))
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功')</script>");
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作失败')</script>");
            }
            switch (obj.RefType)
            {
                case 1://ocs分类
                    return Redirect("AboutRelationShip.html?navno=" + obj.NavigationId + "&brandName=" + Request.Form["brandName"] + "&brandNO=" + obj.BrandNo + CommonService.GetTimeStamp("&"));
                //return View("~/Areas/Shangpin/Views/Brand/AboutProduct.cshtml", obj);
                case 2://关联商品
                    return View("~/Areas/Shangpin/Views/Brand/AboutProduct.cshtml", obj);
                case 3://关联链接
                    return View("~/Areas/Shangpin/Views/Brand/AboutLink.cshtml", obj);
                default://默认ocs分类
                    return Redirect("AboutRelationShip.html?navno=" + obj.NavigationId + "&brandName=" + Request.Form["brandName"] + "&brandNO=" + obj.BrandNo + CommonService.GetTimeStamp("&"));
            }
        }
        //关联导航商品查询
        public ActionResult GetProductList(int navno, int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService dal = new SWfsBrandService();
            //ViewBag.AllFirstCategory = dal.SelectErpCategoryByParentNo("ROOT");
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            int totalCount = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            IList<string> idlist = new List<string>();
            IList<ProductInfo> List = dal.GetProductList(ViewBag.Gender, ViewBag.category, obj.BrandNo, idlist, ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            if (!string.IsNullOrEmpty(obj.RefContent))
            {
                string[] oldidlist = obj.RefContent.Split(',');
                foreach (var item in oldidlist)
                {
                    if (List.Count(p => p.ProductNo == item) > 0)
                    {
                        List.Where(p => p.ProductNo == item).First().Status = -2;
                    }
                }
            }
            ViewBag.totalCount = totalCount;
            ViewBag.navNO = navno;
            ViewBag.brandNO = obj.BrandNo;
            return View(List);
        }
        //添加导航关联商品
        [HttpPost]
        public ActionResult AddRelationProductList(int navno, string brandName, string brandNO)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            sb.Append(obj.RefContent);
            if (Request.Form["productNO"] != null)
            {
                string[] newidlist = Request.Form["productNO"].Split(',');
                newidlist = newidlist.Where(p => !string.IsNullOrEmpty(p)).ToArray();
                if (string.IsNullOrEmpty(obj.RefContent))
                {
                    for (int i = 0; i < newidlist.Length; i++)
                    {
                        if (i != newidlist.Length - 1)
                        {
                            sb.Append(newidlist[i] + ",");
                        }
                        else
                        {
                            sb.Append(newidlist[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newidlist.Length; i++)
                    {
                        sb.Append("," + newidlist[i]);
                    }
                }

            }
            dal.UpdateNavigateRelationShip(new SWfsBrandNavigation() { NavigationId = navno, RefType = 2, RefContent = sb.ToString() });
            return Redirect("GetProductList.html?navNO=" + navno + "&brandName=" + brandName + "&brandNO=" + brandNO + CommonService.GetTimeStamp("&"));
        }
        [HttpPost]
        public int AddRelationProductListAboutNavByAjax(int navno, string productNO)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            string[] newidlist = productNO.Split(',');
            System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            for (int i = 0; i < newidlist.Length; i++)//过滤不合法商品编号
            {
                if (!string.IsNullOrEmpty(newidlist[i].Trim().Replace("\n", null)) && _regex.IsMatch(newidlist[i].Trim().Replace("\n", null)))
                {
                    newidlist[i] = newidlist[i].Trim().Replace("\n", null);
                }
                else
                {
                    newidlist[i] = "0";
                }
            }
            newidlist = newidlist.Where(p => p != "0").ToArray();
            //newidlist = newidlist.Where(p => !string.IsNullOrEmpty(p.Trim().Replace("\n", null))).ToArray();
            if (newidlist.Length <= 0)
            {
                return 0;
            }
            sb.Append(obj.RefContent);
            if (!string.IsNullOrEmpty(productNO))
            {
                if (string.IsNullOrEmpty(obj.RefContent))
                {
                    for (int i = 0; i < newidlist.Length; i++)
                    {
                        if (i != newidlist.Length - 1)
                        {
                            sb.Append(newidlist[i].Trim().Replace("\n", null) + ",");
                        }
                        else
                        {
                            sb.Append(newidlist[i].Trim().Replace("\n", null));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newidlist.Length; i++)
                    {
                        if (obj.RefContent.IndexOf(newidlist[i].Trim().Replace("\n", null)) < 0)
                        {
                            sb.Append("," + newidlist[i].Trim().Replace("\n", null));
                        }
                    }
                }
                if (dal.UpdateNavigateRelationShip(new SWfsBrandNavigation() { NavigationId = navno, RefType = 2, RefContent = sb.ToString() }))
                {
                    return 1;
                }
            }
            return 0;
        }
        //管理已经加入的导航商品列表页
        public ActionResult ManagerNavRelationProduct(int navno, int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService dal = new SWfsBrandService();
            ViewBag.AllFirstCategory = dal.SelectErpCategoryByParentNo("ROOT");
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            int totalCount = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            ViewBag.navNO = navno;
            ViewBag.pNOList = obj.RefContent;
            IList<ProductInfo> List = dal.GetProductList(ViewBag.Gender, ViewBag.category, obj.BrandNo, obj.RefContent.Split(',').ToList<string>(), ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            ViewBag.totalCount = totalCount;
            ViewBag.brandNO = obj.BrandNo;
            return View(List);
        }
        //删除已经加入导航商品
        [HttpPost]
        public ActionResult RemoveNavRalationProduct(int navno, string brandName, string brandNO)
        {
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandNavigation obj = dal.GetNavObj(navno);
            if (Request.Form["productNO"] != null)
            {
                string[] oldidlist = obj.RefContent.Split(',');
                string[] newidlist = Request.Form["productNO"].Split(',');
                //newidlist = newidlist.Where(p => !string.IsNullOrEmpty(p)).ToArray();
                foreach (var item in newidlist)
                {
                    if (obj.RefContent.IndexOf(item) >= 0)
                    {
                        obj.RefContent = obj.RefContent.Replace(item + ",", "");
                        obj.RefContent = obj.RefContent.Replace("," + item, "");
                        obj.RefContent = obj.RefContent.Replace(item, "");
                    }
                }
                //for (int i = 0; i < oldidlist.Length; i++)
                //{
                //    if (newidlist.Count(p => p == oldidlist[i]) > 0)
                //    {

                //    }
                //    else
                //    {
                //        sb.Append(oldidlist[i] + ",");
                //    }
                //}
                //if (sb.Length > 0)
                //{
                //    sb.Remove(sb.Length - 1, 1);
                //}
            }
            if (string.IsNullOrEmpty(obj.RefContent))
            {
                obj.RefType = 0;
                dal.UpdateNavigateRelationShip(new SWfsBrandNavigation() { NavigationId = navno, RefType = obj.RefType, RefContent = obj.RefContent });
                return Redirect("AboutRelationShip.html?navNO=" + navno + "&brandName=" + brandName + "&brandNO=" + brandNO + CommonService.GetTimeStamp("&"));
            }
            else
            {
                dal.UpdateNavigateRelationShip(new SWfsBrandNavigation() { NavigationId = navno, RefType = obj.RefType, RefContent = obj.RefContent });
                return Redirect("ManagerNavRelationProduct.html?navNO=" + navno + "&brandName=" + brandName + "&brandNO=" + brandNO + CommonService.GetTimeStamp("&"));
            }

        }
        //根据父类ID查询erp类别
        public string GetErpCategoryChildList(string pNo)
        {
            SWfsBrandService dal = new SWfsBrandService();
            IList<ErpCategory> list = dal.SelectErpCategoryByParentNo(pNo);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (list != null)
            {
                foreach (var item in list)
                {
                    sb.Append("<option value='" + item.CategoryNo + "'>" + item.CategoryName + "</option>");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 旗舰店
        public ActionResult FlagShipStore(string brandNO)
        {
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipByBrandNO(brandNO);
            if (obj == null)
            {
                obj = new SWfsBrandFlagShipStoreSave() { BrandNo = brandNO, IsNaviBg = 1 };
            }
            return View(obj);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FlagShipStore(SWfsBrandFlagShipStoreSave obj)
        {
            #region 上传图片
            if (Request.Files["LogoPicNofile"] != null && Request.Files["LogoPicNofile"].ContentLength > 0)//旗舰店LOGO图
            {
                obj.LogoPicNo = SaveImg(Request.Files["LogoPicNofile"], "width:960,Height:100,Length:20");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.LogoPicNo))
                {
                    obj.LogoPicNo = "";
                }
            }
            if (Request.Files["LogoBgPicfile"] != null && Request.Files["LogoBgPicfile"].ContentLength > 0)//旗舰店LOGO图背景图
            {
                HttpPostedFileBase file = Request.Files["LogoBgPicfile"];
                int conLen = file.ContentLength;
                //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                byte[] btImgs = new byte[conLen];
                using (Stream s = Request.Files["LogoBgPicfile"].InputStream)
                {
                    s.Read(btImgs, 0, conLen);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                int width = original_image.Width;//图片的宽度
                int height = original_image.Height;//图片的高度
                original_image.Dispose();//释放资源
                long length = stream.Length / 1024;//图片的大小（KB）
                stream.Close();
                if (width >= 1200 && width <= 1900 && length <= 100 && height == 100)
                {
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = Path.GetExtension(file.FileName).ToLower();
                    model.FileName = Path.GetFileName(file.FileName);
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    obj.LogoBgPic = service.InserSystemImg(model);
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('请上传（高为100宽为1200-1900小于400k的图片）')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.LogoBgPic) || Request.Form["istoutu"] == null)
                {
                    obj.LogoBgPic = "";
                }
            }
            obj.IsNaviBg = Request.Form["IsNaviBg"] != null ? 1 : 0;//导航底色扩展
            if (Request.Files["AlterPicNo1file"] != null && Request.Files["AlterPicNo1file"].ContentLength > 0)//轮播1
            {
                obj.AlterPicNo1 = SaveImg(Request.Files["AlterPicNo1file"], "width:960,Height:420,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.AlterPicNo1))
                {
                    obj.AlterPicNo1 = "";
                }
            }
            if (Request.Files["AlterPicNo2file"] != null && Request.Files["AlterPicNo2file"].ContentLength > 0)//轮播2
            {
                obj.AlterPicNo2 = SaveImg(Request.Files["AlterPicNo2file"], "width:960,Height:420,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.AlterPicNo2))
                {
                    obj.AlterPicNo2 = "";
                }
            }
            if (Request.Files["AlterPicNo3file"] != null && Request.Files["AlterPicNo3file"].ContentLength > 0)//轮播3
            {
                obj.AlterPicNo3 = SaveImg(Request.Files["AlterPicNo3file"], "width:960,Height:420,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.AlterPicNo3))
                {
                    obj.AlterPicNo3 = "";
                }
            }
            if (Request.Files["StoreBgPicfile"] != null && Request.Files["StoreBgPicfile"].ContentLength > 0)//大背景图
            {
                HttpPostedFileBase file = Request.Files["StoreBgPicfile"];
                int conLen = file.ContentLength;
                //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                byte[] btImgs = new byte[conLen];
                using (Stream s = Request.Files["StoreBgPicfile"].InputStream)
                {
                    s.Read(btImgs, 0, conLen);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                int width = original_image.Width;//图片的宽度
                int height = original_image.Height;//图片的高度
                original_image.Dispose();//释放资源
                long length = stream.Length / 1024;//图片的大小（KB）
                stream.Close();
                if (width >= 1200 && width <= 1900 && length <= 400)
                {
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = Path.GetExtension(file.FileName).ToLower();
                    model.FileName = Path.GetFileName(file.FileName);
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    obj.StoreBgPic = service.InserSystemImg(model);
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('请上传（宽为1200-1900小于400k的图片）')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.StoreBgPic))
                {
                    obj.StoreBgPic = "";
                }
            }
            if (Request.Files["ProductTitlePicNo1file"] != null && Request.Files["ProductTitlePicNo1file"].ContentLength > 0)//商品模块1
            {
                //obj.ProductTitlePicNo1 = SaveImg(Request.Files["ProductTitlePicNo1file"], "width:960,Height:20-200,Length:50");
                //if (rsPic.Keys.Contains("error"))
                //{
                //    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                //    return View(obj);
                //}
                HttpPostedFileBase file = Request.Files["ProductTitlePicNo1file"];
                int conLen = file.ContentLength;
                //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                byte[] btImgs = new byte[conLen];
                using (Stream s = Request.Files["ProductTitlePicNo1file"].InputStream)
                {
                    s.Read(btImgs, 0, conLen);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                int width = original_image.Width;//图片的宽度
                int height = original_image.Height;//图片的高度
                original_image.Dispose();//释放资源
                long length = stream.Length / 1024;//图片的大小（KB）
                stream.Close();
                if (width == 960 && height >= 20 && height <= 200 && length <= 60)
                {
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = Path.GetExtension(file.FileName).ToLower();
                    model.FileName = Path.GetFileName(file.FileName);
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    obj.ProductTitlePicNo1 = service.InserSystemImg(model);
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('请上传宽960（高为20-200小于60k的图片）')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ProductTitlePicNo1))
                {
                    obj.ProductTitlePicNo1 = "";
                }
            }
            if (Request.Files["ProductTitlePicNo2file"] != null && Request.Files["ProductTitlePicNo2file"].ContentLength > 0)//商品模块2
            {
                //obj.ProductTitlePicNo2 = SaveImg(Request.Files["ProductTitlePicNo2file"], "width:952,Height:40,Length:50");
                //if (rsPic.Keys.Contains("error"))
                //{
                //    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                //    return View(obj);
                //}
                HttpPostedFileBase file = Request.Files["ProductTitlePicNo2file"];
                int conLen = file.ContentLength;
                //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                byte[] btImgs = new byte[conLen];
                using (Stream s = Request.Files["ProductTitlePicNo2file"].InputStream)
                {
                    s.Read(btImgs, 0, conLen);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                int width = original_image.Width;//图片的宽度
                int height = original_image.Height;//图片的高度
                original_image.Dispose();//释放资源
                long length = stream.Length / 1024;//图片的大小（KB）
                stream.Close();
                if (width == 960 && height >= 20 && height <= 200 && length <= 60)
                {
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = Path.GetExtension(file.FileName).ToLower();
                    model.FileName = Path.GetFileName(file.FileName);
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    obj.ProductTitlePicNo2 = service.InserSystemImg(model);
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('请上传宽960（高为20-200小于60k的图片）')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.ProductTitlePicNo2))
                {
                    obj.ProductTitlePicNo2 = "";
                }
            }
            #endregion
            obj.DateCreate = DateTime.Now;
            if (true)//测试数据
            {
                if (string.IsNullOrEmpty(obj.ProductNos1))
                {
                    obj.ProductNos1 = "";
                }
                if (string.IsNullOrEmpty(obj.ProductNos2))
                {
                    obj.ProductNos2 = "";
                }
                if (string.IsNullOrEmpty(obj.TemplateCode))
                {
                    obj.TemplateCode = "";
                }
                if (string.IsNullOrEmpty(obj.HtmlCode))
                {
                    obj.HtmlCode = "";
                }
                if (string.IsNullOrEmpty(obj.ProductTitleAddr1))
                {
                    obj.ProductTitleAddr1 = "";
                }
                if (string.IsNullOrEmpty(obj.ProductTitleAddr2))
                {
                    obj.ProductTitleAddr2 = "";
                }
                if (string.IsNullOrEmpty(obj.AlterPicAddr1))
                {
                    obj.AlterPicAddr1 = "";
                }
                if (string.IsNullOrEmpty(obj.AlterPicAddr2))
                {
                    obj.AlterPicAddr2 = "";
                }
                if (string.IsNullOrEmpty(obj.AlterPicAddr3))
                {
                    obj.AlterPicAddr3 = "";
                }
                //obj.ProductNos1 = "01235682,01234119,01235701,01231865,01235682,01233981,01235680,01235678,01234116,01234122,01235794";
                //obj.ProductNos2 = "01235682,01234119,01235701,01231865,01235682,01233981,01235680,01235678,01234116,01234122,01235794";
            }
            SWfsBrandService dal = new SWfsBrandService();
            if (obj.FlagShipStoreId == 0)//添加
            {
                if (dal.InsertFlagShipStore(obj) > 0)
                {
                    obj = dal.GetFlagShipByBrandNO(obj.BrandNo);
                    return Redirect("FlagShipStore.html?brandNO=" + obj.BrandNo + CommonService.GetTimeStamp("&"));
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");
                    return View(obj);
                }
            }
            else //修改
            {
                if (dal.UpdateFlagShipStore(obj))
                {
                    return Redirect("FlagShipStore.html?brandNO=" + obj.BrandNo + CommonService.GetTimeStamp("&"));
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");
                    return View(obj);
                }
            }

        }
        //添加商品加载
        public ActionResult ProductList()
        {
            SWfsBrandService dal = new SWfsBrandService();
            return View();
        }
        [HttpPost]
        public ActionResult AddAboutProduct(int flagNo, int order)
        {
            SWfsBrandService dal = new SWfsBrandService();
            dal.AddAboutProduct(flagNo, Request.Form["objid"], order);
            return RedirectToAction("ProductList" + CommonService.GetTimeStamp("?"));
        }
        //发布旗舰店
        [HttpPost, ValidateInput(false)]
        public ActionResult PublishiFlag(SWfsBrandFlagShipStoreSave obj)
        {
            SWfsBrandFlagShipStore obj1 = new SWfsBrandFlagShipStore()
            {
                BrandNo = obj.BrandNo,
                LogoPicNo = obj.LogoPicNo != null ? obj.LogoPicNo : "",
                LogoBgPic = obj.LogoBgPic != null ? obj.LogoBgPic : "",
                IsNaviBg = obj.IsNaviBg,
                TemplateCode = obj.TemplateCode != null ? obj.TemplateCode : "",
                AlterPicNo1 = obj.AlterPicNo1 != null ? obj.AlterPicNo1 : "",
                AlterPicAddr1 = obj.AlterPicAddr1 != null ? obj.AlterPicAddr1 : "",
                AlterPicNo2 = obj.AlterPicNo2 != null ? obj.AlterPicNo2 : "",
                AlterPicAddr2 = obj.AlterPicAddr2 != null ? obj.AlterPicAddr2 : "",
                AlterPicNo3 = obj.AlterPicNo3 != null ? obj.AlterPicNo3 : "",
                AlterPicAddr3 = obj.AlterPicAddr3 != null ? obj.AlterPicAddr3 : "",
                DateCreate = DateTime.Now,
                ProductTitlePicNo1 = obj.ProductTitlePicNo1 != null ? obj.ProductTitlePicNo1 : "",
                ProductTitleAddr1 = obj.ProductTitleAddr1 != null ? obj.ProductTitleAddr1 : "",
                ProductNos1 = obj.ProductNos1 != null ? obj.ProductNos1 : "",
                ProductTitlePicNo2 = obj.ProductTitlePicNo2 != null ? obj.ProductTitlePicNo2 : "",
                ProductTitleAddr2 = obj.ProductTitleAddr2 != null ? obj.ProductTitleAddr2 : "",
                ProductNos2 = obj.ProductNos2 != null ? obj.ProductNos2 : "",
                StoreBgPic = obj.StoreBgPic != null ? obj.StoreBgPic : "",
                HtmlCode = obj.HtmlCode != null ? obj.HtmlCode : "",
                ProductPicShowType = obj.ProductPicShowType == 0 ? 1 : obj.ProductPicShowType

            };
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.PublishFlagShipStoreInfo(obj1) > 0)
            {
                //更新旗舰店State状态
                if (obj.Status == 0)
                {
                    if (dal.UpdateFlagShipStatus(obj.FlagShipStoreId))
                    {
                        obj.Status = 1;
                    }
                    obj.Status = 1;
                }
                ViewData["tip"] = new HtmlString("<script>alert('发布成功')</script>");
            }
            else
            {
                ViewData["tip"] = new HtmlString("<script>alert('发布失败')</script>");
            }
            return View("FlagShipStore", obj);
        }
        //异步上传图片
        public string GetImgPath()
        {
            string picNO = "";
            if (Request.Files["Filedata"] != null && Request.Files["Filedata"].ContentLength > 0)
            {
                picNO = SaveImg(Request.Files["Filedata"], "width:0,Height:0,Length:80");
            }
            return ServicePic.ResolveUGCImage("2", picNO, 0, 0);
            //return "/ReadPic/GetPic.ashx?width=70&height=107&pictureFileNo=" + picNO + "&type=2";
        }
        public string GetAjaxImgJson(string picPath, string targetlabel, string imglink = "")
        {
            return "{\"imgsrc\":\"" + (string.IsNullOrEmpty(picPath) ? "#" : picPath) + "\",\"imghref\":\"" + (string.IsNullOrEmpty(imglink) ? "#" : imglink) + "\",\"targetlabel\":\"" + (string.IsNullOrEmpty(targetlabel) ? "1" : targetlabel) + "\"}";
        }

        //读取模板文件
        //[HttpPost]
        public string GetTemplateText(int flagNO = 0)
        {
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(flagNO);
            if (obj == null)
            {
                obj = new SWfsBrandFlagShipStoreSave();
            }
            if (string.IsNullOrEmpty(obj.HtmlCode))
            {
                if (!string.IsNullOrEmpty(obj.TemplateCode))
                {
                    return obj.TemplateCode;
                }
                string templatePath = Server.MapPath("~/Areas/Shangpin/Content/data/templ_example.txt");//data/templ_example.txt 
                if (System.IO.File.Exists(templatePath))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(templatePath))
                    {
                        return sr.ReadToEnd();
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return obj.HtmlCode;
            }

        }
        //添加旗舰店关联商品
        public ActionResult FlagRelationProduct(int navno, int region, int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService dal = new SWfsBrandService();
            ViewBag.AllFirstCategory = dal.SelectErpCategoryByParentNo("ROOT");
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(navno);
            int totalCount = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            IList<string> idlist = new List<string>();
            IList<ProductInfo> List = dal.GetProductList(ViewBag.Gender, ViewBag.category, obj.BrandNo, idlist, ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            List = List.GroupBy(p => p.ProductNo).Select(p => new ProductInfo 
            {
                ProductNo=p.ElementAt(0).ProductNo,
                ProductName=p.ElementAt(0).ProductName,
                ProductModel=p.ElementAt(0).ProductModel,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductPicFile=p.ElementAt(0).ProductPicFile,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                PcSaleState = p.ElementAt(0).PcSaleState,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoldPrice = p.Min(a=>a.GoldPrice),
                SellPrice = p.ElementAt(0).SellPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            }).ToList();
            if (region == 1)
            {
                if (!string.IsNullOrEmpty(obj.ProductNos1))
                {
                    string[] oldidlist = obj.ProductNos1.Split(',');
                    foreach (var item in oldidlist)
                    {
                        if (List.Count(p => p.ProductNo == item) > 0)
                        {
                            List.Where(p => p.ProductNo == item).First().Status = -2;
                        }
                    }
                }
            }
            if (region == 2)
            {
                if (!string.IsNullOrEmpty(obj.ProductNos2))
                {
                    string[] oldidlist = obj.ProductNos2.Split(',');
                    foreach (var item in oldidlist)
                    {
                        if (List.Count(p => p.ProductNo == item) > 0)
                        {
                            List.Where(p => p.ProductNo == item).First().Status = -2;
                        }
                    }
                }
            }
            ViewBag.totalCount = totalCount;
            ViewBag.navNO = navno;
            ViewBag.region = region;
            ViewBag.brandNO = obj.BrandNo;
            return View(List);
        }
        [HttpPost]
        public ActionResult AddFlagRelationProduct(int navno, int region)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(navno);
            if (region == 1)
            {
                sb.Append(obj.ProductNos1);
                if (Request.Form["productNO"] != null)
                {
                    string[] newidlist = Request.Form["productNO"].Split(',');
                    newidlist = newidlist.Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    if (string.IsNullOrEmpty(obj.ProductNos1))
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (i != newidlist.Length - 1)
                            {
                                sb.Append(newidlist[i] + ",");
                            }
                            else
                            {
                                sb.Append(newidlist[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            sb.Append("," + newidlist[i]);
                        }
                    }

                }
            }
            if (region == 2)
            {
                sb.Append(obj.ProductNos2);
                if (Request.Form["productNO"] != null)
                {
                    string[] newidlist = Request.Form["productNO"].Split(',');
                    if (string.IsNullOrEmpty(obj.ProductNos2))
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (i != newidlist.Length - 1)
                            {
                                sb.Append(newidlist[i] + ",");
                            }
                            else
                            {
                                sb.Append(newidlist[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            sb.Append("," + newidlist[i]);
                        }
                    }
                }
            }

            dal.AddAboutProduct(navno, sb.ToString(), region);
            return Redirect("FlagRelationProduct.html?navNO=" + navno + "&region=" + region + CommonService.GetTimeStamp("&"));

        }
        [HttpPost]
        public int AddFlagRelationProductByAjax(int navno, int region, string productNO)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(navno);
            string[] newidlist = null;
            if (!string.IsNullOrEmpty(productNO))
            {
                if (productNO.IndexOf(',')>0)
                {
                    newidlist = productNO.Split(',');
                }
                else
                {
                    newidlist = productNO.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            //string[] newidlist = productNO.Split(',');
            System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            for (int i = 0; i < newidlist.Length; i++)//过滤不合法商品编号
            {
                if (!string.IsNullOrEmpty(newidlist[i].Trim()) && _regex.IsMatch(newidlist[i].Trim()))
                {
                    newidlist[i] = newidlist[i].Trim();
                }
                else
                {
                    newidlist[i] = "0";
                }
            }
            newidlist = newidlist.Where(p => p != "0"&&p.Length<=20).ToArray();
            //newidlist = newidlist.Where(p => !string.IsNullOrEmpty(p.Trim().Replace("\n", null))).ToArray();
            if (newidlist.Length <= 0)
            {
                return 0;
            }
            if (region == 1)
            {
                sb.Append(obj.ProductNos1);
                if (!string.IsNullOrEmpty(productNO))
                {
                    if (string.IsNullOrEmpty(obj.ProductNos1))
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (i != newidlist.Length - 1)
                            {
                                sb.Append(newidlist[i].Trim()+ ",");
                            }
                            else
                            {
                                sb.Append(newidlist[i].Trim());
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (obj.ProductNos1.IndexOf(newidlist[i].Trim()) < 0)
                            {
                                sb.Append("," + newidlist[i].Trim());
                            }
                        }
                    }

                }
            }
            if (region == 2)
            {
                sb.Append(obj.ProductNos2);
                if (!string.IsNullOrEmpty(productNO))
                {
                    if (string.IsNullOrEmpty(obj.ProductNos2))
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (i != newidlist.Length - 1)
                            {
                                sb.Append(newidlist[i].Trim() + ",");
                            }
                            else
                            {
                                sb.Append(newidlist[i].Trim());
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newidlist.Length; i++)
                        {
                            if (obj.ProductNos2.IndexOf(newidlist[i].Trim()) < 0)
                            {
                                sb.Append("," + newidlist[i].Trim());
                            }
                        }
                    }
                }
            }

            if (dal.AddAboutProduct(navno, sb.ToString(), region))
            {
                return 1;
            }
            return 0;
        }
        //更新旗舰店关联商品排序
        [HttpPost]
        public int UpdateRelationProductOrder(int flagId, string pNoList, int region)
        {
            SWfsBrandService dal = new SWfsBrandService();
            if (dal.AddAboutProduct(flagId, pNoList, region))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        //管理旗舰店商品
        public ActionResult ManagerFlagRelationProduct(int navno, int region, int pageIndex = 1, int pageSize = 100)
        {
            SWfsBrandService dal = new SWfsBrandService();
            ViewBag.AllFirstCategory = dal.SelectErpCategoryByParentNo("ROOT");
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(navno);
            IList<ProductInfo> List = new List<ProductInfo>();
            IList<ProductInfo> orderList = null;
            int totalCount = 0;
            if (Request.QueryString["keyWord"] != null)
            {
                ViewBag.keyWord = Request.QueryString["keyWord"];
            }
            if (Request.QueryString["CategoryNo"] != null)
            {
                ViewBag.category = Request.QueryString["CategoryNo"];
            }
            if (Request.QueryString["Gender"] != null)
            {
                ViewBag.Gender = Request.QueryString["Gender"];
            }
            ViewBag.navNO = navno;
            ViewBag.region = region;
            ViewBag.brandNO = obj.BrandNo;
            IList<string> idlist = new List<string>();
            if (region == 1)
            {
                if (!string.IsNullOrEmpty(obj.ProductNos1))
                {
                    idlist = obj.ProductNos1.Split(',');
                }
                else
                {
                    ViewBag.totalCount = 0;
                    return View(List);
                }

            }
            if (region == 2)
            {
                if (!string.IsNullOrEmpty(obj.ProductNos2))
                {
                    idlist = obj.ProductNos2.Split(',');
                }
                else
                {
                    ViewBag.totalCount = 0;
                    return View(List);
                }
            }
            List = dal.GetProductList(ViewBag.Gender, ViewBag.category, obj.BrandNo, idlist, ViewBag.keyWord, pageIndex, pageSize, out totalCount);
            List = List.GroupBy(p => p.ProductNo).Select(p => new ProductInfo
            {
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductModel = p.ElementAt(0).ProductModel,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                PcSaleState = p.ElementAt(0).PcSaleState,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoldPrice = p.ElementAt(0).GoldPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            }).ToList();
            
            ViewBag.totalCount = totalCount;
            if (idlist.Count > 0)
            {
                orderList = new List<ProductInfo>();
                foreach (var item in idlist)
                {
                    if (List.Count(p => p.ProductNo == item) > 0)
                    {
                        orderList.Add(List.Single(p => p.ProductNo == item));
                    }
                }
                return View(orderList);
            }
            return View(List);
        }
        //删除旗舰店关联商品
        [HttpPost]
        public ActionResult RemoveFlagRalationProduct(int navno, int region)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            SWfsBrandService dal = new SWfsBrandService();
            SWfsBrandFlagShipStoreSave obj = dal.GetFlagShipObj(navno);
            if (region == 1)
            {
                if (Request.Form["productNO"] != null)
                {
                    //string[] oldidlist = obj.ProductNos1.Split(',');
                    string[] newidlist = Request.Form["productNO"].Split(',');
                    foreach (var item in newidlist)
                    {
                        if (obj.ProductNos1.IndexOf(item) >= 0)
                        {
                            obj.ProductNos1 = obj.ProductNos1.Replace(item + ",", "");
                            obj.ProductNos1 = obj.ProductNos1.Replace("," + item, "");
                            obj.ProductNos1 = obj.ProductNos1.Replace(item, "");
                        }
                    }
                    sb.Append(obj.ProductNos1);
                    //for (int i = 0; i < oldidlist.Length; i++)
                    //{
                    //    if (newidlist.Count(p => p == oldidlist[i]) > 0)
                    //    {

                    //    }
                    //    else
                    //    {
                    //        sb.Append(oldidlist[i] + ",");
                    //    }
                    //}
                    //if (sb.Length > 0)
                    //{
                    //    sb.Remove(sb.Length - 1, 1);
                    //}
                }
            }
            if (region == 2)
            {
                if (Request.Form["productNO"] != null)
                {
                    //string[] oldidlist = obj.ProductNos2.Split(',');
                    string[] newidlist = Request.Form["productNO"].Split(',');
                    foreach (var item in newidlist)
                    {
                        if (obj.ProductNos2.IndexOf(item) >= 0)
                        {
                            obj.ProductNos2 = obj.ProductNos2.Replace(item + ",", "");
                            obj.ProductNos2 = obj.ProductNos2.Replace("," + item, "");
                            obj.ProductNos2 = obj.ProductNos2.Replace(item, "");
                        }
                    }
                    sb.Append(obj.ProductNos2);
                    //for (int i = 0; i < oldidlist.Length; i++)
                    //{
                    //    if (newidlist.Count(p => p == oldidlist[i]) > 0)
                    //    {

                    //    }
                    //    else
                    //    {
                    //        sb.Append(oldidlist[i] + ",");
                    //    }
                    //}
                    //if (sb.Length > 0)
                    //{
                    //    sb.Remove(sb.Length - 1, 1);
                    //}
                }
            }
            dal.AddAboutProduct(navno, sb.ToString(), region);
            return Redirect("ManagerFlagRelationProduct.html?navNO=" + navno + "&region=" + region + CommonService.GetTimeStamp("&"));
        }

        //通用保存图片
        private string SaveImg(HttpPostedFileBase file, string imgInfo, string type = "")
        {
            CommonService commonService = new CommonService();
            rsPic.Clear();
            //Dictionary<string, string> rsPic = new Dictionary<string, string>();
            if (file.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(type))
                {
                    rsPic = commonService.PostImg(file, imgInfo);
                }
                else
                {
                    rsPic = commonService.PostImg(file, imgInfo, type);
                }

                if (rsPic.Keys.Contains("error"))
                {
                    return rsPic["error"];
                }
                if (rsPic.Keys.Contains("success"))
                {
                    return rsPic["success"];
                }
            }
            return "";
        }
        #endregion

        #region 品牌索引新建页面

        public ActionResult BrandIndexCreate(string brandIndexId)
        {
            SWfsBrandService brand = new SWfsBrandService();
            //var brandModuleList = brandService.GetBrandModules();
            //ViewBag.BrandModuleList = brandModuleList;

            //获取所有模块，重新排序
            IList<SWfsBrandModule> list = brand.GetBrandModules(0);
            IList<SWfsBrandModule> newlist = new List<SWfsBrandModule>();
            //模块重新排序
            SWfsBrandModule modue = list.Where(p => p.ModuleNo == "M001").FirstOrDefault();
            newlist.Add(modue);
            SWfsBrandModule modue1 = list.Where(p => p.ModuleNo == "M002").FirstOrDefault();
            newlist.Add(modue1);
            SWfsBrandModule modue2 = list.Where(p => p.ModuleNo == "M010").FirstOrDefault();
            newlist.Add(modue2);
            SWfsBrandModule modue3 = list.Where(p => p.ModuleNo == "M003").FirstOrDefault();
            newlist.Add(modue3);
            SWfsBrandModule modue4 = list.Where(p => p.ModuleNo == "M004").FirstOrDefault();
            newlist.Add(modue4);
            SWfsBrandModule modue5 = list.Where(p => p.ModuleNo == "M005").FirstOrDefault();
            newlist.Add(modue5);
            SWfsBrandModule modue6 = list.Where(p => p.ModuleNo == "M009").FirstOrDefault();
            newlist.Add(modue6);
            SWfsBrandModule modue7 = list.Where(p => p.ModuleNo == "M006").FirstOrDefault();
            newlist.Add(modue7);
            ViewBag.BrandModuleList = newlist;
            if (!string.IsNullOrEmpty(brandIndexId))
            {
                SWfsBrandIndexInfo BrandIIEntity = brandService.GetSwfsBrandIndexByIndexId(brandIndexId);
                ViewBag.BrandIndexID = brandIndexId;
                ViewBag.BrandIndexModel = BrandIIEntity;
                ViewBag.BrandModuleType = BrandIIEntity.ModuleType;
            }
            return View();
        }


        [HttpPost]
        public JsonResult BrandIndexSave()
        {
            SWfsBrandIndex brandIndex = new SWfsBrandIndex();
            SWfsBrandService brand = new SWfsBrandService();
            string brandName = string.Empty;

            #region 批量添加（应对热门品牌）
            string _batch = Request.Form["hidbatch"];
            string messageShow = string.Empty;
            if (!string.IsNullOrEmpty(_batch))
            {
                string brandView = Request.Form["hidBrandView"];
                string typeId = Request.Form["hidTypeId"];
                if (!string.IsNullOrEmpty(brandView))
                {
                    string hotBrand = Request.Form["HotBrand"];
                    if (string.IsNullOrEmpty(hotBrand) || hotBrand.StartsWith("请批量"))
                    {
                        return Json(new { result = -1, message = "请输入需要添加的品牌！" }, "text/plain", Encoding.UTF8);
                    }
                    hotBrand = hotBrand.Replace("，", ",");
                    string[] brandNoS = hotBrand.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int j = brand.GetIsExistBrandNoByBrandNo(hotBrand);
                    if (brandNoS.Length != j)
                    {
                        return Json(new { result = -1, message = "批量添加中含有无效的品牌！" }, "text/plain", Encoding.UTF8);
                    }
                    int _IsExist = brand.GetIsExistBrandNoByBrandNoAndBrandView(hotBrand, Convert.ToInt32(typeId), "true");
                    if (_IsExist > 0)
                    {
                        return Json(new { result = -1, message = "批量添加中有品牌已经存在！" }, "text/plain", Encoding.UTF8);
                    }
                    int _status = 0;
                    int _Count = brand.GetCountShowStatusByBrandView(Convert.ToInt32(typeId));
                    if ((128 - _Count) < brandNoS.Length)
                    {
                        return Json(new { result = -1, message = "列表中已经显示【" + _Count + "】个，您批量添加的个数过多！" }, "text/plain", Encoding.UTF8);
                    }
                    if (_Count <= 128)
                    {
                        _status = 1;
                    }
                    else
                    {
                        messageShow = "因列表已经显示【128】个，因此批量添加中有部分【显示状态】为【隐藏】";
                    }

                    for (int i = 0; i < brandNoS.Length; i++)
                    {
                        //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNoS[i]).BrandEnName;
                        brandName = DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetBrandEnNameByBrandNo", new { BrandNo = brandNoS[i] }).FirstOrDefault();
                        brandIndex = new SWfsBrandIndex()
                        {
                            BrandAddr = "",
                            BrandNo = brandNoS[i],
                            BrandPic = "",
                            BrandShowName = brandName == null ? "" : brandName,
                            ModuleNo = "M011",
                            Sort = 128,
                            Status = _status,
                            TypeId = Convert.ToInt32(typeId),
                            BrandView = 2,
                            DateCreate = DateTime.Now
                        };
                        brandService.SaveBrandIndex(brandIndex);
                    }
                    return Json(new { result = 1, message = messageShow }, "text/plain", Encoding.UTF8);
                }
            }
            #endregion

            string brandNo = string.Empty;
            string brandPic = string.Empty;
            string brandAddr = string.Empty;
            string type = Request.Form["Type"];
            string brandStatus = Request.Form["BrandVisible"];
            string brandModule = Request.Form["moduleName"];
            string brandIndexId = Request.Form["brandIndexId"];
            if (!string.IsNullOrEmpty(brandIndexId))
            {
                brandIndex = DapperUtil.QueryByIdentity<SWfsBrandIndex>(brandIndexId);
            }

            try
            {
                /*
                 *   首页品牌改版， BrandView字段 值为 2 品牌首页管理  
                 *   【热门】对应【logo墙】； 
                 *   【旗舰店】对应【广告位】
                 */
                int _Count = 0;
                int _status = 0;
                if (!string.IsNullOrEmpty(brandIndexId))
                {
                    #region 编辑

                    // 走之前的逻辑处理，不影响【国际时尚品牌】
                    if (brandIndex.BrandView == 0 || brandIndex.BrandView == 1)
                    {
                        #region 国际时尚品牌处理
                        if (type == "1")
                        {
                            brandName = Request.Form["BrandIndexName"];
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            string pictureSize = AppSettingManager.AppSettings["BrandIndexPic"];
                            //if (string.IsNullOrEmpty(brandIndexId))
                            //{
                            if (Request.Files["BrandIndexPic"] != null)
                            {
                                string brandIndexPicNo = SaveImg(Request.Files["BrandIndexPic"], pictureSize);
                                if (!string.IsNullOrEmpty(brandIndexPicNo) && brandIndexPicNo.IndexOf("请") == -1)
                                {
                                    brandPic = brandIndexPicNo;
                                }
                                else
                                {
                                    return Json(new { result = "BrandIndexPicUp", message = brandIndexPicNo }, "text/plain", Encoding.UTF8);
                                }
                            }
                            else
                            {
                                brandPic = brandIndex != null ? brandIndex.BrandPic : "";
                            }
                            brandAddr = Request.Form["BrandIndexLink"];
                        }
                        else
                        {
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            brandName = DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetBrandEnNameByBrandNo", new { BrandNo = brandNo }).FirstOrDefault();
                        }
                        _status = int.Parse(brandStatus);
                        #endregion
                    }
                    else if (brandIndex.BrandView == 2)    // 【品牌首页管理】处理
                    {
                        #region 热门品牌
                        if (brandIndex.TypeId == 2)        // typeid 为【热门品牌】
                        {
                            _status = int.Parse(brandStatus);
                            if (_status == 1 && brandIndex.Status == 0)
                            {
                                _Count = brand.GetCountShowStatusByBrandView(Convert.ToInt32(brandIndex.TypeId));
                                if (_Count >= 128)
                                {
                                    return Json(new { result = "-1", message = "当前列表已经显示【128】，请重新选择状态！" }, "text/plain", Encoding.UTF8);
                                }
                            }
                            brandModule = "M011";
                            brandNo = Request.Form["BrandNo"];
                            if (brandNo != brandIndex.BrandNo)
                            {
                                int _IsExist = brand.GetIsExistBrandNoByBrandNoAndBrandView(brandNo, Convert.ToInt32(brandIndex.TypeId));
                                if (_IsExist > 0)
                                {
                                    return Json(new { result = -1, message = "品牌已经存在！" });
                                }
                            }
                           
                            brandName = DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetBrandEnNameByBrandNo", new { BrandNo = brandNo }).FirstOrDefault();
                            type = "2";
                            brandIndex.BrandView = 2;
                        }
                        #endregion

                        #region 旗舰店品牌

                        else if (brandIndex.TypeId == 1) // typeid 为【旗舰店品牌】
                        {
                            _status = int.Parse(brandStatus);
                            if (_status == 1 && brandIndex.Status == 0)
                            {
                                _Count = brand.GetCountShowStatusByBrandView(Convert.ToInt32(brandIndex.TypeId));
                                if (_Count >= 12)
                                {
                                    return Json(new { result = "-1", message = "当前列表已经显示【12】，请重新选择状态！" }, "text/plain", Encoding.UTF8);
                                }
                            }
                            brandModule = "M011";
                            brandName = Request.Form["BrandIndexName"];
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            string pictureSize = AppSettingManager.AppSettings["BrandFlagshipPic"];
                            //if (string.IsNullOrEmpty(brandIndexId))
                            //{
                            if (Request.Files["BrandIndexPic"] != null)
                            {
                                string brandIndexPicNo = SaveImg(Request.Files["BrandIndexPic"], pictureSize, ".jpg");
                                if (!string.IsNullOrEmpty(brandIndexPicNo) && brandIndexPicNo.IndexOf("请") == -1)
                                {
                                    brandPic = brandIndexPicNo;
                                }
                                else
                                {
                                    return Json(new { result = "BrandIndexPicUp", message = brandIndexPicNo }, "text/plain", Encoding.UTF8);
                                }
                            }
                            else
                            {
                                brandPic = brandIndex != null ? brandIndex.BrandPic : "";
                            }
                            brandAddr = Request.Form["BrandIndexLink"];
                            //if (!string.IsNullOrEmpty(brandAddr))
                            //{
                            //    brandAddr = "http://" + brandAddr;
                            //}
                            type = "1";
                            brandIndex.BrandView = 2;
                        }

                        #endregion
                    }
                    brandIndex.BrandAddr = brandAddr == null ? "" : brandAddr;
                    brandIndex.BrandNo = brandNo;
                    brandIndex.BrandPic = brandPic;
                    brandIndex.BrandShowName = brandName == null ? "" : brandName;
                    brandIndex.ModuleNo = brandModule;
                    brandIndex.Status = _status;
                    brandIndex.TypeId = int.Parse(type);
                    brandIndex.DateCreate = DateTime.Now;
                    #endregion
                }
                else
                {
                    #region 添加

                    string CreateBV = Request.Form["hidCreateBrandView"];
                    string CreateTI = Request.Form["hidCreateTypeId"];
                    string _brandView = CreateBV;
                    int _Sort = 0;
                    int _BrandView = 0;
                    if (string.IsNullOrEmpty(CreateBV))
                    {
                        #region 国际时尚品牌处理
                        _Sort = 99;
                        if (type == "1")
                        {
                            brandName = Request.Form["BrandIndexName"];
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            string pictureSize = AppSettingManager.AppSettings["BrandIndexPic"];
                            //if (string.IsNullOrEmpty(brandIndexId))
                            //{
                            if (Request.Files["BrandIndexPic"] != null)
                            {
                                string brandIndexPicNo = SaveImg(Request.Files["BrandIndexPic"], pictureSize);
                                if (!string.IsNullOrEmpty(brandIndexPicNo) && brandIndexPicNo.IndexOf("请") == -1)
                                {
                                    brandPic = brandIndexPicNo;
                                }
                                else
                                {
                                    return Json(new { result = "BrandIndexPicUp", message = brandIndexPicNo }, "text/plain", Encoding.UTF8);
                                }
                            }
                            else
                            {
                                brandPic = brandIndex != null ? brandIndex.BrandPic : "";
                            }
                            brandAddr = Request.Form["BrandIndexLink"];
                        }
                        else
                        {
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            brandName = DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetBrandEnNameByBrandNo", new { BrandNo =brandNo }).FirstOrDefault();
                        }
                        _status = int.Parse(brandStatus);
                        #endregion
                    }
                    else if (CreateBV == "2")
                    {
                        #region 热门品牌
                        if (CreateTI == "2")
                        {
                            _status = int.Parse(brandStatus);
                            if (_status == 1)
                            {
                                _Count = brand.GetCountShowStatusByBrandView(Convert.ToInt32(CreateTI));
                                if (_Count >= 128)
                                {
                                    return Json(new { result = "-1", message = "当前列表已经显示【128】，请重新选择状态！" }, "text/plain", Encoding.UTF8);
                                }
                            }
                            brandModule = "M011";
                            type = "2";
                            _BrandView = 2;
                            _Sort = 128;
                            brandNo = Request.Form["BrandNo"];
                            int _IsExist = brand.GetIsExistBrandNoByBrandNoAndBrandView(brandNo, Convert.ToInt32(CreateBV));
                            if (_IsExist > 0)
                            {
                                return Json(new { result = -1, message = "品牌已经存在！" });
                            }
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            brandName = DapperUtil.Query<string>("ComBeziWfs_SWfsBrand_GetBrandEnNameByBrandNo", new { BrandNo = brandNo }).FirstOrDefault();
                        }
                        #endregion

                        #region 旗舰店品牌
                        else if (CreateTI == "1")
                        {
                            _status = int.Parse(brandStatus);
                            if (_status == 1)
                            {
                                _Count = brand.GetCountShowStatusByBrandView(Convert.ToInt32(CreateTI));
                                if (_Count >= 12)
                                {
                                    return Json(new { result = "-1", message = "当前列表已经显示【12】，请重新选择状态！" }, "text/plain", Encoding.UTF8);
                                }
                            }
                            brandModule = "M011";
                            type = "1";
                            _BrandView = 2;
                            _Sort = 12;
                            brandName = Request.Form["BrandIndexName"];
                            brandNo = Request.Form["BrandNo"];
                            //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                            string pictureSize = AppSettingManager.AppSettings["BrandFlagshipPic"];
                            //if (string.IsNullOrEmpty(brandIndexId))
                            //{
                            if (Request.Files["BrandIndexPic"] != null)
                            {
                                string brandIndexPicNo = SaveImg(Request.Files["BrandIndexPic"], pictureSize, ".jpg");
                                if (!string.IsNullOrEmpty(brandIndexPicNo) && brandIndexPicNo.IndexOf("请") == -1)
                                {
                                    brandPic = brandIndexPicNo;
                                }
                                else
                                {
                                    return Json(new { result = "BrandIndexPicUp", message = brandIndexPicNo }, "text/plain", Encoding.UTF8);
                                }
                            }
                            else
                            {
                                brandPic = brandIndex != null ? brandIndex.BrandPic : "";
                            }
                            brandAddr = Request.Form["BrandIndexLink"];
                            //if (!string.IsNullOrEmpty(brandAddr))
                            //{
                            //    brandAddr = "http://" + brandAddr;
                            //}
                        }
                        #endregion
                    }
                    brandIndex = new SWfsBrandIndex()
                    {
                        BrandAddr = brandAddr == null ? "" : brandAddr,
                        BrandNo = brandNo,
                        BrandPic = brandPic,
                        BrandShowName = brandName == null ? "" : brandName,
                        ModuleNo = brandModule,
                        Sort = _Sort,
                        Status = _status,
                        TypeId = int.Parse(type),
                        DateCreate = DateTime.Now,
                        BrandView = _BrandView
                    };
                    #endregion
                }


                int result = brandService.SaveBrandIndex(brandIndex);
                return Json(new { result = 1, message = messageShow }, "text/plain", Encoding.UTF8);
            }
            catch
            {
                return Json(new { result = -1, message = "新建品牌失败！" }, "text/plain", Encoding.UTF8);
            }
        }
        #endregion

        #region 品牌预览
        public ActionResult BrandPreviewList(string brandNo)
        {
            SWfsBrandService bll = new SWfsBrandService();
            var list = bll.BrandPreview(brandNo);
            return View(list);
        }
        #endregion

        #region 初始化品牌数据
        //初始化品牌数据
        public ActionResult InitialData()
        {
            SWfsBrandService bll = new SWfsBrandService();
            bll.InitialData();
            return Redirect("AIIBrandsSelect.html" + CommonService.GetTimeStamp("?"));
        }
        #endregion

        #region 轮播图

        #region 品牌轮播图管理
        public ActionResult AlterPicList(int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            ViewBag.list = AlterPicLists(pageIndex, pageSize, out count);
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalCount = count;
            return View();
        }
        //轮播图
        public IEnumerable<SWfsAlterPic> AlterPicLists(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            if (Request.QueryString["AlterPicView"] != "-1" && Request.QueryString["AlterPicView"] != null && Request.QueryString["AlterPicView"] != "")
            {
                dic.Add("AlterPicView", int.Parse(Request.QueryString["AlterPicView"]));
            }
            else
            {
                dic.Add("AlterPicView", "");
            }
            if (Request.QueryString["AlterPicPosition"] != "-1" && Request.QueryString["AlterPicPosition"] != null && Request.QueryString["AlterPicPosition"] != "")
            {
                dic.Add("AlterPicPosition", int.Parse(Request.QueryString["AlterPicPosition"]));
            }
            else
            {
                dic.Add("AlterPicPosition", "");
            }
            if (Request.QueryString["AlterPicName"] != "-1" && Request.QueryString["AlterPicName"] != null && Request.QueryString["AlterPicName"] != "")
            {
                dic.Add("AlterPicName", Request.QueryString["AlterPicName"]);
                ViewBag.KeyWord = Request.QueryString["AlterPicName"];
            }
            else
            {
                dic.Add("AlterPicName", "");
                ViewBag.KeyWord = Request.QueryString["AlterPicName"];
            }
            if (Request.QueryString["Status"] != "-1" && Request.QueryString["Status"] != null && Request.QueryString["Status"] != "")
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            IEnumerable<SWfsAlterPic> list = DapperUtil.Query<SWfsAlterPic>("ComBeziWfs_SWfsBrandAlterPic_List", dic, new { AlterPicView = dic["AlterPicView"], AlterPicPosition = dic["AlterPicPosition"], AlterPicName = dic["AlterPicName"], status = dic["Status"], pageIndex = pageIndex, pageSize = pageSize }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandAlterPic_Select", dic, new { AlterPicView = dic["AlterPicView"], AlterPicPosition = dic["AlterPicPosition"], AlterPicName = dic["AlterPicName"], status = dic["Status"] }).First<int>();
            return list;
        }
        #endregion

        #region 修改轮播图状态
        public ActionResult UpdateASWfsAlterPicStatus(int alterPicId, int alterstatus)
        {
            SWfsBrandService bll = new SWfsBrandService();
            bll.UpdateASWfsAlterPicStatus(alterPicId, alterstatus);
            return Redirect("AlterPicList.html?AlterPicView=" + Request.QueryString["AlterPicView"] + "&AlterPicPosition=" + Request.QueryString["AlterPicPosition"] + "&AlterPicName=" + Request.QueryString["AlterPicName"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 删除轮播图
        public ActionResult DeleteAlterPic(int alterPicId)
        {
            SWfsBrandService bll = new SWfsBrandService();
            bll.AlterPicDelete(alterPicId);
            return Redirect("AlterPicList.html?AlterPicView=" + Request.QueryString["AlterPicView"] + "&AlterPicPosition=" + Request.QueryString["AlterPicPosition"] + "&AlterPicName=" + Request.QueryString["AlterPicName"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 添加轮播图
        public ActionResult AlterPicAdd(int id = 0)
        {
            SWfsBrandService service = new SWfsBrandService();
            if (id == 0)
            {
                return View(new SWfsAlterPic() { AlterPicView = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) });
            }
            var dd = service.GetSWfsAlterPicByID(id);
            if (dd == null)
            {
                return View(new SWfsAlterPic() { AlterPicView = 1, StartDate = DateTime.Now, EndDate = DateTime.Now });
            }
            return View(dd);
        }
        [HttpPost]
        public ActionResult AlterPicAdd(SWfsAlterPic obj)
        {
            SWfsBrandService service = new SWfsBrandService();
            obj.CreateDate = DateTime.Now;
            obj.AlterPicName = Request.Form["AlterPicName"];
            obj.AlterPicAddr = Request.Form["AlterPicAddr"];
            obj.Status = Request.Form["Status"] != null ? int.Parse(Request.Form["Status"]) : 0;
            obj.StartDate = DateTime.Parse(Request.Form["StartDate"]);
            obj.EndDate = DateTime.Parse(Request.Form["EndDate"]);

            #region 添加图片
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.AlterPicNo = SaveImg(Request.Files["imgfile"], "width:960,Height:250,Length:100");
                if (rsPic.Keys.Contains("error"))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('" + rsPic["error"] + "')</script>");
                    return View(obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(obj.AlterPicNo))
                {
                    obj.AlterPicNo = "";
                }
            }
            #endregion
            if (obj.AlterPicId == 0)
            {
                if (service.AlterPicInsert(obj) > 0)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加成功'); window.location.href='AlterPicList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('添加失败')</script>");

                }
            }
            else
            {
                if (service.AlterPicUpdate(obj))
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改成功'); window.location.href='AlterPicList.html" + CommonService.GetTimeStamp("?") + "'</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('修改失败')</script>");
                }

            }
            return View(obj);
        }
        #endregion
        #endregion

        #region 国际高端品牌
        #region 列表
        public ActionResult BrandsListGuoJi(int pageIndex = 1, int pageSize = 10)
        {
            SWfsBrandService brand = new SWfsBrandService();
            ViewBag.BrandModuleList = brandService.GetBrandModules(1);
            int count = 0;
            //IList<SWfsBrandIndexInfo> list = brand.GetBrandIndexList(pageIndex, pageSize, out count);
            ViewBag.list = BrandDataBinds(pageIndex, pageSize, out count);
            ViewBag.PageIndex = pageIndex;
            ViewBag.totalcount = count;
            return View();
        }
        //返回总条数
        public IEnumerable<SWfsBrandIndexInfo> BrandDataBinds(int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            string aa = Request.QueryString["TypeId"];
            ViewBag.KeyWord = Request.QueryString["BrandShowName"] == null ? "" : Request.QueryString["BrandShowName"].Trim();
            dic.Add("BrandShowName", Request.QueryString["BrandShowName"] == null ? "" : Request.QueryString["BrandShowName"].Trim());
            if (Request.QueryString["TypeId"] != "-1" && Request.QueryString["TypeId"] != null && Request.QueryString["TypeId"] != "")
            {
                dic.Add("TypeId", int.Parse(Request.QueryString["TypeId"]));
            }
            else
            {
                dic.Add("TypeId", "");
            }

            if (Request.QueryString["Status"] != "-1" && Request.QueryString["Status"] != null && Request.QueryString["Status"] != "")
            {
                dic.Add("Status", int.Parse(Request.QueryString["Status"]));
            }
            else
            {
                dic.Add("Status", "");
            }
            if (Request.QueryString["ModuleName"] != "-1" && Request.QueryString["ModuleName"] != null && Request.QueryString["ModuleName"] != "")
            {
                dic.Add("ModuleName", Request.QueryString["ModuleName"]);
            }
            else
            {
                dic.Add("ModuleName", "");
            }
            //dic.Add("BrandEnName", Request.QueryString["enName"] != null ? Request.QueryString["enName"] : "");
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            IEnumerable<SWfsBrandIndexInfo> query = DapperUtil.Query<SWfsBrandIndexInfo>("ComBeziWfs_Brand_SWfsBrandIndex_Select", dic, new { BrandShowName = dic["BrandShowName"], TypeId = dic["TypeId"], Status = dic["Status"], ModuleName = dic["ModuleName"], pageIndex = pageIndex, pageSize = pageSize });
            count = DapperUtil.Query<int>("ComBeziWfs_Brand_SWfsBrandIndex_Select_Count", dic, new { BrandShowName = dic["BrandShowName"], TypeId = dic["TypeId"], Status = dic["Status"], ModuleName = dic["ModuleName"] }).First<int>();
            return query;
        }
        #endregion

        #region 添加
        public ActionResult BrandGouJiInsert(string brandIndexId)
        {
            var brandModuleList = brandService.GetBrandModules(1);
            ViewBag.BrandModuleList = brandModuleList;
            if (!string.IsNullOrEmpty(brandIndexId))
            {
                ViewBag.BrandIndexID = brandIndexId;
                ViewBag.BrandIndexModel = brandService.GetSwfsBrandIndexByIndexId(brandIndexId);
                ViewBag.BrandModuleType = brandService.GetSwfsBrandIndexByIndexId(brandIndexId).ModuleType;
            }
            return View();
        }
        [HttpPost]
        public JsonResult BrandGouJiInserts()
        {
            SWfsBrandIndex brandIndex = null;

            string brandName = string.Empty;
            string brandNo = string.Empty;
            string brandPic = string.Empty;
            string brandAddr = string.Empty;
            string type = Request.Form["Type"];
            string brandStatus = Request.Form["BrandVisible"];
            string brandModule = Request.Form["moduleName"];
            string brandIndexId = Request.Form["brandIndexId"];
            if (!string.IsNullOrEmpty(brandIndexId))
                brandIndex = DapperUtil.QueryByIdentity<SWfsBrandIndex>(brandIndexId);

            try
            {

                if (type == "1")
                {
                    if (brandModule == "M008")
                    {
                        brandName = Request.Form["BrandName"];
                    }
                    else
                    {
                        brandName = Request.Form["BrandIndexName"];
                    }
                    string pictureSize = AppSettingManager.AppSettings["BrandIndexPic"];
                    brandNo = Request.Form["BrandNo"];
                    //brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                    //if (string.IsNullOrEmpty(brandIndexId))
                    //{
                    if (Request.Files["BrandIndexPic"] != null)
                    {
                        string brandIndexPicNo = SaveImg(Request.Files["BrandIndexPic"], pictureSize);
                        if (!string.IsNullOrEmpty(brandIndexPicNo) && brandIndexPicNo.IndexOf("请") == -1)
                        {
                            brandPic = brandIndexPicNo;
                        }
                        else
                        {
                            return Json(new { result = "BrandIndexPicUp", message = brandIndexPicNo });
                        }
                    }
                    else
                    {
                        brandPic = brandIndex != null ? brandIndex.BrandPic : "";
                    }
                    brandAddr = Request.Form["BrandIndexLink"];
                }
                //else
                //{
                //    brandNo = Request.Form["BrandNo"];
                //    brandName = DapperUtil.QueryByIdentity<WfsBrand>(brandNo).BrandEnName;
                //}


                if (!string.IsNullOrEmpty(brandIndexId))
                {
                    brandIndex.BrandAddr = brandAddr == null ? "" : brandAddr;
                    brandIndex.BrandNo = brandNo;
                    brandIndex.BrandPic = brandPic;
                    brandIndex.BrandShowName = brandName == null ? "" : brandName;
                    brandIndex.ModuleNo = brandModule;
                    brandIndex.Status = int.Parse(brandStatus);
                    brandIndex.TypeId = int.Parse(type);
                    brandIndex.DateCreate = DateTime.Now;

                }
                else
                {
                    brandIndex = new SWfsBrandIndex()
                    {
                        BrandAddr = brandAddr == null ? "" : brandAddr,
                        BrandNo = brandNo,
                        BrandPic = brandPic,
                        BrandShowName = brandName == null ? "" : brandName,
                        ModuleNo = brandModule,
                        Sort = 99,
                        Status = int.Parse(brandStatus),
                        TypeId = int.Parse(type),
                        DateCreate = DateTime.Now,
                        BrandView = 1
                    };
                }
                int result = brandService.SaveBrandIndex(brandIndex);
                return Json(new { result = 1 });
            }
            catch
            {
                return Json(new { result = -1, message = "新建品牌索引失败！" });
            }
        }
        #endregion

        #region 修改状态
        public ActionResult GuoJiUpdateStatus(int indexId, int editestatus)
        {
            SWfsBrandService bll = new SWfsBrandService();
            bll.UpdateStatus(indexId, editestatus);//?BrandShowName=&ModuleName=-1&TypeId=1&Status=-1
            return Redirect("BrandsListGuoJi.html?BrandShowName=" + Request.QueryString["BrandShowName"] + "&ModuleName=" + Request.QueryString["ModuleName"] + "&TypeId=" + Request.QueryString["TypeId"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
        }
        #endregion

        #region 删除
        public ActionResult GuoJiBrandDelete(string indexId)
        {
            SWfsBrandService bll = new SWfsBrandService();
            var list = bll.BrandIndexDelete(indexId);
            return Redirect("BrandsListGuoJi.html?BrandShowName=" + Request.QueryString["BrandShowName"] + "&ModuleName=" + Request.QueryString["ModuleName"] + "&TypeId=" + Request.QueryString["TypeId"] + "&Status=" + Request.QueryString["Status"] + CommonService.GetTimeStamp("&"));
        }
        #endregion
        #endregion




        #region 旗舰店模板管理
        public ActionResult TemplateList(int pageIndex = 1, int pageSize = 20)
        {
            int total = 0;
            IEnumerable<SWfsBrandFlagShipTemplate> list = brandService.GetTemplateList(pageIndex,
                pageSize, Request.QueryString["templatenameorno"], Request.QueryString["templatetype"], Request.QueryString["starttime"], Request.QueryString["endtime"], out total);
            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //编辑模板
        public ActionResult EditeTemplate(int id = 0)
        {
            string tempNo = brandService.GetMaxTemplateNO();//查询最大模板编号
            if (tempNo == null)
            {
                tempNo = DateTime.Now.ToString("yyyyMMdd") + "001";
            }
            else
            {
                int identity = int.Parse(tempNo.Substring(8));
                identity++;
                if (identity == 999)
                {
                    identity = 0;
                }
                tempNo = DateTime.Now.ToString("yyyyMMdd") + ((identity == 999) ? "001" : identity + "");
            }
            if (id == 0)
            {
                return View(new SWfsBrandFlagShipTemplate() { TemplateNO = tempNo });
            }
            SWfsBrandFlagShipTemplate obj = brandService.GetTemplateObjByID(id);
            if (obj == null)
            {
                return View(new SWfsBrandFlagShipTemplate() { TemplateNO = tempNo });
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditeTemplate(SWfsBrandFlagShipTemplate obj)
        {
            int result = brandService.EditeTemplate(obj);
            if (result > 0)
            {
                ViewData["tip"] = new HtmlString("<script>alert('操作成功');location.href='/Shangpin/brand/TemplateList';</script>");
                return View(obj);
            }
            else
            {
                if (result == -1)
                {
                    ViewData["tip"] = new HtmlString("<script>alert('已存在该模板编号');</script>");
                }
                else
                {
                    ViewData["tip"] = new HtmlString("<script>alert('操作失败');</script>");
                }
            }
            return View(obj);
        }
        //删除模板
        public ActionResult DeleteTemplate(int id)
        {
            brandService.DeleteTemplateByID(id);
            return Redirect("/shangpin/brand/TemplateList");
        }
        //模板代码编辑
        [HttpPost]
        public string EditeTemplateContent(int tempID)
        {
            SWfsBrandFlagShipTemplate obj = brandService.GetTemplateObjByID(tempID);
            if (obj == null)
            {
                return null;
            }
            return brandService.GetTemplateContent(obj.TemplatePath);
        }
        //保存模板代码
        [HttpPost, ValidateInput(false)]
        public int SaveTempContent(int tempID, string tempContent)
        {
            SWfsBrandFlagShipTemplate obj = brandService.GetTemplateObjByID(tempID);
            if (obj == null)
            {
                return 0;
            }
            return brandService.SaveTemplateContent(obj.TemplatePath, tempContent);
        }
        #endregion

        #region 移动端旗舰店管理
        //查询旗舰店
        public ActionResult GetMobileFlagShipStore(string brandNO)
        {
            SWfsBrandFlagShipStoreMobile obj = brandService.GetMobileFlagShipStoreObjByBrandNO(brandNO);
            if (obj == null)
            {
                //不存在就插入数据
                brandService.EditeMobileFlagShipStoreByID(new SWfsBrandFlagShipStoreMobile { BrandNO = brandNO });
                obj = brandService.GetMobileFlagShipStoreObjByBrandNO(brandNO);
            }
            return View(obj);
        }
        //根据模板生成HTML
        public ActionResult CreateHTMLByTemplate(string brandNO, string templateNO)
        {
            if (string.IsNullOrEmpty(templateNO) || brandNO == null)
            {
                return Content("0");
            }
            SWfsBrandFlagShipTemplate obj = brandService.GetTemplateObjByTempNO(templateNO);//获取模板
            if (obj == null)
            {
                return Content("0");
            }
            if (string.IsNullOrEmpty(obj.TemplatePath))
            {
                return Content("0");
            }
            if (!System.IO.File.Exists(Server.MapPath(obj.TemplatePath))) //验证模板是否存在
            {
                return Content("0");
            }
            //查询填充模板的数据列表
            IEnumerable<SWfsBrandFlagShipStoreRegion> Opratorlist = brandService.GetRegionRelationInfoByCondition(brandNO, templateNO, 0);
            return View(obj.TemplatePath, Opratorlist);
        }
        //发布旗舰店
        [HttpPost, ValidateInput(false)]
        public int PublishFlagShip(int flagID, string htmlCode)
        {
            return brandService.SaveVenueHtml(flagID, htmlCode);
        }
        //选择模板列表
        public ActionResult SelectMeetingTemplate(int pageIndex = 1, int pageSize = 3)
        {
            int total = 0;
            IEnumerable<SWfsBrandFlagShipTemplate> list = brandService.GetTemplateList(pageIndex,
                pageSize, Request.QueryString["templatenameorno"], Request.QueryString["templatetype"], Request.QueryString["starttime"], Request.QueryString["endtime"], out total);

            ViewBag.totalCount = total;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            return View(list);
        }
        //更新模板编号
        public int EditeFlagTemplateNO(int flagID, string tempNO)
        {
            if (tempNO == null)
            {
                tempNO = "";
            }
            return brandService.EditeMobileFlagShipStoreTempNO(flagID, tempNO);
        }
        #endregion

        #region 区块管理
        //检查尚品活动编号
        public string CheckActiveNO(string activeNO)
        {
            string result = brandService.CheckActiveNO(activeNO);
            if (result == null)
            {
                return "0";
            }
            return result;
        }
        //获取区块关联数据
        public string GetRelationInfo(string templateNO, int flagReigionID, string brandNO, int regionID)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            SWfsBrandFlagShipStoreRegion obj = null;
            IEnumerable<SWfsBrandFlagShipStoreRegion> objlist = brandService.GetRegionRelationInfoByCondition(brandNO, templateNO, regionID);
            if (objlist.Count() > 0)
            {
                obj = objlist.ElementAt(0);
            }
            else
            {
                obj = new SWfsBrandFlagShipStoreRegion { RelationType = 1 };
            }
            return json.Serialize(obj);
        }
        //保存区块
        [HttpPost]
        public string SaveRegionRelationContent(SWfsBrandFlagShipStoreRegion obj)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            #region 数据验证
            if (obj.BrandNO == null || obj.RegionID <= 0 || obj.RelationType <= 0 || string.IsNullOrEmpty(obj.TemplateNo))
            {
                return "{\"status\" : 0, \"message\" : \"数据不合法\"}";
            }
            if (Request.Form["selectedType"] == null)
            {
                return "{\"status\" : 0, \"message\" : \"类别不存在\"}";
            }
            else
            {
                obj.RelationType = short.Parse(Request.Form["selectedType"]);
                if (obj.RelationType == 3)
                {
                    if (string.IsNullOrEmpty(Request.Form["navID"]))
                    {
                        return "{\"status\" : 0, \"message\" : \"请选择手动导航内容\"}";
                    }
                    else
                    {
                        obj.RelationContent = Request.Form["navID"];
                    }
                }
            }
            if (Request.Files["imgfile"] != null && Request.Files["imgfile"].ContentLength > 0)
            {
                obj.ImgNO = SaveImg(Request.Files["imgfile"], "width:0,Height:0,Length:1000");
                if (rsPic.Keys.Contains("error"))
                {
                    return "{\"status\" : 0, \"message\" : \"图片不合法\"}";
                }
            }
            #endregion
            if (!brandService.SaveRegionRelationInfo(obj))
            {
                return "{\"status\" : 0, \"message\" : \"数据保存异常\"}";
            }
            return "{\"status\" : 1, \"message\" : " + json.Serialize(obj) + "}";
        }
        //获取手动导航
        public string GetNavTree(string brandNO, int selectValue = 0)
        {
            SWfsBrandService dal = new SWfsBrandService();
            IList<SWfsBrandNavigation> newlist = new List<SWfsBrandNavigation>();
            dal.CreateDropDownTree(dal.GetNavigateList(brandNO), newlist, 0);
            StringBuilder sb = new StringBuilder("<option value='0'>==请选择==</option>");
            if (newlist.Count() > 0)
            {
                for (int i = 0; i < newlist.Count; i++)
                {
                    if (selectValue == newlist[i].NavigationId)
                    {
                        sb.Append("<option selected='seleced' value='" + newlist[i].NavigationId + "'>" + newlist[i].NavigationName + "</option>");
                    }
                    else
                    {
                        sb.Append("<option value='" + newlist[i].NavigationId + "'>" + newlist[i].NavigationName + "</option>");
                    }
                }
            }
            return sb.ToString();
        }
        //清空轮播图
        public int ClearAlterPic(string brandNO)
        {
            return brandService.ClearMobileAlterPic(brandNO, 99990);
        }
        #endregion


    }
}

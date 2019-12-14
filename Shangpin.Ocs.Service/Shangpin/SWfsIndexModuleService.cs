using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Service.Outlet;
using System.IO;
using System.Web;
using Shangpin.Framework.Common.Cache;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsIndexModuleService
    {
        /// <summary>
        /// 根据楼层id获取楼层对象
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public SWfsIndexModule GetSWfsIndexModuleById(int floorId)
        {
            if (floorId <= 0) return null;
            SWfsIndexModule tempFloor = DapperUtil.Query<SWfsIndexModule>("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleByModuleId", new { ModuleId = floorId }).FirstOrDefault();
            //if (tempFloor != null)
            //    tempFloor = CheckAdShowTypeChangeDate(ref tempFloor);
            return tempFloor;
        }
        /// <summary>
        /// 更新或插入楼层
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SWfsIndexModule InsertOrUpdateSWfsIndexModule(SWfsIndexModule model, Boolean needEntity = false)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (model.ModuleId <= 0)
                DapperUtil.Insert<SWfsIndexModule>(model);
            else
                DapperUtil.Update<SWfsIndexModule>(model);

            if (needEntity && model.ModuleId <= 0)//插入时 需要获取最ID
            {
                var result = DapperUtil.Query<int>("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleIdByCreaterAndCreateTime", new { model.DateCreate, model.OperateUserId }).FilterList();
                model.ModuleId = (result != null && result.Any()) ? result.First() : -1;
            }
            return model;
        }
        /// <summary>
        /// 更新或插入广告图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SWfsOperationPicture InsertOrUpdateSWfsOperationPicture(SWfsOperationPicture model)
        {
            if (model.PictureManageId <= 0)
                DapperUtil.Insert<SWfsOperationPicture>(model);
            else
                DapperUtil.Update<SWfsOperationPicture>(model);
            return model;
        }
        /// <summary>
        /// 逻辑删除楼层关联图片
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public int DeleteSWfsIndexModuleRefPic(int moduleId)
        {
            if (moduleId <= 0) return -1;
            return DapperUtil.Execute("ComBeziWfs_SWfsOperationPicture_DeleteSWfsIndexModuleRefPic", new { ModuleId = moduleId.ToString() });
        }
        /// <summary>
        /// 逻辑删除楼层广告图
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public int DeleteSWfsOperationPictureByPicId(int PictureManageId)
        {
            if (PictureManageId <= 0) return -1;
            return DapperUtil.Execute("ComBeziWfs_SWfsOperationPicture_DeleteSWfsOperationPictureByPicId", new { PictureManageId = PictureManageId });
        }
        /// <summary>
        /// 插入楼层关联上下广告图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertSwfsIndexModuleRefPic(SWfsOperationPicture model)
        {
            return DapperUtil.Insert<SWfsOperationPicture>(model);
        }
        /// <summary>
        /// 根据广告图ID获取一个对象
        /// </summary>
        /// <param name="PictureManageId"></param>
        /// <returns></returns>
        public SWfsOperationPicture GetSWfsOperationPictureByPictureManageId(int PictureManageId)
        {
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByPictureManageId", new { PictureManageId }).FirstOrDefault();
        }
        public IEnumerable<SWfsOperationPicture> GetSWfsindexModuleRefPic(Dictionary<string, object> dic, int pageIndex, int pageSize, out int count)
        {
            IEnumerable<SWfsOperationPicture> query = DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicturePageByPagePositionNo", dic, new
            {
                moduleId = dic["moduleId"].ToString(),
                PictureName = dic["PictureName"] + "",
                DateBegin = dic["DateBegin"] + "",
                DateEnd = dic["DateEnd"] + "",
                pageIndex = pageIndex,
                pageSize = pageSize
            });
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureCountByPagePositionNo", dic, new
            {
                moduleId = dic["moduleId"].ToString(),
                PictureName = dic["PictureName"] + "",
                DateBegin = dic["DateBegin"] + "",
                DateEnd = dic["DateEnd"] + ""
            }).First<int>();
            count = totalCount;
            return query;
        }
        /// <summary>
        /// 获取今日style
        /// </summary>
        /// <returns></returns>
        public SWfsIndexModule GetSWfsIndexModuleByTodayStyle()
        {
            SWfsIndexModule module = new SWfsIndexModule();
            module = DapperUtil.Query<SWfsIndexModule>("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleTodayStyle").FirstOrDefault();
            return module;
        }
        /// <summary>
        /// 更新楼层排序
        /// </summary>
        /// <param name="moduls"></param>
        public int UpdateSWfsIndexModuleSort(List<SWfsIndexModule> moduls)
        {
            try
            {
                foreach (SWfsIndexModule item in moduls)
                {
                    DapperUtil.UpdatePartialColumns<SWfsIndexModule>(new { item.ModuleId, item.Sort, item.Stutas });
                }
            }
            catch (Exception)
            {

                return 0;
            }
            return 1;

        }
        /// <summary>
        /// 获取包含今日style在内的所有楼层
        /// </summary>
        /// <returns></returns>
        public List<SWfsIndexModule> GetAllFloors()
        {
            List<SWfsIndexModule> floors = DapperUtil.Query<SWfsIndexModule>("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleIncludeTodayStyle").FilterList() ?? new List<SWfsIndexModule>();
            for (int i = 0; i < floors.Count; i++)//类型转变时间判定
            {
                SWfsIndexModule temp = floors[i];
                CheckAdShowTypeChangeDate(temp);
            }
            return floors;
        }

        /// <summary>
        /// 判定类型转变时间, 更新到数据库
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public void CheckAdShowTypeChangeDate(SWfsIndexModule module)
        {
            if (module.PageNo == "index" && module.ADShowTypeChangeDate < DateTime.Now && (module.ADShowType == 1 || module.ADShowType == 2) && module.ModuleId > 0)
            {
                module.ADShowTypeChangeDate = Convert.ToDateTime("2099-01-01");
                module.ADShowType = module.ADShowType == 1 ? (short)2 : (short)1;
                DapperUtil.Update<SWfsIndexModule>(module);
                //清除缓存
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsIndexModule_GetSWfsIndexModuleIncludeTodayStyle_GetAllFloors");
            }
            //return module;
        }
        /// <summary>
        /// 获取楼层对应的图片表
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public List<SWfsOperationPicture> GetSWfsOperationPictureByModuleId(int ModuleId)
        {
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByModuleId", new { ModuleId = ModuleId.ToString() }).FilterList();

        }
        /// <summary>
        /// 获取时间大于布局修改时间 开始时间小于现在时间的广告图前2位
        /// </summary>
        /// <param name="module"></param>
        public List<SWfsOperationPicture> GetCurrentSWfsOperationPictureByModuleId(SWfsIndexModule module)
        {
            if (module.ModuleId == 0)
                return new List<SWfsOperationPicture>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic["ADShowType"] = module.ADShowType;
            dic.Add("ADShowType", module.ADShowType);
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetCurrentSWfsOperationPictureByModuleId", dic,
                 new
                 {
                     ModuleId = module.ModuleId,
                     ADShowType = module.ADShowType
                 }
                 ).FilterList() ?? new List<SWfsOperationPicture>();

        }
        /// <summary>
        /// 获取时间大于布局修改时间 开始时间小于现在时间的广告图前100位
        /// </summary>
        /// <param name="module"></param>
        public List<SWfsOperationPicture> GetAllSWfsOperationPictureByModuleId(int moduleId, int ADShowType)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic["ADShowType"] = ADShowType;
            dic.Add("ADShowType", ADShowType);
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetAllSWfsOperationPictureByModuleId", dic,
                 new
                 {
                     ModuleId = moduleId.ToString(),
                     ADShowType = ADShowType
                 }
                 ).FilterList() ?? new List<SWfsOperationPicture>();

        }
        /// <summary>
        /// 检查楼层图片时间重复
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <param name="ADShowTypeChangeDate"></param>
        /// <param name="PictureIndex"></param>
        /// <returns></returns>
        public int CheckModulePictureSameTime(int ModuleId, int PictureIndex, DateTime DateBegin)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            return DapperUtil.Query<int>("ComBeziWfs_SWfsOperationPicture_CheckModulePictureSameTime", dic,
                 new
                 {
                     ModuleId = ModuleId.ToString(),
                     PictureIndex = PictureIndex,
                     DateBegin = DateBegin
                 }
                 ).First();
        }
        /// <summary>
        /// 验证是否广告图有存在相同时间的
        /// </summary>
        /// <returns></returns>
        public int CheckExisTheSameTimePic(string ModuleId, DateTime DateBegin, short PictureIndex)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsOperationPicture_CheckExisTheSameTimePic", new { ModuleId, DateBegin, PictureIndex }).First();
        }

        /// <summary>
        /// 根据分类父ID获取分类集合
        /// </summary>
        /// <param name="ParentNo"></param>
        /// <returns></returns>
        public List<SpfShopCategory> GetSWfsCategoryByParentNo(string ParentNo)
        {
            if (string.IsNullOrEmpty(ParentNo))
                ParentNo = "ROOT";
            var temp = DapperUtil.Query<SpfShopCategory>("ComBeziWfs_SpfShopCategory_GetSWfsCategoryByParentNo", new { ParentNo }).FilterList();
            if (temp == null)
            {
                return new List<SpfShopCategory>();
            }
            return temp;
        }
        #region 图片保存处理

        public Dictionary<string, string> SaveImg(HttpPostedFileBase file, string imgInfo)
        {
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            CommonService commonService = new CommonService();
            if (file.ContentLength > 0)
            {
                rsPic = PostImg(file, imgInfo, new string[] { ".jpg", ".jpeg", ".gif" }, 2);
            }
            return rsPic;
        }
        /// <summary>
        /// 通用保存图片 isArea   0 宽大于等于  1 宽大于等于，高等于  2严格宽高
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="imgSize">width:640,Height:0,Length:50</param>
        /// <returns></returns>
        public Dictionary<string, string> PostImg(HttpPostedFileBase postedFile, string imgSize, string[] allowTypes, int isArea = 0)
        {
            #region 方案二
            //string types = AppSettingManager.AppSettings["InputSystemImgType"];
            //string[] allowTypes = string.IsNullOrEmpty(types) ? (new string[] { ".jpg", ".gif" }) : (types.Split('/'));
            Dictionary<string, string> result = new Dictionary<string, string>();
            string message = string.Empty;
            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
            {
                string fileFullName = postedFile.FileName;
                string fileType = Path.GetExtension(fileFullName).ToLower();
                string fileName = Path.GetFileName(fileFullName);

                if (allowTypes.Contains(fileType))
                {
                    int conLen = postedFile.ContentLength;
                    //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                    byte[] btImgs = new byte[conLen];
                    using (Stream s = postedFile.InputStream)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        s.Position = 0;
                        s.Read(btImgs, 0, conLen);
                    }
                    System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                    System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                    int width = original_image.Width;//图片的宽度
                    int height = original_image.Height;//图片的高度
                    original_image.Dispose();//释放资源
                    long length = stream.Length / 1024;//图片的大小（KB）
                    stream.Close();
                    stream.Dispose();
                    //string pictureSize = AppSettingManager.AppSettings[imgSize];
                    int img_Width = Convert.ToInt32(imgSize.Split(',')[0].Split(':')[1].ToString());
                    int img_Heighth = Convert.ToInt32(imgSize.Split(',')[1].Split(':')[1].ToString());
                    int img_Length = Convert.ToInt32(imgSize.Split(',')[2].Split(':')[1].ToString());
                    if (isArea == 1)//宽大于等于，高等于
                    {
                        if (width < img_Width || height != img_Heighth)
                        {
                            message = "请上传宽大于等于" + img_Width.ToString() + "高等于" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 2)  //严格宽高
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((width != img_Width) || (height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 0)
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((img_Width > 0 && width != img_Width) || (img_Heighth > 0 && height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = fileType;
                    model.FileName = fileName;
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    string pictureNo = service.InserSystemImg(model);

                    result.Add("success", pictureNo);
                    return result;
                }
                else
                {
                    result.Add("error", "上传图片格式不正确！");
                    return result;
                }
            }
            string typestr = allowTypes.Aggregate((a, b) => a + "," + b);
            result.Add("error", "请上传尺寸为" + imgSize + "，格式为" + typestr + "的图片");
            result.Add("error", "请上传图片！");
            return result;
            #endregion
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="picNo"></param>
        /// <returns></returns>
        public int DeleteImg(string picNo)
        {
            return DapperUtil.Execute("ComBeziPic_SystemPictureFile_DeleteByPicNo", new { PictureFileNo = picNo });
        }
        #endregion
    }
}

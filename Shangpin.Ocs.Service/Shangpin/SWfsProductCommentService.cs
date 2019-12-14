using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;
using System.Collections;
using Shangpin.Entity.ComBeziPic;
using System.Web;
using Shangpin.Framework.Configuration;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsProductCommentService
    {
        #region 评论
        #region 评论列表
        //获取评论列表 ，先取得商品级别的信息
        public IEnumerable<SWfsProductLevelCommentExtends> GetSWfsProductLevelCommentList(string brandNO, string categoryNo, string keyword, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProductComment_SelectSWfsProductLevelCommentListCount", dic, new { KeyWord = keyword, BrandNO = brandNO, CategoryNo = categoryNo }).FirstOrDefault();
            var commentLevelList = DapperUtil.Query<SWfsProductCommentExtends>("ComBeziWfs_SWfsProductComment_SelectSWfsProductLevelCommentList", dic, new { KeyWord = keyword, BrandNO = brandNO, CategoryNo = categoryNo, pageIndex = pageIndex, pageSize = pageSize }).ToList();
            return null;
        }

        //获评论列表
        public IEnumerable<SWfsProductCommentExtends> GetSWfsProductCommentList(string status, string brandNO, string categoryNo, string keyword, string starLevel, string commentKeyWord, string userName, string startTime, string endTime, string orderNo, string isResult, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("StarLevel", starLevel == null ? "" : starLevel);
            dic.Add("CommentKeyWord", commentKeyWord == null ? "" : commentKeyWord);
            dic.Add("UserName", userName == null ? "" : userName);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("OrderNo", orderNo == null ? "" : orderNo);
            dic.Add("IsResult", isResult == null ? "" : isResult);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProductComment_SelectSWfsProductCommentCount", dic, new { Status = status, KeyWord = keyword, BrandNO = brandNO, StarLevel = starLevel, CategoryNo = categoryNo, CommentKeyWord = commentKeyWord, UserName = userName, StartTime = startTime, EndTime = endTime, OrderNo = orderNo, IsResult = isResult }).FirstOrDefault();
            var commentList = DapperUtil.Query<SWfsProductCommentExtends>("ComBeziWfs_SWfsProductComment_SelectSWfsProductCommentList", dic, new { Status = status, KeyWord = keyword, BrandNO = brandNO, StarLevel = starLevel, CategoryNo = categoryNo, CommentKeyWord = commentKeyWord, UserName = userName, StartTime = startTime, EndTime = endTime, OrderNo = orderNo, IsResult = isResult, pageIndex = pageIndex, pageSize = pageSize }).ToList();
            foreach (var item in commentList)
            {
                if(string.IsNullOrEmpty(item.NickName))
                {
                    if (string.IsNullOrEmpty(item.UserName))
                    {
                        item.NickName = item.MailAddress;
                    }
                    else
                    {
                        item.NickName = item.UserName;
                    }
                }
                if(string.IsNullOrEmpty(item.LevelName))
                {
                    switch(item.UserLevel)
                    {
                        case "0000":
                            item.LevelName = "注册用户";
                            break;
                        case "0001":
                            item.LevelName = "正式会员";
                            break;
                        case "0002":
                            item.LevelName = "黄金会员";
                            break;
                        case "0003":
                            item.LevelName = "白金会员";
                            break;
                        case "0004":
                            item.LevelName = "钻石会员";
                            break;
                    }
                }
                item.CommentPictures = GetProductCommentPicRef(item.CommentId);
            }
            return commentList;
        }
        #endregion
        /// <summary>
        /// 根据评论列表获取评论图片列表
        /// </summary>
        /// <param name="commentInfos">评论列表</param>
        /// <returns></returns>
        public List<SWfsProductCommentPicRef> GetProductCommentPicRef(int commentId)
        {
            var commentPicRef = DapperUtil.Query<SWfsProductCommentPicRef>("ComBeziWfs_SWfsProductCommentPicRef_SelectListForComment", new { CommentId = commentId }).FilterList();

            return commentPicRef;
        }

        #region 修改评论显示状态
        public bool UpdateASWfsProductCommentStatus(int verify, int commentId, int status)
        {
            if (status == 0)
            { //隐藏状态，已经审核通过，某种原因把它隐藏了。所以修改为审核通过显示状态
                return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, Status = 1 });
            }
            else if (status == 1)
            { //显示状态，只能隐藏
                return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, Status = 0 });
            }
            else
            {//待审核状态：审核通过修改为审核通过并显示状态，如果不通过状态不变（待审核）
                if (verify == 1)
                {
                    return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, Status = 1 });
                }
            }
            UpdateSWfsProductCommentOperateUserId(commentId, PresentationHelper.GetPassport().UserName);
            //return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, Status = status });
            return false;
        }
        #endregion

        #region 修改操作者名称
        public bool UpdateSWfsProductCommentOperateUserId(int commentId, string userName)
        {
            return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, OperateUserId = userName });
        }
        #endregion

        #region 获取评论数据 没有 则只返回一条
        /// <summary>
        /// 获取评论数据 没有 则只返回一条
        /// </summary>
        /// <param name="skuNo"></param>
        /// <returns></returns>
        public List<ProductCommentEntity> GetProductDetailOrCommentDetailBySku(string skuNo)
        {
            List<ProductCommentEntity> list = new List<ProductCommentEntity>();
            if (!string.IsNullOrEmpty(skuNo))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                list = DapperUtil.Query<ProductCommentEntity>("ComBeziWfs_SWfsProductComment_GetProductDetailOrCommentDetailBySku", dic, new { skuNo = skuNo }).ToList();
            }
            return list;
        } 
        #endregion
        #region 后台假评论
        /// <summary>
        /// 添加用户评论
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int AddReview(SWfsProductComment content)
        {
            int returnValue = 0;
            try
            {
                returnValue = DapperUtil.Insert<SWfsProductComment>(content);
            }
            catch (Exception)
            {
                throw;
            }
            return returnValue;
        }
        public void InsertPicFile(UserPictureFile model)
        {
            DapperUtil.Insert<UserPictureFile>(model);

        }
        /// <summary>
        /// 根据商品号 订单号 创建时间 查询一条订单
        /// </summary>
        /// <param name="productNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="DateCreate"></param>
        /// <returns></returns>
        public SWfsProductComment GetCommentByProductNoAndOrderNoAndDate(string productNo, string orderNo, DateTime DateCreate)
        {
            return DapperUtil.Query<SWfsProductComment>("ComBeziWfs_WfsOrder_GetCommentByProductNoAndOrderNoAndDate", new { productNo, orderNo, DateCreate }).FirstOrDefault();
        }
        /// <summary>
        /// 保存评论项
        /// </summary>
        /// <param name="items"></param>
        public void AddCommentItem(List<SWfsProductCommentItem> items)
        {
            try
            {
                foreach (var item in items)
                {
                    DapperUtil.Insert<SWfsProductCommentItem>(item);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void ComputWidthHeight(ref int width, ref int height)
        {
            #region 图片缩放尺寸算法
            float _height = 960;
            float _width = 640;
            float rate = 1;
            if (width > height && width > _width)
                rate = _width / Convert.ToSingle(width);
            if (height > width && height > _height)
            {
                rate = _height / Convert.ToSingle(height);
            }
            width = Convert.ToInt32(width * rate);
            height = Convert.ToInt32(height * rate);
            #endregion
        }
        /// <summary>
        /// 保存文件 返回对象
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public UserPictureFile SavePostedFile(HttpPostedFile file)
        {
            byte[] btImgs = new byte[file.ContentLength];
            file.InputStream.Read(btImgs, 0, btImgs.Length);
            ///压缩图片 
            System.Drawing.Image original_image = System.Drawing.Image.FromStream(file.InputStream);//使用流创建图片
            int width = original_image.Width;//图片的宽度
            int height = original_image.Height;//图片的高度
            ComputWidthHeight(ref width, ref  height);
            System.Drawing.Image newImg = PictureFileConverter.GetThumbnailImage("", width, height, file.InputStream);
            byte[] newBytes = PictureFileConverter.ImageToByteArray(newImg, System.Drawing.Imaging.ImageFormat.Jpeg);
            string fileBase64Str = Convert.ToBase64String(newBytes);
            newImg.Dispose();//释放
            file.InputStream.Dispose();//释放
            original_image.Dispose();//释放
            ///压缩图片 结束

            UserPictureFile model = new UserPictureFile();
            model.FileContent = fileBase64Str;
            model.Extension = file.ContentType;
            model.FileName = file.FileName;
            model.OperatorId = string.Empty;
            model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
            InsertPicFile(model);
            return model;
        }
        /// <summary>
        /// 配置文件值判断
        /// </summary>
        /// <param name="optionstr"></param>
        /// <returns></returns>
        public Boolean CheckInputValueType(string[] commids, string[] commvalues, ref string error)
        {
            Boolean result = true;
            if (commids.Length == 0 || commvalues.Length == 0) return false;
            for (int i = 0; i < commids.Length; i++)
            {
                var flag = true;
                CommentItemOption option = CommentItemManager.AllCommentItemOption.FirstOrDefault(a => a.Id.ToString() == commids[i]);
                if (commvalues[i].Length == 0 && option.Required != 1)
                {
                    continue;
                }//非空判断
                else if (commvalues[i].Length == 0 && option.Required == 1)
                {
                    error += "," + option.Name + "不能为空";

                }
                if (option != null && commvalues[i].Length > 0)
                {
                    switch (option.ValueType)
                    {
                        case "int":
                            {
                                int temp = 0;
                                if (!int.TryParse(commvalues[i], out temp))
                                {
                                    error += "," + option.Name + "只能输入整数";
                                    flag = false;
                                }
                            }; break;
                        case "float":
                            {
                                float temp = 0;
                                if (!Single.TryParse(commvalues[i], out temp))
                                {
                                    error += "," + option.Name + "只能输入整数或小数";
                                    flag = false;
                                }
                            }; break;
                    }
                }
                if (!flag) result = false;
            }
            if (error.Length > 0)
                error = error.Substring(1);
            return result;
        }
        /// <summary>
        /// 批量添加评论项
        /// </summary>
        /// <param name="commids"></param>
        /// <param name="commvalues"></param>
        /// <param name="commentid"></param>
        public void AddCommentItem(string[] commids, string[] commvalues, int commentid)
        {
            List<SWfsProductCommentItem> items = new List<SWfsProductCommentItem>();
            for (int i = 0; i < commids.Length; i++)
            {
                SWfsProductCommentItem item = new SWfsProductCommentItem
                {
                    CommentId = commentid,
                    CreateDate = DateTime.Now,
                    ItemType = Convert.ToInt16(commids[i]),
                    ObjectName = CommentItemManager.AllCommentItemOption.First(a => a.Id.ToString() == commids[i]).Name,
                    ObjectValue = commvalues[i]
                };
                items.Add(item);
            }
            AddCommentItem(items);
        }
        /// <summary>
        /// 保存图片关联
        /// </summary>
        /// <param name="items"></param>
        public void AddSWfsProductCommentPicRef(List<SWfsProductCommentPicRef> items)
        {
            try
            {
                foreach (var item in items)
                {
                    DapperUtil.Insert<SWfsProductCommentPicRef>(item);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region 根据Id获取评论
        public SWfsProductComment GetCommentById(int commentId)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsProductComment>(commentId);
        }
        #endregion

        #region 更新评论回复内容
        public bool UpdateCommentRContentById(string commentId, string commentRContent, string reType)
        {
            //return DapperUtil.Execute("ComBeziWfs_SWfsProductComment_UpdateCommentRContent", new { CommentId = commentId, CommentRContent = commentRContent});
            if (reType == "result")
            {
                return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, ResultCode = commentRContent, ResultDate = DateTime.Now, ResultNo = PresentationHelper.GetPassport().UserName });
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsProductComment>(new { CommentId = commentId, CommentRContent = commentRContent, CommentRDate = DateTime.Now, OperateUserId = PresentationHelper.GetPassport().UserName });
            }

        }
        public List<SWfsUserLevel> GetUserLevel()
        {
            return DapperUtil.Query<SWfsUserLevel>("ComBeziWfs_SWfsUserLevel_GetAllUserLevel").FilterList();
        }
        #endregion

        #region 修改配置
        /// <summary>
        /// 读取配置值
        /// </summary>
        /// <param name="functionNo">功能编号，固定</param>
        /// <param name="configValue">配置值</param>
        /// <returns></returns>
        public bool UpdateGlobalConfig(string functionNo, string configValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsGlobalConfig>(new { FunctionNo = functionNo, ConfigValue = configValue });
        }

        public SWfsGlobalConfig GetGlobalConfig(string functionNo)
        {
            return DapperUtil.QueryByIdentity<SWfsGlobalConfig>("FC01");
        }
        #endregion

        #region 获取当前配置
        public SWfsGlobalConfig GetGlobalConfigByFNo(string functionNo)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsGlobalConfig>(functionNo);
        }
        #endregion
        #endregion

        #region 咨询
        //插入一条新的咨询
        public int InsertSWfsQuestAnswer(SWfsQuestAnswer QA)
        {
            UpdateSWfsQuestAnswerOperateUserId(QA.QuestAnswerId, PresentationHelper.GetPassport().UserName);
            return DapperUtil.Insert<SWfsQuestAnswer>(QA, false);
        }

        //更新一条新的咨询
        public bool UpdateSWfsQuestAnswer(SWfsQuestAnswer QA)
        {
            UpdateSWfsQuestAnswerOperateUserId(QA.QuestAnswerId, PresentationHelper.GetPassport().UserName);
            return DapperUtil.Update<SWfsQuestAnswer>(QA);
        }

        //修改咨询前台显示状态
        public bool UpdateSWfsQuestAnswerIsShow(int questAnswerId, int isShow)
        {
            UpdateSWfsQuestAnswerOperateUserId(questAnswerId, PresentationHelper.GetPassport().UserName);
            return DapperUtil.UpdatePartialColumns<SWfsQuestAnswer>(new { QuestAnswerId = questAnswerId, IsShow = isShow });
        }
        //修改操作者名称
        public bool UpdateSWfsQuestAnswerOperateUserId(int questAnswerId, string userName)
        {
            return DapperUtil.UpdatePartialColumns<SWfsQuestAnswer>(new { QuestAnswerId = questAnswerId, OperateUserId = userName });
        }
        //删除咨询表
        public int DeleteSWfsQuestAnswer(string questAnswerId)
        {
            if (DeleteSWfsQuestAnswerRef(questAnswerId) > 0 || GetSWfsSWfsQuestAnswerRefListByQuestAnswerId(questAnswerId).Count() == 0)
            {
                UpdateSWfsQuestAnswerOperateUserId(Convert.ToInt32(questAnswerId), PresentationHelper.GetPassport().UserName);
                return DapperUtil.Execute("ComBeziWfs_SWfsProductQuestAnswer_DeleteSWfsProductQuestAnswer_Delete", new { QuestAnswerId = questAnswerId });
            }
            return 0;
        }

        //插入新的咨询关联
        public int InsertSWfsQuestAnswerRef(SWfsQuestAnswerRef QAR)
        {
            if (UpdateSWfsQuestAnswerOperateUserId(QAR.QuestAnswerId, PresentationHelper.GetPassport().UserName))
            {
                return DapperUtil.Insert<SWfsQuestAnswerRef>(QAR, false);
            }
            return -1;
        }

        //删除咨询关联表根据questAnswerId
        public int DeleteSWfsQuestAnswerRef(string questAnswerId)
        {
            UpdateSWfsQuestAnswerOperateUserId(Convert.ToInt32(questAnswerId), PresentationHelper.GetPassport().UserName);
            return DapperUtil.Execute("ComBeziWfs_SWfsProductQuestAnswerRef_DeleteSWfsProductQuestAnswerRef_Delete", new { QuestAnswerId = questAnswerId });
        }

        public int DeleteSWfsQuestAnswerRefById(string Id, string questAnswerId)
        {
            UpdateSWfsQuestAnswerOperateUserId(Convert.ToInt32(questAnswerId), PresentationHelper.GetPassport().UserName);
            return DapperUtil.Execute("ComBeziWfs_SWfsProductQuestAnswerRef_DeleteSWfsProductQuestAnswerRef_DeleteById", new { Id = Id });
        }

        public SWfsQuestAnswer GetSWfsSWfsQuestAnswerById(string questAnswerId)
        {
            if (!string.IsNullOrEmpty(questAnswerId))
            {
                return DapperUtil.Query<SWfsQuestAnswer>("ComBeziWfs_SWfsProductQuestAnswer_SelectSWfsProductQuestAnswerById", new { QuestAnswerId = questAnswerId }).FirstOrDefault();
            }
            return null;
        }

        public IEnumerable<SWfsQuestAnswer> GetSWfsSWfsQuestAnswerList(string productKeywords, string questKeywords, string answerKeywords, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ProductKeywords", productKeywords == null ? "" : productKeywords);
            dic.Add("QuestKeywords", questKeywords == null ? "" : questKeywords);
            dic.Add("AnswerKeywords", answerKeywords == null ? "" : answerKeywords);
            total = 0;
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProductQuestAnswer_SelectSWfsProductQuestAnswerCount", dic, new { ProductKeywords = productKeywords, QuestKeywords = questKeywords, AnswerKeywords = answerKeywords }).FirstOrDefault();
            return DapperUtil.Query<SWfsQuestAnswer>("ComBeziWfs_SWfsProductQuestAnswer_SelectSWfsProductQuestAnswerList", dic, new { ProductKeywords = productKeywords, QuestKeywords = questKeywords, AnswerKeywords = answerKeywords, pageIndex = pageIndex, pageSize = pageSize });
        }
        public IEnumerable<SWfsQuestAnswerExtends> GetSWfsSWfsQuestAnswerRefList()
        {
            return DapperUtil.Query<SWfsQuestAnswerExtends>("ComBeziWfs_SWfsProductQuestAnswerRef_SelectSWfsProductQuestAnswerRefList", null);
        }

        public IEnumerable<SWfsQuestAnswerExtends> GetSWfsSWfsQuestAnswerRefListByQuestAnswerId(string questAnswerId)
        {
            return DapperUtil.Query<SWfsQuestAnswerExtends>("ComBeziWfs_SWfsProductQuestAnswerRef_SelectSWfsProductQuestAnswerRefListByQuestAnswerId", new { QuestAnswerId = questAnswerId });
        }
        public SWfsQuestAnswerExtends CheckQuestAnswerRefByNos(string productNo, string categoryNo, string brandNo, string questAnswerId)
        {
            return DapperUtil.Query<SWfsQuestAnswerExtends>("ComBeziWfs_SWfsProductQuestAnswerRef_SelectSWfsProductQuestAnswerRefByNos", new { BrandNo = brandNo, ProductNo = productNo, CategoryNo = categoryNo, QuestAnswerId = questAnswerId }).FirstOrDefault();
        }
        #endregion

        public void BetchInsertComment(string sql)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("SQL", sql);
            var abc = DapperUtil.Query<int>("ComBeziWfs_SWfsProductComment_BetchInsertComment", dic);
        }
    }
}

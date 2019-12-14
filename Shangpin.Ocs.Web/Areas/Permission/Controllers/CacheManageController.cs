using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Common;
using System.Threading.Tasks;
using Shangpin.Framework.Common.Cache;

namespace Shangpin.Ocs.Web.Areas.Permission.Controllers
{
    public class CacheManageController : Controller
    {
        //
        // GET: /Permission/CacheManage/

        public ActionResult Clear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoClear(FormCollection from)
        {
            string member = from["Member"].ToString();
            string redis = from["Redis"].ToString();
            string nginx = from["Nginx"].ToString();
            string sohuyun = from["SoHuYun"].ToString();
            //DoMember(member);
            //DoRedis(redis);
            //DoNginx(nginx);
            var task1 = Task.Factory.StartNew(() => DoMember(member));
            var task2 = Task.Factory.StartNew(() => DoRedis(redis));
            var task3 = Task.Factory.StartNew(() => DoNginx(nginx));
            var task4 = Task.Factory.StartNew(() => DoSoHuYun(sohuyun));
            Task.WaitAll(task1, task2, task3, task4);

            return Json(new { result = "1", message = "操作成功" });
        }

        [HttpPost]
        public ActionResult DoWrite(FormCollection from)
        {

            string keyType = from["keyType"].ToString();
            string keyName = from["keyName"].ToString();
            string keyValue = from["keyValue"].ToString();
            if (string.IsNullOrWhiteSpace(keyName) || string.IsNullOrWhiteSpace(keyValue))
            {
                return Json(new { result = "1", message = "请填写正确的键值" });
            }

            switch (keyType)
            {
                case "1":
                    MemcachedProvider.Instance.Set(keyName, keyValue, DateTime.Now.AddHours(10));
                    return Json(new { result = "1", message = "操作成功" });

                case "2":

                    RedisCacheProvider.Instance.Set(keyName, keyValue, TimeSpan.FromHours(10));//?????
                    return Json(new { result = "1", message = "操作成功" });

                case "3":
                    return Json(new { result = "1", message = "此环境不支持Nginx/CDN测试" });


                default:
                    return Json(new { result = "1", message = "请选择写入缓存类型" });
            }

        }

        [HttpPost]
        public ActionResult DoRead(FormCollection from)
        {

            string keyType = from["keyType"].ToString();
            string keyName = from["keyName"].ToString();
            string keyValue = from["keyValue"].ToString();
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return Json(new { result = "1", message = "请填写正确的键值" });
            }
            string t = string.Empty;
            switch (keyType)
            {
                case "1":
                    t = (null == MemcachedProvider.Instance.Get(keyName)) ? "缓存不存在" : MemcachedProvider.Instance.Get(keyName).ToString();
                    return Json(new { result = "1", message = t });

                case "2":


                    t = RedisCacheProvider.Instance.Get<string>(keyName);
                    return Json(new { result = "1", message = t ?? "缓存不存在" });

                case "3":
                    return Json(new { result = "1", message = "此环境不支持Nginx/CDN测试" });


                default:
                    return Json(new { result = "1", message = "请选择写入缓存类型" });
            }

        }

        private void DoMember(string member)
        {
            if (!string.IsNullOrWhiteSpace(member))
            {
                CacheService service = new CacheService();
                string[] key = member.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                service.MemberCacheDeleteAll(key);
            }
        }

        private void DoRedis(string redis)
        {
            if (!string.IsNullOrWhiteSpace(redis))
            {
                CacheService service = new CacheService();
                string[] key = redis.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                service.RedisCacheDeleteAll(key);
            }
        }

        private void DoNginx(string nginx)
        {
            if (!string.IsNullOrWhiteSpace(nginx))
            {
                CacheService service = new CacheService();
                string[] url = nginx.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in url)
                {
                    service.ClarNginxCache(item);
                }
            }
        }

        private void DoSoHuYun(string sohuyun)
        {
            if (!string.IsNullOrWhiteSpace(sohuyun))
            {
                CacheService service = new CacheService();
                service.ClearSoHuYun(sohuyun);

            }
        }

        [HttpPost]
        public ActionResult DoQuerySoHuYun(FormCollection from)
        {
            string val = string.Empty;
            string keyName = from["keyName"].ToString();
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return Json(new { result = "1", message = "请填写正确的键值" });
            }
            if (!string.IsNullOrWhiteSpace(keyName))
            {
                CacheService service = new CacheService();
                val = service.QuerySoHuYunInfo(keyName);
            }
            return Json(new { result = "0", message = val, });
        }
    }
}

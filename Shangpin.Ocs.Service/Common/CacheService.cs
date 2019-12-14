using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Cache;

using Sohu;
using Sohu.CS;
using Sohu.CS.Model;
using Sohu.Runtime;

namespace Shangpin.Ocs.Service.Common
{
    public class CacheService
    {
        //public bool ClearCdnCache(string url)
        //{
        //    return false;
        //}

        public bool ClarNginxCache(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // Clear nginx cache 
            string[] servers = AppSettingManager.AppSettings["NginxServers"].Split(',');

            string host1 = Regex.Replace(url, "http://", string.Empty, RegexOptions.IgnoreCase);
            string host = host1.Substring(0, host1.IndexOf("/", StringComparison.Ordinal));
            string relativeUrl = host1.Replace(host, string.Empty);
            foreach (var server in servers)
            {
                RequestUrlWithGet(string.Format("http://{0}/purge/{1}", server, relativeUrl), host);
            }

            ClearChinanetCenterCache(url);

            return true;
        }

        private string ClearChinanetCenterCache(string url)
        {
            string target = "http://wscp.lxdns.com:8080/wsCP/servlet/contReceiver?username={0}&passwd={1}&url={2}";

            string userName = System.Configuration.ConfigurationManager.AppSettings["CdnUser"];

            string password = System.Configuration.ConfigurationManager.AppSettings["CdnPasswd"];

            string md5 = Utils.ToHashString(userName + password + url).ToLower();

            string result = string.Format(target, userName, md5, url);

            return RequestUrlWithGet(result);
        }


        private static string RequestUrlWithGet(String url, string host = "")
        {
            try
            {
                StreamReader sr = null;
                HttpWebResponse response = null;
                HttpWebRequest req = null;
                string temp = string.Empty;

                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                if (!string.IsNullOrEmpty(host))
                {
                    req.Host = host;
                }
                response = (HttpWebResponse)req.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    temp += line;
                }
                sr.Close();
                response.Close();
                req.Abort();
                return temp;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        #region 清理MemberCache RedisCache操作
        public void MemberCacheDelete(string key)
        {
            MemcachedProvider.Instance.Delete(key);
            EnyimMemcachedClient.Instance.Remove(key);
        }

        public void MemberCacheDeleteAll(string[] key)
        {
            foreach (var item in key)
            {
                MemcachedProvider.Instance.Delete(item);
                EnyimMemcachedClient.Instance.Remove(item);
            }
        }

        public void RedisCacheDelete(string key)
        {
            RedisCacheProvider.Instance.Remove(key);
        }

        public void RedisCacheDeleteAll(string[] key)
        {
            foreach (var item in key)
            {
                RedisCacheProvider.Instance.Remove(item);
            }
        }


        #endregion

        /// <summary>
        /// 删除搜狐云CDN文件
        /// </summary>
        /// <param name="key">缓存文件Key</param>
        public void ClearSoHuYun(string key)
        {

            #region 验证密钥
            string accessKey = System.Configuration.ConfigurationManager.AppSettings["accessKey"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["secretKey"];
            SHSCSCredentials myCredentials = new BasicSHSCSCredentials(accessKey, secretKey);
            #endregion

            SohuCSClient client = new SohuCSClient(myCredentials, RegionEndpoint.BJCNC);
            try
            {
                string[] keyArray = key.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyArray != null && keyArray.Length > 0)
                {
                    ListBucketsResponse bucketsResponse = client.ListBuckets();
                    foreach (SCSBucket bucket in bucketsResponse.Buckets)
                    {
                        List<KeyVersion> keyList = new List<KeyVersion>();
                        foreach (var item in keyArray)
                        {
                            KeyVersion kv = new KeyVersion();
                            kv.Key = item;
                            //kv.VersionId = "";
                            keyList.Add(kv);
                        }
                        DeleteObjectsRequest request = new DeleteObjectsRequest
                        {
                            BucketName = bucket.BucketName, //空间
                            Objects = keyList
                        };
                        DeleteObjectsResponse response = client.DeleteObjects(request);
                    }
                }
            }
            catch (DeleteObjectsException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查看搜狐云文件
        /// </summary>
        /// <param name="key">查询文件名称</param>
        /// <returns></returns>
        public string QuerySoHuYunInfo(string key)
        {
            StringBuilder sb = new StringBuilder();

            #region 验证密钥
            string accessKey = System.Configuration.ConfigurationManager.AppSettings["accessKey"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["secretKey"];
            SHSCSCredentials myCredentials = new BasicSHSCSCredentials(accessKey, secretKey);
            #endregion

            SohuCSClient client = new SohuCSClient(myCredentials, RegionEndpoint.BJCNC);
            GetObjectMetadataRequest request1 = new GetObjectMetadataRequest
            {
                BucketName = "shangpin",
                Key = key,
                VersionId = ""
            };
            try
            {
                GetObjectMetadataResponse response1 = client.GetObjectMetadata(request1);
                Console.WriteLine("{0}", response1.LastModified);
                foreach (string sk in response1.Metadata.Keys)
                {
                    sb.Append(string.Format("{0}-{1}", sk, response1.Metadata[sk]));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

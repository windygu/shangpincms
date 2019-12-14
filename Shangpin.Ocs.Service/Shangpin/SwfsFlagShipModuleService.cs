using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using System.IO;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Text.RegularExpressions;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Xml;
using Shangpin.Ocs.Service.Common;
using Shangpin.Framework.Common.Dapper;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsFlagShipModuleService
    {
        /// <summary>
        /// 返回空楼层内容对象
        /// </summary>
        /// <returns></returns>
        public SwfsFlagShipModule CreateEmptySwfsFlagShipModule()
        {
            return new SwfsFlagShipModule
            {
                DataCreate = DateTime.Now,
                State = 1,
                OperatorUserId = PresentationHelper.GetPassport().UserName,
                DataState = 1,
                ModuleName = "",
                BrandNo = "",
            };
        }
        /// <summary>
        /// 保存或更新楼层
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SwfsFlagShipModule InsertOrUpdateSwfsFlagShipModule(SwfsFlagShipModule model)
        {
            if (model.ModuleId == 0)
            {
                model.ModuleId = DapperUtil.Insert<SwfsFlagShipModule>(model, true);
            }
            else
            {
                DapperUtil.Update<SwfsFlagShipModule>(model);
            }
            return model;
        }
        /// <summary>
        /// 根据品牌名获取楼层集合
        /// </summary>
        /// <param name="BrandNo"></param>
        /// <returns></returns>
        public List<SwfsFlagShipModule> GetAllModulesByBrandNo(string BrandNo)
        {
            List<SwfsFlagShipModule> modules = null;
            modules = DapperUtil.Query<SwfsFlagShipModule>("ComBeziWfs_SwfsFlagShipModule_GetAllModulesByBrandNo", new { BrandNo = BrandNo }).FilterList();

            return modules ?? new List<SwfsFlagShipModule>();
        }

        /// <summary>
        /// 根据ID获取SwfsFlagShipModuleLink
        /// </summary>
        /// <param name="LinkId"></param>
        /// <returns></returns>
        public SwfsFlagShipModule GetSwfsFlagShipModuleById(int ModuleId)
        {

            return DapperUtil.QueryByIdentityWithNoLock<SwfsFlagShipModule>(ModuleId);
        }
        /// <summary>
        /// 获取 或 初始化楼层  根据品牌
        /// </summary>
        /// <param name="BrandNo"></param>
        /// <returns></returns>
        public List<SwfsFlagShipModule> GetOrInitialBrandModules(string BrandNo)
        {
            List<SwfsFlagShipModule> modules = GetAllModulesByBrandNo(BrandNo);
            for (int i = 1; i <= 3; i++)
            {
                SwfsFlagShipModule temp = modules.FirstOrDefault(a => a.ModuleType == (i - 1)) ?? CreateEmptySwfsFlagShipModule();
                if (temp.ModuleId == 0)
                {
                    temp.SortId = i;
                    temp.ModuleType = (short)(i - 1);
                    temp.BrandNo = BrandNo;
                    temp.ModuleName = i + "F";
                    InsertOrUpdateSwfsFlagShipModule(temp);
                    modules.Add(temp);
                }
            }
            return modules;
        }

        #region 修改配置
        /// <summary>
        /// 读取配置值
        /// </summary>
        /// <param name="functionNo">功能编号，固定</param>
        /// <param name="configValue">配置值</param>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig InsertOrUpdateSwfsFlagShipGloalConfig(SwfsFlagShipGloalConfig config)
        {
            if (config.ConfigId > 0)
            {
                DapperUtil.Update(config);
            }
            else
            {
                config.ConfigId = DapperUtil.Insert(config, true);
            }
            return config;
        }
        /// <summary>
        /// 获取或初始化配置 根据品牌名
        /// </summary>
        /// <param name="BrandNo"></param>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig GetOrInitSwfsFlagShipGloalConfigbyBrandNo(string BrandNo)
        {
            SwfsFlagShipGloalConfig config = GetGlobalConfigByBrandNo(BrandNo) ?? GetEmptySwfsFlagShipGloalConfig();
            config.BrandNo = BrandNo;
            Brand brand = null;
            if (config.ConfigId == 0)
            {
                if (BrandNo != null && BrandNo.StartsWith("Flagship_"))
                {
                    brand = GetWfsBrandByNo(BrandNo.Split('_')[1]);
                }
                if (brand != null)
                { 
                    config.ConfigName = brand.BrandEnName;
                    config.DataCreate = DateTime.Now;
                    config.ConfigTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
                    config.ConfigId = DapperUtil.Insert<SwfsFlagShipGloalConfig>(config, true);
                }
            }
            return config;
        }
        public Brand GetWfsBrandByNo(string BrandNo)
        {
            return DapperUtil.Query<Brand>("ComBeziWfs_WfsBrand_GetWfsBrandByBrandNo", new { BrandNo = BrandNo }).FirstOrDefault();
        }
        /// <summary>
        /// 根据主键查找品牌名
        /// </summary>
        /// <param name="ConfigId"></param>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig GetGlobalConfigByConfigId(int ConfigId)
        {
            return DapperUtil.QueryByIdentity<SwfsFlagShipGloalConfig>(ConfigId);
        }
        /// <summary>
        /// 根据品牌名查找配置对象
        /// </summary>
        /// <param name="BrandNo"></param>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig GetGlobalConfigByBrandNo(string BrandNo)
        {
            return DapperUtil.Query<SwfsFlagShipGloalConfig>("ComBeziWfs_SwfsFlagShipGloalConfig_GetGlobalConfigByBrandNo", new { BrandNo = BrandNo }).FirstOrDefault();
        }
        /// <summary>
        /// 初始化配置对象
        /// </summary>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig GetEmptySwfsFlagShipGloalConfig()
        {
            return new SwfsFlagShipGloalConfig { BrandNo = "", ConfigName = "品牌状态", ConfigValue = "1", ConfigTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), OperateUserId = PresentationHelper.GetPassport().UserName };
        }
        #endregion
    }
}

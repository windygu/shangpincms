using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using System.Xml.Serialization;
using System.IO;

namespace Shangpin.Ocs.Service.Outlet
{
    /// <summary>
    /// 奥莱统计数据临时服务
    /// </summary>
    public class TempStatisticsService
    {
        public List<SubjectSaleVisitStatisticsDataM> GetStatisticsListBySubjectNoes(List<string> subjectNoes)
        {
            List<SubjectSaleVisitStatisticsDataM> list = new List<SubjectSaleVisitStatisticsDataM>();
            List<SWfsSubjectStatisticsDataTemp> dataList = DapperUtil.Query<SWfsSubjectStatisticsDataTemp>("ComBeziWfs_SWfsSubjectStatisticsDataTemp_GetList", new { SubjectNo = subjectNoes.ToArray() }).ToList();
            SubjectSaleVisitStatisticsDataModel tmpModel;
            SerDeserHelper sdhelper = new SerDeserHelper();
            string xml = string.Empty;
            foreach (var item in dataList)
            {
                xml = item.StatisticsDataXML;
                tmpModel = sdhelper.SerializeTo<SubjectSaleVisitStatisticsDataModel>(xml);
                if(tmpModel!=null)
                {
                    SubjectSaleVisitStatisticsDataM model = new SubjectSaleVisitStatisticsDataM
                    {
                        SubjectNo = tmpModel.SubjectNo,
                        SaleStatistic = tmpModel.SaleStatistic,
                        VisitStatistic = tmpModel.VisitStatistic
                    };
                    list.Add(model);
                }
            }

            return list;
        }
    }

    public class SerDeserHelper
    {

        public T SerializeTo<T>(string xml)
        {
            try
            {
                using (StringReader rdr = new StringReader(xml))
                {
                    //声明序列化对象实例serializer 
                    XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                    //反序列化，并将反序列化结果值赋给变量i
                    T o = (T)xmlserializer.Deserialize(rdr);
                    return o;
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectBrand
    {
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public string SubjectNo { get; set; }
    }

    public class SpikeProductManage
    {
        public string ID { get; set; }
        public DateTime ShowTime { get; set; }
        public string ShowProductPicFileNo { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public int Status { get; set; }
        public string SubjectNo { get; set; }
        public string SubjectCategoryNo { get; set; }
        public DateTime DateDay { get; set; }
        public short Type { get; set; }
        //public DateTime DateCreate { get; set; }
        //public string CreateUserId { get; set; }
    }

    //public class SWfsSubjectSpikeList
    //{
    //    public DateTime Day { get; set; }
    //    public List<SWfsSubjectSpikeProductManage> List { get; set; }
    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;


namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    /// <summary>
    /// 轮播图实体类
    /// </summary>

    [Table(Name = "dbo.SWfsAlterPic")]
    [Database(Name = "ComBeziWfs")]
    public class SWfsAlterPica
    {
        [Column(Storage = "AlterPicId", DbType = "int(4)", IsPrimaryKey = true, IsDbGenerated = true)]
        public int AlterPicId { get; set; }
        public string AlterPicName { get; set; }
        public string AlterPicNo { get; set; }
        public string AlterPicAddr { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int AlterPicView { get; set; }
        public int AlterPicPosition { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SWfsListAlterGroupList
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Status { get; set; }
        public int Gender { get; set; }
        public string Category { get; set; }
        public DateTime CreateDate { get; set; }
        //SWfsListAlterPic
        public int AlterPicId { get; set; }
        public string LargePicture { get; set; }
        public string LinkName { get; set; }
        public string AlterAddress { get; set; }
    }
}

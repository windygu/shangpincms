using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class SWfsProductCommentExtends : SWfsProductComment
    {
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public string ProductPicFile { get; set; }
        public List<SWfsProductCommentPicRef> CommentPictures { get; set; }
        public string MailAddress { get; set; }
        public string LevelName { get; set; }
        public string UserName { get; set; }
        public Int16 GenderStyle { get; set; }
    }
    public class SWfsProductLevelCommentExtends : SWfsProductComment
    {
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public string ProductPicFile { get; set; }
    }
}

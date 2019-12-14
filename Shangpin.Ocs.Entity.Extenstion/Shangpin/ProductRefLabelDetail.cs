using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ProductRefLabelDetail
    { 
        public string ProductNo { get; set; }
        public string RefLabelNo { get; set; }
        public int Weight { get; set; }
        public string RefLabelParentNo { get; set; }
        public string LabelName { get; set; }
        public string ParentLabelName { get; set; }
        public int LabelType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class Category
    {
        public string CategoryName { get; set; }
        public string CategoryNo { get; set; }
        public string ParentNo { get; set; }
        public int Sort { get; set; }
        public short VisibleState { get; set; }
    }
}

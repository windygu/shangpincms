using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class ProductLabel
    {
        public int ElementsId
        {
            get;
            set;
        }
        public string CategoryNo
        {
            set;
            get;
        }
        public string LabelName
        {
            set;
            get;
        }

        public string LabelNo
        {
            set;
            get;
        }

        public int ProductCount
        {
            set;
            get;
        }

        public int SortId
        {
            set;
            get;
        }

        public int Status
        {
            set;
            get;
        }

        public string ParentNo
        {
            get;
            set;
        }

        public string ParentName
        {
            get;
            set;
        }
    }

    public class LableRefProInfo
    {
        public string ProductNo
        {
            get;
            set;
        }
        public string LabelNo
        {
            get;
            set;
        }
        public string ParentNo
        {
            get;
            set;
        }
        public string LabelName
        {
            get;
            set;
        }
        public string LabelNickName
        {
            get;
            set;
        }
    }
}

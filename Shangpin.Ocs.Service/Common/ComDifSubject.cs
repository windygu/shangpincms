using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;

namespace Shangpin.Ocs.Service.Common
{
    public class ComDifSubject : IEqualityComparer<SubjectInfo>
    {
        public bool Equals(SubjectInfo t1, SubjectInfo t2)
        {
            return (t1.SubjectNo == t2.SubjectNo);
        }
        public int GetHashCode(SubjectInfo t)
        {
            return t.ToString().GetHashCode();
        }
    }
}

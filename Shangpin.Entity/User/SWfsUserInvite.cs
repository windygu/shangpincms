using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.User
{
    public class SWfsUserInviteX : PagingEntityBase
    {
        public String InviteId { get; set; }

        public String UserId { get; set; }

        public String SerialNo { get; set; }

        public Int16 InviteType { get; set; }

        public String InviteAccount { get; set; }

        public DateTime DateCreate { get; set; }
    }
}

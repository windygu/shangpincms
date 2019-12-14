using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Login
{
    public class Passport
    {


        public string IdentityId { get; set; } 

        public string SessionId { get; set; }

        public string UserName { get; set; }

        public bool IsAuthenticate()
        {
           return this.IdentityId.Equals(this.SessionId);
        }

        public string GetUserName()
        {
            return this.UserName;
        }

        public string GetIdentityId()
        {
            return this.IdentityId;
        }
    }
}

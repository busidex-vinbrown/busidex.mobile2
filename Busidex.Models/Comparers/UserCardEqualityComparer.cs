﻿using System.Collections.Generic;

namespace Busidex.Models.Domain
{
    public class UserCardEqualityComparer : IEqualityComparer<UserCard>
    {
        public bool Equals(UserCard uc1, UserCard uc2)
        {
            if (uc2 == null && uc1 == null)
                return true;
            else if (uc1 == null | uc2 == null)
                return false;
            else if (uc1.CardId == uc2.CardId)
                return true;
            else
                return false;
        }

        public int GetHashCode(UserCard uc)
        {
            long hCode = uc.CardId;
            return hCode.GetHashCode();
        }
    }
}


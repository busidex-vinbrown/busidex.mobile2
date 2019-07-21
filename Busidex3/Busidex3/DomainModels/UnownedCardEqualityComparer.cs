using System.Collections.Generic;

namespace Busidex3.DomainModels
{
	public class UnownedCardEqualityComparer : IEqualityComparer<UnownedCard>
	{
		public bool Equals (UnownedCard uc1, UnownedCard uc2)
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

		public int GetHashCode (UnownedCard uc)
		{
			long hCode = uc.CardId;
			return hCode.GetHashCode ();
		}
	}
}


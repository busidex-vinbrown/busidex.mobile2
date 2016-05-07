using System;

namespace Busidex.Mobile
{
	public class BranchApiController : BaseController
	{
		public static string GetBranchUrl (QuickShareLink link)
		{
			const string URL = "https://api.branch.io/v1/url";

			var model = new BranchApiLinkParameters () {
				branch_key = Resources.BRANCH_KEY,
				sdk = "api",
				campaign = "",
				feature = "share",
				channel = "sms",
				tags = null,
				data = Newtonsoft.Json.JsonConvert.SerializeObject (new { cardId = link.CardId, _f = link.From, _d = link.DisplayName, _m = link.PersonalMessage})
			};

			var data = Newtonsoft.Json.JsonConvert.SerializeObject (model);
			return MakeRequest (URL, "POST", UISubscriptionService.AuthToken, data);
		}
	}
}


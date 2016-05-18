using System;

using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class BaseCardEditController : BaseController
	{
		protected Card SelectedCard { get; set; }

		public BaseCardEditController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SelectedCard = UISubscriptionService.OwnedCard;
		}

		public virtual void SaveCard ()
		{
			
		}
	}
}



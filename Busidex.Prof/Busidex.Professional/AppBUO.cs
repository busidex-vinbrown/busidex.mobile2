using BranchXamarinSDK;
using Xamarin.Forms;

namespace Busidex.Professional
{
    public class AppBUO: Application, IBranchBUOSessionInterface
    {
        public AppBUO()
        {

        }

        #region IBranchBUOSessionInterface implementation

        public void InitSessionComplete(BranchUniversalObject buo, BranchLinkProperties blp)
        {
        }

        public void SessionRequestError(BranchError error)
        {
        }

        #endregion
    }
}

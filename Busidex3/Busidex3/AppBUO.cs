using BranchXamarinSDK;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Busidex3
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

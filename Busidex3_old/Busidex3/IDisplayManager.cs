using Busidex3.ViewModels;

namespace Busidex3
{
    public interface IDisplayManager
    {
        IDisplayManager GetInstance();
        void SetOrientation(UserCardDisplay.CardOrientation orientation);
    }
}

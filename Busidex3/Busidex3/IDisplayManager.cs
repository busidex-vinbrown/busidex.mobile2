using Busidex.Models.Constants;

namespace Busidex3
{
    public interface IDisplayManager
    {
        IDisplayManager GetInstance();
        void SetOrientation(CardOrientation orientation);
    }
}

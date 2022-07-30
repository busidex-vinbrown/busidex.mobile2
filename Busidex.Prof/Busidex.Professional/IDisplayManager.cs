using Busidex.Models.Constants;

namespace Busidex.Professional
{
    public interface IDisplayManager
    {
        IDisplayManager GetInstance();
        void SetOrientation(CardOrientation orientation);
    }
}

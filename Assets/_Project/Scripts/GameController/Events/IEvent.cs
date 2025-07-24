using System.Collections;

namespace TwelveG.GameController
{
    public interface IEvent
    {
        IEnumerator Execute();
    }
}
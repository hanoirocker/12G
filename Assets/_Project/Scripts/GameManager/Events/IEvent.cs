using System.Collections;

namespace TwelveG.GameManager
{
    public interface IEvent
    {
        IEnumerator Execute();
    }
}
namespace TwelveG.GameController
{
    public class EventContextData
    {
        public SceneEnum sceneEnum;
        public EventsEnum eventEnum;

        public EventContextData(SceneEnum sceneE, EventsEnum eventE)
        {
            sceneEnum = sceneE;
            eventEnum = eventE;
        }
    }
}
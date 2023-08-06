using Events;
using Events.Implementations;

namespace UI
{
    public class PlayButton : UIButton
    {
        protected override void OnPressed()
        {
            GameEventSystem.Invoke<PlayButtonPressedEvent>();
        }
    }
}
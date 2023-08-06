using Events;
using Events.Implementations;

namespace UI
{
    public class NextButton : UIButton
    {
        protected override void OnPressed()
        {
            GameEventSystem.Invoke<NextButtonPressedEvent>();
        }
    }
}
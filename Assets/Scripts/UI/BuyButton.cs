﻿using Events;
using Events.Implementations;

namespace UI
{
    public class BuyButton : UIButton
    {
        protected override void OnPressed()
        {
            GameEventSystem.Invoke<BuyButtonPressedEvent>(1);
        }
    }
}
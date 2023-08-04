﻿using PlayerSpace;

namespace Gates
{
    public class TripleShotGate : Gate
    {
        public override void OnPlayerEnter(Player player)
        {
            player.SetFireBurst(3);
        }
    }
}
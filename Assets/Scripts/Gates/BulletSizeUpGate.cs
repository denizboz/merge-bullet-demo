using PlayerSpace;

namespace Gates
{
    public class BulletSizeUpGate : Gate
    {
        public override void OnPlayerEnter(Player player)
        {
            player.SetBulletSize(isLarge: true);
        }
    }
}
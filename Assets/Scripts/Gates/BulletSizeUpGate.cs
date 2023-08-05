using PlayerSpace;

namespace Gates
{
    public class BulletSizeUpGate : Gate
    {
        public override void OnPlayerEnter(Player player)
        {
            player.SetBulletSize(effectPower);
        }

        protected override void UpdateUI()
        {
            effectUI.text = $"X{effectPower.ToString()}";
        }
    }
}
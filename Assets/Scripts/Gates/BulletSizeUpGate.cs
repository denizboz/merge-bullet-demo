using PlayerSpace;

namespace Gates
{
    public class BulletSizeUpGate : Gate
    {
        public override void OnPlayerEnter(Player player)
        {
            player.SetBulletSize(effectPower);
            gameObject.SetActive(false);
        }

        protected override void UpdateUI()
        {
            effectUI.text = $"x{effectPower.ToString()}";
        }
    }
}
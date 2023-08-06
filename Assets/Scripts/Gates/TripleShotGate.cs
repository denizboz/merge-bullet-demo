using PlayerSpace;

namespace Gates
{
    public class TripleShotGate : Gate
    {
        public override void OnPlayerEnter(Player player)
        {
            player.SetFireBurst(effectPower);
            gameObject.SetActive(false);
        }

        protected override void UpdateUI()
        {
            effectUI.text = $"x{effectPower.ToString()}";
        }
    }
}
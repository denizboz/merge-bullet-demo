using PlayerSpace;

namespace Interfaces
{
    public interface IInteractable
    {
        public void OnBulletEnter(Bullet bullet);
        
        public void OnPlayerEnter(Player player);
    }
}
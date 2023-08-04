using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Managers
{
    public class CurrencyManager : MonoBehaviour, IDependency
    {
        public void Bind()
        {
            DI.Bind(this);
        }

        public void UpdateAmount(int amount)
        {
        }
    }
}
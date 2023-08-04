using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Managers
{
    public class CurrencyPooler : MonoBehaviour, IDependency
    {
        public void Bind()
        {
            DI.Bind(this);
        }
        
        public void SendDollars(Vector3 originPos)
        {
        }
    }
}
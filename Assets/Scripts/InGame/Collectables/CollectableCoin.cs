using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Collectables
{
    public class CollectableCoin : Collectable
    {
        [Tooltip("Amount of coin to gain")]
        [SerializeField] 
        private int amount;

        public int CoinAmount { get => amount; }

        protected override void OnCollected(Character collector)
        {
            EntityManager.Instance.DespawnCoin(this);
        }
    }
}

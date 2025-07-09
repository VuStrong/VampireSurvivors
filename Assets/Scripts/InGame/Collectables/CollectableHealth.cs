using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Collectables
{
    public class CollectableHealth : Collectable
    {
        [Tooltip("Amount of health to gain")]
        [SerializeField] 
        protected float hp;

        protected override void OnCollected(Character collector)
        {
            collector.GainHealth(hp);
            Destroy(gameObject);
        }
    }
}

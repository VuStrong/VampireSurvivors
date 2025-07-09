using ProjectCode1.InGame.Characters;
using ProjectCode1.Blueprints;
using UnityEngine;

namespace ProjectCode1.InGame.Collectables
{
    public class CollectableExperience : Collectable
    {
        [SerializeField] protected CollectableExperienceBlueprint experienceBlueprint;

        protected override void OnCollected(Character collector)
        {
            collector.GainExperience(experienceBlueprint.exp);
            EntityManager.Instance.DespawnExp(this);
        }
    }
}

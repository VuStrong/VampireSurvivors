using System.Collections;
using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Augments
{
    public class RaiseSpeedAugment : Augment
    {
        [SerializeField] private float duration;
        [SerializeField] private float interval;
        private float counter = 0;
        private bool activated = false;

        public override bool RequirementMet(Character character = null) => true;

        public override void Init(Character character)
        {
            base.Init(character);

            character.stats.speed += 20;
        }

        void Update()
        {
            if (character == null) return;

            counter += Time.deltaTime;
            if (counter > interval && !activated)
            {
                activated = true;
                StartCoroutine(Boost());
            }
        }

        IEnumerator Boost()
        {
            character.SetGhostMode(true);
            yield return new WaitForSeconds(duration);
            character.SetGhostMode(false);
            activated = false;
            counter = 0;
        }
    }
}

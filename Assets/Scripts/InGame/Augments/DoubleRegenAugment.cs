using ProjectCode1.InGame.Characters;

namespace ProjectCode1.InGame.Augments
{
    public class DoubleRegenAugment : Augment
    {
        public override bool RequirementMet(Character character = null)
        {
            return character != null && character.stats.recoveryPerSecond > 0;
        }

        public override void Init(Character character)
        {
            base.Init(character);

            character.stats.recoveryPerSecond = 5;
        }

        void Update()
        {
            if (character == null) return;

            if (character.IsMoving)
            {
                character.stats.recoveryPerSecond = 5;
            }
            else
            {
                character.stats.recoveryPerSecond = 10;
            }
        }
    }
}

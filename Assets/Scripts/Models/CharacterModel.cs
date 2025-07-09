using ProjectCode1.Blueprints;

namespace ProjectCode1.Models
{
    public class CharacterModel
    {
        public CharacterBlueprint Blueprint { get; private set; }
        public bool Owned { get; private set; }
        public int Level { get; private set; }

        public CharacterModel(CharacterBlueprint blueprint, bool owned, int level)
        {
            Blueprint = blueprint;
            Owned = owned;
            Level = level;
        }
    }
}

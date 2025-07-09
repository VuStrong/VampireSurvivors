using ProjectCode1.Blueprints;

namespace ProjectCode1.Models
{
    public class PowerUpModel
    {
        public PowerUpBlueprint PowerUpBlueprint { get; private set; }
        public int Level { get; set; }

        public bool CanBeLevelUp { get => Level < PowerUpBlueprint.TotalLevel; }

        public PowerUpModel(PowerUpBlueprint powerUpBlueprint, int level)
        {
            PowerUpBlueprint = powerUpBlueprint;
            Level = level;
        }

        public float GetValueOfCurrentLevel()
        {
            return PowerUpBlueprint.GetValueAtLevel(Level);
        }

        public string GetDescriptionOfCurrentLevel()
        {
            return PowerUpBlueprint.GetPowerUpDescriptionAtLevel(Level);
        }

        public string GetDescriptionOfNextLevel()
        {
            return PowerUpBlueprint.GetPowerUpDescriptionAtLevel(Level + 1);
        }
    }
}

namespace ProjectCode1.InGame.Characters
{
    /// <summary>
    /// Character Effect: Start with 1 more projectile
    /// </summary>
    public class Character1 : Character
    {
        protected override void Awake()
        {
            base.Awake();

            stats.projectileCountBonus += 1;
        }
    }
}

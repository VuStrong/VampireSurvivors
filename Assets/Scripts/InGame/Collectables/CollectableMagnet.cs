using ProjectCode1.InGame.Characters;

namespace ProjectCode1.InGame.Collectables
{
    public class CollectableMagnet : Collectable
    {
        protected override void OnCollected(Character collector)
        {
            EntityManager.Instance.CollectAllCollectableItems();
            Destroy(gameObject);
        }
    }
}

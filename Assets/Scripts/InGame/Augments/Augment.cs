using ProjectCode1.InGame.Characters;
using UnityEngine;
using UnityEngine.Localization;

namespace ProjectCode1.InGame.Augments
{
    public abstract class Augment : MonoBehaviour
    {
        [Header("Augment Info")]
        [SerializeField] private string id;
        [SerializeField] protected LocalizedString localizedDescription;

        protected Character character;

        public string Id { get => id; }
        public string Description { get => localizedDescription.GetLocalizedString(); }

        public abstract bool RequirementMet(Character character = null);

        public virtual void Init(Character character)
        {
            this.character = character;
            transform.parent = character.transform;
            transform.localPosition = Vector3.zero;
        }
    }
}

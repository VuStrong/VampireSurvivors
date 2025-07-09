using System.Collections.Generic;
using ProjectCode1.InGame.Skills;
using UnityEngine;

namespace ProjectCode1.InGame.UI
{
    public class CombinationSkillList : MonoBehaviour
    {
        [SerializeField] private CombinationSkillListItem skillPrefab;
        [SerializeField] private Transform listHolder;

        public void Populate(List<Skill> skills)
        {
            foreach (Transform child in listHolder)
            {
                Destroy(child.gameObject);
            }

            foreach (Skill skill in skills)
            {
                var prefab = Instantiate(skillPrefab, listHolder);
                prefab.Init(skill);
            }
        }
    }
}

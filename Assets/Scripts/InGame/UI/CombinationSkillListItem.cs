using ProjectCode1.InGame.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.InGame.UI
{
    public class CombinationSkillListItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text skillName;
        [SerializeField] private TMP_Text skillDescription;
        [SerializeField] private Transform requiredSkillsHolder;
        [SerializeField] private SkillMiniIcon requiredSkillPrefab;

        public void Init(Skill skill)
        {
            image.sprite = skill.Image;
            skillName.text = skill.Name;
            skillDescription.text = skill.Description;

            foreach (var requiredSkill in skill.RequiredSkills)
            {
                var prefab = Instantiate(requiredSkillPrefab, requiredSkillsHolder);
                prefab.Setup(requiredSkill.skill.Image, requiredSkill.requiredLevel);
            }
        }
    }
}
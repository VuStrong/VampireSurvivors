using System;
using ProjectCode1.InGame.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectCode1.InGame.UI
{
    public class SkillCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text skillDescriptionText;
        [SerializeField] private GameObject combinationSkillsHolder;
        private Skill skill;
        private Action<Skill> onSelected;

        public void Init(Skill skill, Action<Skill> onSelected)
        {
            this.skill = skill;
            this.onSelected = onSelected;

            image.sprite = skill.Image;
            skillNameText.text = skill.Name;

            if (skill.Owned)
            {
                levelText.text = $"LV.{skill.Level + 1}";
                skillDescriptionText.text = skill.GetUpgradeDescription();
            }
            else
            {
                levelText.text = $"New skill";
                skillDescriptionText.text = skill.Description;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSelected?.Invoke(skill);
        }
    }
}

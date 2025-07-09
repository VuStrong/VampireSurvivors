using System;
using System.Collections.Generic;
using ProjectCode1.InGame.Skills;
using UnityEngine;

namespace ProjectCode1.InGame.UI
{
    public class SkillSelectionMenu : MonoBehaviour
    {
        [SerializeField] private SkillCard skillCardPrefab;
        [SerializeField] private GameObject skillCardsHolder;
        [SerializeField] private SkillMiniIcon skillIconPrefab;
        [SerializeField] private GameObject ownedSkillsHolder;

        private Action<Skill> onSkillSelected;

        public void Setup(Action<Skill> onSkillSelected)
        {
            this.onSkillSelected = onSkillSelected;
        }

        public void Open(IEnumerable<Skill> skillsToDisplay, IEnumerable<Skill> ownedSkills)
        {
            PopulateOwnedSkills(ownedSkills);

            // Destroy all previous cards
            foreach (Transform child in skillCardsHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var skill in skillsToDisplay)
            {
                var skillCard = Instantiate(skillCardPrefab, skillCardsHolder.transform, false);
                skillCard.Init(skill, onSkillSelected);
            }

            gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        void PopulateOwnedSkills(IEnumerable<Skill> ownedSkills)
        {
            foreach (Transform child in ownedSkillsHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var skill in ownedSkills)
            {
                var skillIcon = Instantiate(skillIconPrefab, ownedSkillsHolder.transform, false);
                skillIcon.Setup(skill.Image, skill.Level);
            }
        }
    }
}

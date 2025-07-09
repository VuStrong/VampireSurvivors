using System.Collections.Generic;
using System.Linq;
using ProjectCode1.InGame.Augments;
using ProjectCode1.InGame.Characters;
using ProjectCode1.InGame.Skills;
using ProjectCode1.InGame.UI;
using UnityEngine;

namespace ProjectCode1.InGame
{
    public class SkillManager : MonoBehaviour
    {
        [Header("Skill Section Menu")]
        [SerializeField] private SkillSelectionMenu skillSelectionMenu;
        [SerializeField] private int numberOfSkillsToDisplay;

        [Header("Combination skill list")]
        [SerializeField] private CombinationSkillList combinationSkillList;

        [Header("Augment Section Menu")]
        [SerializeField] private AugmentSelectionMenu augmentSelectionMenu;
        [SerializeField] private int numberOfAugmentsToDisplay;

        private readonly List<Skill> combinableSkills = new();
        private readonly List<Skill> newSkills = new();
        private IReadOnlyList<Skill> ownedSkills;
        private readonly List<Augment> availableAugments = new();
        private Character character;

        public void Setup(Character character)
        {
            this.character = character;

            availableAugments.AddRange(LoadAugments());
            List<Skill> allSkills = LoadSkills();
            ownedSkills = character.Skills;

            foreach (Skill skill in allSkills)
            {
                if (skill.IsCombinedSkill)
                    combinableSkills.Add(skill);
                else if (!ownedSkills.Any(s => s.Id == skill.Id))
                    newSkills.Add(skill);
            }

            if (combinationSkillList != null)
                combinationSkillList.Populate(combinableSkills);

            skillSelectionMenu.Setup(Select);
            augmentSelectionMenu.Setup(Select);
            character.OnLevelUp += OnCharacterLevelUp;

            Resources.UnloadUnusedAssets();
        }

        List<Skill> LoadSkills()
        {
            return Resources.LoadAll<Skill>("Prefabs/Skills").ToList();
        }

        List<Augment> LoadAugments()
        {
            return Resources.LoadAll<Augment>("Prefabs/Augments").ToList();
        }

        void OnCharacterLevelUp(int level)
        {
            if (level % 10 != 0)
                ShowSkillSelectionMenu();
            else
                ShowAugmentSelectionMenu();
        }

        public void ShowSkillSelectionMenu()
        {
            var skillsToDisplay = SelectSkills(numberOfSkillsToDisplay);
            if (skillsToDisplay.Count == 0) return;
            skillSelectionMenu.Open(skillsToDisplay, ownedSkills);
        }

        public void ShowAugmentSelectionMenu()
        {
            var augmentsToDisplay = availableAugments
                .Where(a => a.RequirementMet(character))
                .OrderBy(s => UnityEngine.Random.value)
                .Take(numberOfAugmentsToDisplay)
                .ToList();

            if (augmentsToDisplay.Count == 0)
            {
                ShowSkillSelectionMenu();
                return;
            }

            augmentSelectionMenu.Open(augmentsToDisplay);
        }

        public void Select(Skill skill)
        {
            if (skill == null || character == null) return;

            if (skill.Owned)
            {
                skill.Upgrade();
            }
            else
            {
                var newSkill = Instantiate(skill);
                character.AddSkill(newSkill);

                if (!newSkill.IsCombinedSkill)
                    newSkills.RemoveAll(s => s.Id == newSkill.Id);
            }

            HideSkillSelectionMenu();
        }

        public void Select(Augment augment)
        {
            if (augment == null || character == null) return;

            var newAugment = Instantiate(augment);
            newAugment.Init(character);

            availableAugments.RemoveAll(a => a.Id == newAugment.Id);

            HideAugmentSelectionMenu();
        }

        public void HideSkillSelectionMenu()
        {
            skillSelectionMenu.Close();
        }

        public void HideAugmentSelectionMenu()
        {
            augmentSelectionMenu.Close();
        }

        /// <summary>
        /// Select skills to display
        /// </summary>
        List<Skill> SelectSkills(int count = 3)
        {
            List<Skill> selectedSkills = new();

            // First get skills that can be combined
            var skillsToCombine = GetSkillsToCombine(count);
            selectedSkills.AddRange(skillsToCombine);
            if (selectedSkills.Count >= count) return selectedSkills;

            // Attemp to get max 2 owned skills that can be upgraded for player to upgrade
            int ownedSkillsCount = Mathf.Clamp(ownedSkills.Count, 0, 2);
            var upgradeableSkills = ownedSkills
                .Where(s => s.CanBeUpgrade)
                .OrderByDescending(s => s.Level)
                .Take(ownedSkillsCount);

            selectedSkills.AddRange(upgradeableSkills);
            if (selectedSkills.Count >= count) return selectedSkills;

            // If still have empty slots, select new random skills
            var remainSkills = newSkills.OrderBy(s => UnityEngine.Random.value).Take(count - selectedSkills.Count);
            selectedSkills.AddRange(remainSkills);

            return selectedSkills;
        }

        List<Skill> GetSkillsToCombine(int max)
        {
            List<Skill> skills = new();

            foreach (var skill in combinableSkills)
            {
                if (CheckSkillCanBeCombined(skill))
                    skills.Add(skill);

                if (skills.Count >= max)
                    return skills;
            }

            return skills;
        }

        bool CheckSkillCanBeCombined(Skill skillToCheck)
        {
            if (!skillToCheck.IsCombinedSkill || ownedSkills.Any(s => s.Id == skillToCheck.Id)) return false;

            foreach (var requiredSkill in skillToCheck.RequiredSkills)
            {
                var playerSkill = ownedSkills.FirstOrDefault(s => s.Id == requiredSkill.skill.Id);
                if (playerSkill == null || playerSkill.Level < requiredSkill.requiredLevel)
                    return false;
            }

            return true;
        }
    }
}

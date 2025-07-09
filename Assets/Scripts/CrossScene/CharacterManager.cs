using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCode1.Models;
using ProjectCode1.Blueprints;
using UnityEngine;

namespace ProjectCode1.CrossScene
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [SerializeField] private List<string> defaultCharacterIds;

        private readonly List<CharacterModel> characters = new();

        public IReadOnlyList<CharacterModel> Characters { get => characters.AsReadOnly(); }
        public CharacterModel SelectedCharacter { get; private set; }
        public Action SelectedCharacterChanged;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void InitializeCharacters()
        {
            var characterBlueprints = Resources.LoadAll<CharacterBlueprint>("Blueprints/Characters").ToList();
            if (characterBlueprints == null || characterBlueprints.Count == 0) return;

            string idsString = PlayerPrefs.GetString("OwnedCharacterIds", "");
            List<string> ownedCharacterIds = !string.IsNullOrEmpty(idsString) ? idsString.Split(",").ToList() : new List<string>();
            ownedCharacterIds.AddRange(defaultCharacterIds);

            foreach (var blueprint in characterBlueprints)
            {
                bool owned = ownedCharacterIds.Contains(blueprint.id);
                var model = new CharacterModel(blueprint, owned, 0);
                characters.Add(model);
            }

            GetSelectedCharacter();
        }

        void GetSelectedCharacter()
        {
            string characterId = PlayerPrefs.GetString("SelectedCharacter");
            var characterModel = characters.FirstOrDefault(c => c.Blueprint.id == characterId);

            if (characterModel != null && characterModel.Owned)
            {
                SelectedCharacter = characterModel;
            }
            else
            {
                SelectedCharacter = characters.FirstOrDefault(c => c.Owned);
            }

            SelectedCharacterChanged?.Invoke();
        }

        public void SelectCharacter(string characterId)
        {
            if (characterId == SelectedCharacter?.Blueprint.id) return;

            var characterModel = characters.FirstOrDefault(c => c.Blueprint.id == characterId);
            if (characterModel == null || !characterModel.Owned) return;

            PlayerPrefs.SetString("SelectedCharacter", characterId);
            
            SelectedCharacter = characterModel;
            SelectedCharacterChanged?.Invoke();
        }
    }
}

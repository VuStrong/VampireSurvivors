using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "Map", menuName = "Blueprints/Map")]
    public class MapBlueprint : ScriptableObject
    {
        [Header("Map info")]
        public string mapName;
        public string description;

        [Header("Time")]
        public float totalTime = 720;

        [Header("Wave settings")]
        public float timeBetweenWave;
        public Wave[] waves;

        [Header("Boss")]
        public Monster boss;

        public List<Monster> GetAllNormalMonsters()
        {
            List<Monster> monsters = new();

            foreach (Wave wave in waves)
            {
                foreach (SpawnChance spawnChance in wave.spawnChances)
                {
                    if (!monsters.Any(m => m.Id == spawnChance.monster.Id))
                    {
                        monsters.Add(spawnChance.monster);
                    }
                }
            }

            return monsters;
        }

        [Serializable]
        public class Wave
        {
            public int minimumMonsterAmount;
            public float spawnInterval;
            public SpawnChance[] spawnChances;
        }

        [Serializable]
        public class SpawnChance
        {
            [Range(0f, 100f)]
            public float chance;
            public Monster monster;
        }
    }
}

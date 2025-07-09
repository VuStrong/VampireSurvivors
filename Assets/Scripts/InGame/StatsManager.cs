using TMPro;
using UnityEngine;

namespace ProjectCode1.InGame
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;

        private int coinGained;
        public int CoinGained
        {
            get => coinGained;
            set
            {
                coinGained = value;
                coinText.text = value.ToString();
            }
        }

        public int MonstersKilled { get; set; }
    }
}

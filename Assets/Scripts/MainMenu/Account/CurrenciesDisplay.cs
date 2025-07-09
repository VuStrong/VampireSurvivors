using ProjectCode1.CrossScene;
using TMPro;
using UnityEngine;

namespace ProjectCode1.MainMenu
{
    public class CurrenciesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text gemText;

        void OnEnable()
        {
            coinText.text = AccountManager.Instance.Coin.ToString();
            gemText.text = AccountManager.Instance.Gem.ToString();
            AccountManager.Instance.CoinChanged += OnCoinChanged;
            AccountManager.Instance.GemChanged += OnGemChanged;
        }

        void OnDisable()
        {
            AccountManager.Instance.CoinChanged -= OnCoinChanged;
            AccountManager.Instance.GemChanged -= OnGemChanged;
        }

        void OnCoinChanged(int coin)
        {
            coinText.text = coin.ToString();
        }

        void OnGemChanged(int gem)
        {
            gemText.text = gem.ToString();
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.InGame.UI
{
    public class SkillMiniIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text levelText;

        public void Setup(Sprite image, int level)
        {
            this.image.sprite = image;
            levelText.text = level.ToString();
        }
    }
}

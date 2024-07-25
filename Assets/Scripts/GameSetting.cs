using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Localization.Settings;
using MoreMountains.NiceVibrations;

namespace HungwX
{
    public class GameSetting : MonoBehaviour
    {
        public string spriteKey;
        private Image image;
        private int index;
        [SerializeField] private Sprite[] sprites = default;
        private bool isActive;

        void Start()
        {
            isActive = false;
            image = GetComponent<Image>();
            index = PlayerPrefs.GetInt(spriteKey, 0);
            Setting();
            if (sprites.Length > 0)
            {
                image.sprite = sprites[index];
            }
            else
            {
                image.color = index == 0 ? new Color32(86, 86, 86, 255) : new Color32(186, 186, 186, 255);
            }
        }

        public void SwapSprite()
        {
            index = (index + 1) % 2;
            if (sprites.Length > 0)
            {
                image.sprite = sprites[index];
            }
            else
            {
                image.color = index == 0 ? new Color32(86, 86, 86, 255) : new Color32(186, 186, 186, 255);
            }
            PlayerPrefs.SetInt(spriteKey, index);
            Setting();
        }

        private void Setting()
        {
            switch (spriteKey)
            {
                case "Sound":
                    if (index == 1)
                    {
                        SoundManager.ChangeVolumeFXSound(0);
                    }
                    else
                    {
                        SoundManager.ChangeVolumeFXSound(1);
                    }
                    break;
                case "Haptic":
                    MMVibrationManager.SetHapticsActive(index == 0);
                    break;
                case "Language":
                    if (!isActive)
                    {
                        StartCoroutine(SetLocale(index));
                    }
                    break;
            }
        }
        IEnumerator SetLocale(int id)
        {
            isActive = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[id];
            PlayerPrefs.SetInt("Language", id);
            isActive = false;
        }
    }
}

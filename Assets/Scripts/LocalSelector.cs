using UnityEngine;
using System.Globalization;
using System.Collections;
using UnityEngine.Localization.Settings;

namespace HungwX
{
    public class LocalSelector : MonoBehaviour
    {
        private bool isActive = false;
        private int GetRegion()
        {
            string region = RegionInfo.CurrentRegion.Name;
            print(region);
            if (region.Equals("VN"))
            {
                return 1;
            }
            return 0;
        }

        void Start()
        {
            int id = PlayerPrefs.GetInt("Language", -1);
            if (id == -1)
            {
                id = GetRegion();
                PlayerPrefs.SetInt("Language", id);
            }
            ChangeLocale(id);
        }

        public void ChangeLocale(int id)
        {
            if (isActive)
            {
                return;
            }
            StartCoroutine(SetLocale(id));
        }

        IEnumerator SetLocale(int id)
        {
            isActive = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[id];
            isActive = false;
        }
    }
}

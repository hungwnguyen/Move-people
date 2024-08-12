using UnityEngine;

namespace HungwX
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialClips = default;
        [SerializeField] private string tutorialKey = "Tutorial";
        public static TutorialClip currentClip;
        private static bool isTutorialOn = true;
        private static string tutorialKeyStatic;

        void Awake()
        {
            tutorialKeyStatic = tutorialKey;
            int tutorialStatus = PlayerPrefs.GetInt(tutorialKey, 1);
            if (tutorialStatus == 1)
            {
                isTutorialOn = true;
                currentClip = Instantiate(tutorialClips, this.transform).GetComponent<TutorialClip>();
            }
            else
            {
                isTutorialOn = false;
            }
        }

        public static void TurnOffTutorial()
        {
            PlayerPrefs.SetInt(tutorialKeyStatic, 0);
            if (isTutorialOn)
            {
                DestroyImmediate(currentClip.gameObject);
            }
        }

        public static void TurnOnTutorialClip(int index)
        {
            if (isTutorialOn)
            {
               
            }
        }
    }
}

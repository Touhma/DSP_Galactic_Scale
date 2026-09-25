using UnityEngine;

namespace GalacticScale
{
    public sealed class GSUILanguageWatcher : MonoBehaviour
    {
        private int language = Localization.CurrentLanguageLCID;

        private void Update()
        {
            if (language == Localization.CurrentLanguageLCID) return;
            language = Localization.CurrentLanguageLCID;
            SettingsUI.RefreshLocalization();
        }
    }
}

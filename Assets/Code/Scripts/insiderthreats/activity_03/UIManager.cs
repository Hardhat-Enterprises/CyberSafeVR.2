using UnityEngine;
using TMPro;

namespace InsiderThreat03
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text bodyText;

        public void ShowInfo(string title, string body, string status)
        {
            if (titleText) titleText.text = title;
            if (bodyText)  bodyText.text  = body;
            if (panel)     panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel) panel.SetActive(false);
        }
    }
}

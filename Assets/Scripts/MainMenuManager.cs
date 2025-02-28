using UnityEngine;

// Controls ui options on the main menu based on player progress
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject warningScreen;

    [Header("Boss Buttons")]
    [SerializeField] private GameObject bossTwoButton;
    [SerializeField] private GameObject bossThreeButton;

    [Header("Easter Egg Buttons")]
    [SerializeField] private GameObject bossOneEEButton;
    [SerializeField] private GameObject bossTwoEEButton;
    [SerializeField] private GameObject bossThreeEEButton;

    private void Start()
    {
        warningScreen.SetActive(true);

        if (GameManager.Instance.IsBossDefeated(1))
        {
            bossTwoButton.SetActive(true);
        }

        if (GameManager.Instance.IsBossDefeated(2))
        {
            bossThreeButton.SetActive(true);
        }

        if (GameManager.Instance.IsEasterEggFound(1))
        {
            bossOneEEButton.SetActive(true);
        }

        if (GameManager.Instance.IsEasterEggFound(2))
        {
            bossTwoEEButton.SetActive(true);
        }

        if (GameManager.Instance.IsEasterEggFound(3))
        {
            bossThreeEEButton.SetActive(true);
        }
    }
}

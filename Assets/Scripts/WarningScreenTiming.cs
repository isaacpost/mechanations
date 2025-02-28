using System.Collections;
using UnityEngine;

// THe timing used on the intro warning screen
public class WarningScreenTiming : MonoBehaviour
{
    [SerializeField] private GameObject firstTextBox;
    [SerializeField] private GameObject secondTextBox;
    [SerializeField] private GameObject thirdTextBox;

    void Start()
    {
        SFXManager.Instance.LoadMusic("MainMenuMusic");

        if (GameManager.Instance.GetFirstBoot())
        {
            StartCoroutine(ScreenTiming());
        }
        else
        {
            gameObject.SetActive(false);

            SFXManager.Instance.PlayMusic();
        }
    }

    private IEnumerator ScreenTiming()
    {
        firstTextBox.SetActive(true);
        SFXManager.Instance.PlaySound("PickUpPart");

        yield return new WaitForSeconds(3f);

        secondTextBox.SetActive(true);
        SFXManager.Instance.PlaySound("PickUpPart");

        yield return new WaitForSeconds(3f);

        thirdTextBox.SetActive(true);
        SFXManager.Instance.PlaySound("PickUpPart");

        yield return new WaitForSeconds(3f);

        GameManager.Instance.BootedUp();

        gameObject.SetActive(false);

        SFXManager.Instance.PlayMusic();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Menus")]
    [SerializeField] private Canvas youDiedMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas youWinMenu;
    [SerializeField] private Canvas youWinFinalMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas partsMenu;
    [SerializeField] private Canvas controlsMenu;
    [SerializeField] private Canvas easterEgg1Menu;
    [SerializeField] private Canvas easterEgg2Menu;
    [SerializeField] private Canvas easterEgg3Menu;
    [SerializeField] private Canvas easterEgg4Menu;

    private PlayerController player;
    private Canvas activeCanvas;
    private string previousCanvasName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hide OS cursor — use UICursorFollower for visuals
        Cursor.visible = false;
    }

    // Opens the menu of the given string name
    public void OpenMenu(string menuName)
    {
        if (activeCanvas != null)
        {
            string tempPreviousCanvasName = activeCanvas.name;
            CloseCurrentMenu();
            previousCanvasName = tempPreviousCanvasName;
        }

        SFXManager.Instance.PlaySound("PickUpPart");

        switch (menuName)
        {
            case "YouDiedMenuCanvas": activeCanvas = youDiedMenu; break;
            case "PauseMenuCanvas": activeCanvas = pauseMenu; break;
            case "YouWinMenuCanvas": activeCanvas = youWinMenu; break;
            case "YouWinFinalMenuCanvas": activeCanvas = youWinFinalMenu; break;
            case "SettingsMenuCanvas": activeCanvas = settingsMenu; break;
            case "PartDescriptionMenuCanvas": activeCanvas = partsMenu; break;
            case "ControlsMenuCanvas": activeCanvas = controlsMenu; break;
            case "EasterEggOneMenuCanvas": activeCanvas = easterEgg1Menu; break;
            case "EasterEggTwoMenuCanvas": activeCanvas = easterEgg2Menu; break;
            case "EasterEggThreeMenuCanvas": activeCanvas = easterEgg3Menu; break;
            case "EasterEggFourMenuCanvas": activeCanvas = easterEgg4Menu; break;
        }

        if (activeCanvas != null)
            activeCanvas.gameObject.SetActive(true);
    }

    public void TogglePause()
    {
        player = Resources.FindObjectsOfTypeAll<PlayerController>()[0];

        if (activeCanvas)
        {
            Time.timeScale = 1f;
            CloseCurrentMenu();
            SFXManager.Instance.PlayMusic();
            player.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            OpenMenu("PauseMenuCanvas");
            SFXManager.Instance.PauseMusic();
            player.gameObject.SetActive(false);
        }
    }

    public void CloseCurrentMenu()
    {
        if (activeCanvas != null)
        {
            activeCanvas.gameObject.SetActive(false);
            activeCanvas = null;
        }

        if (previousCanvasName != null)
        {
            OpenMenu(previousCanvasName);
            previousCanvasName = null;
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void SwitchToNextBoss()
    {
        if (SceneManager.GetActiveScene().name == "BossOneScene")
            SwitchToBossTwoScene();
        else
            SwitchToBossThreeScene();
    }

    public void SwitchToBossOneScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossOneScene");
    }

    public void SwitchToBossTwoScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossTwoScene");
    }

    public void SwitchToBossThreeScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossThreeScene");
    }

    public void SwitchToBossFourScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossFourScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

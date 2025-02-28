using UnityEngine;
using UnityEngine.SceneManagement;

// A prefab script that controls opening/closing menus
// and moving between scenes
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    // The menus to toggle
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

    private PlayerController player;
    private Canvas activeCanvas; // Current menu canvas thats open
    private string previousCanvasName; // If there was a previous menu open, has its name

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Opens the menu of the given string name
    public void OpenMenu(string menuName)
    {
        // Closes previous menu if there was one open
        if (activeCanvas != null)
        {
            string tempPreviousCanvasName = activeCanvas.name;

            CloseCurrentMenu();

            previousCanvasName = tempPreviousCanvasName;
        }

        SFXManager.Instance.PlaySound("PickUpPart");

        switch (menuName)
        {
            case "YouDiedMenuCanvas":
                activeCanvas = youDiedMenu;
                break;
            case "PauseMenuCanvas":
                activeCanvas = pauseMenu;
                break;
            case "YouWinMenuCanvas":
                activeCanvas = youWinMenu;
                break;
            case "YouWinFinalMenuCanvas":
                activeCanvas = youWinFinalMenu;
                break;
            case "SettingsMenuCanvas":
                activeCanvas = settingsMenu;
                break;
            case "PartDescriptionMenuCanvas":
                activeCanvas = partsMenu;
                break;
            case "ControlsMenuCanvas":
                activeCanvas = controlsMenu;
                break;
            case "EasterEggOneMenuCanvas":
                activeCanvas = easterEgg1Menu;
                break;
            case "EasterEggTwoMenuCanvas":
                activeCanvas = easterEgg2Menu;
                break;
            case "EasterEggThreeMenuCanvas":
                activeCanvas = easterEgg3Menu;
                break;
        }

        activeCanvas.gameObject.SetActive(true);
    }

    // Pauses/Unpauses scene based on if pause menu is active
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

    // Sets current menu to inactive
    public void CloseCurrentMenu()
    {
        activeCanvas.gameObject.SetActive(false);
        activeCanvas = null;

        if (previousCanvasName != null)
        {
            OpenMenu(previousCanvasName);
            previousCanvasName = null;
        }
    }

    // Called to return to the main menu
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartScene()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    public void SwitchToNextBoss()
    {
        if (SceneManager.GetActiveScene().name == "BossOneScene")
        {
            SwitchToBossTwoScene();
        }
        else
        {
            SwitchToBossThreeScene();
        }
    }

    // Called to go to the first boss
    public void SwitchToBossOneScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossOneScene");
    }

    // Called to go to the second boss
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

    // Ends game in unity editor and closes game in application
    public void QuitGame()
    {
        #if UNITY_EDITOR
                // If running in the Unity Editor
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // If running in a built application
                Application.Quit();
        #endif
    }
}

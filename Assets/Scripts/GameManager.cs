using UnityEngine;
using UnityEngine.SceneManagement;

// Keeps track of current progress of player
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;

    // If when loading the main menu scene it should show the intro sequence
    [SerializeField] private bool firstBoot = true;

    [Header("Boss Progress")]
    [SerializeField] private bool boss1Defeated = false;
    [SerializeField] private bool boss2Defeated = false;

    [Header("Easter Egg Progress")]
    [SerializeField] private bool easterEgg1Found = false;
    [SerializeField] private bool easterEgg2Found = false;
    [SerializeField] private bool easterEgg3Found = false;
    [SerializeField] private bool easterEgg4Found = false;

    // Ensure the GameManager persists between scenes
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DefeatBoss(int bossNumber)
    {
        switch (bossNumber)
        {
            case 1:
                boss1Defeated = true;
                break;
            case 2:
                boss2Defeated = true;
                break;
        }
    }

    public void EasterEggFound(string sceneName)
    {
        switch (sceneName)
        {
            case "BossOneScene":
                easterEgg1Found = true;
                break;
            case "BossTwoScene":
                easterEgg2Found = true;
                break;
            case "BossThreeScene":
                easterEgg3Found = true;
                break;
            case "BossFourScene":
                easterEgg4Found = true;
                break;
        }
    }

    public void BootedUp()
    {
        firstBoot = false;
    }

    public bool GetFirstBoot()
    {
        return firstBoot;
    }

    public bool IsBossDefeated(int bossNumber)
    {
        switch (bossNumber)
        {
            case 1:
                return boss1Defeated;
            case 2:
                return boss2Defeated;
            default:
                return false;
        }
    }

    public bool IsEasterEggFound(int bossNumber)
    {
        switch (bossNumber)
        {
            case 1:
                return easterEgg1Found;
            case 2:
                return easterEgg2Found;
            case 3:
                return easterEgg3Found;
            case 4:
                return easterEgg4Found;
            default:
                return false;
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the lifecycle of the current scene, extended by each scene
public abstract class LifecycleManager : MonoBehaviour
{
    // Makes accessable everywhere in scene
    public static LifecycleManager Instance;

    // Objects in scene involved in the lifecycle
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI bossName;

    private readonly float letterDelay = 0.15f; // Delay between each letter in boss intro

    // Called when the script instance is loaded
    protected virtual void Awake()
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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(IntroSequence());
    }

    // Enables relevant gameObjects/actions in the scene
    public void EnableGameObjects()
    {
        SFXManager.Instance.PlayMusic();
        player.SetActive(true);
    }

    // Initiates actions for when a player dies
    public void PlayerDied()
    {
        player.SetActive(false);
        SFXManager.Instance.PauseMusic();
        Time.timeScale = 0f;
        MenuManager.Instance.OpenMenu("YouDiedMenuCanvas");
    }

    // Initiates actions for when a player dies
    public void BossDied()
    {
        SFXManager.Instance.PauseMusic();

        StartCoroutine(DeathSequence());

    }

    // Types boss name to screen on intro
    protected IEnumerator TypeText(string text)
    {
        foreach (char letter in text)
        {
            if (letter != ' ')
            {
                SFXManager.Instance.PlaySound("BossNameLetter");
            }

            bossName.text += letter; // Add one letter at a time.
            yield return new WaitForSeconds(letterDelay); // Wait for the specified delay.
        }
    }

    // Abstract method for boss intro
    protected abstract IEnumerator IntroSequence();

    // Abstract method for boss death
    protected abstract IEnumerator DeathSequence();
}

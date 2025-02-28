using UnityEngine;

// Main script for any sound playing in scenes. Has audio source
// for SFX and music
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip trashSound;
    [SerializeField] private AudioClip pickUpPartSound;
    [SerializeField] private AudioClip placePartSound;
    [SerializeField] private AudioClip turretShootSound;
    [SerializeField] private AudioClip turretHitSound;
    [SerializeField] private AudioClip emptySound;
    [SerializeField] private AudioClip bossNameLetterSound;
    [SerializeField] private AudioClip pickUpItemSound;
    [SerializeField] private AudioClip bossDefeatSound;
    [SerializeField] private AudioClip wallHitSound;
    [SerializeField] private AudioClip droneBossSignalSound;
    [SerializeField] private AudioClip finalBossLaserSound;
    [SerializeField] private AudioClip finalBossSawSound;
    [SerializeField] private AudioClip finalBossIntroSound;
    [SerializeField] private AudioClip easterEggSound;
    [SerializeField] private AudioClip finalBossChargeSound;
    [SerializeField] private AudioClip smallProjectileSound;
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip goatBossDeadSound;
    [SerializeField] private AudioClip staticSound;

    [Header("Music")]
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip bossTwoMusic;
    [SerializeField] private AudioClip bossThreeMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip bossFourMusic;

    private void Awake()
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

    public void PlaySound(string soundName)
    {
        AudioClip clip = null;

        switch (soundName)
        {
            case "Error":
                clip = errorSound;
                break;
            case "Trash":
                clip = trashSound;
                break;
            case "PickUpPart":
                clip = pickUpPartSound;
                break;
            case "PlacePart":
                clip = placePartSound;
                break;
            case "TurretShoot":
                clip = turretShootSound;
                break;
            case "TurretHit":
                clip = turretHitSound;
                break;
            case "Empty":
                clip = emptySound;
                break;
            case "BossNameLetter":
                clip = bossNameLetterSound;
                break;
            case "PickUpItem":
                clip = pickUpItemSound;
                break;
            case "BossDefeat":
                clip = bossDefeatSound;
                break;
            case "WallHit":
                clip = wallHitSound;
                break;
            case "DroneBossSignal":
                clip = droneBossSignalSound;
                break;
            case "FinalBossLaser":
                clip = finalBossLaserSound;
                break;
            case "FinalBossSaw":
                clip = finalBossSawSound;
                break;
            case "FinalBossIntro":
                clip = finalBossIntroSound;
                break;
            case "EasterEgg":
                clip = easterEggSound;
                break;
            case "FinalBossCharge":
                clip = finalBossChargeSound;
                break;
            case "SmallProjectile":
                clip = smallProjectileSound;
                break;
            case "PlayerHurt":
                clip = playerHurtSound;
                break;
            case "GoatBossDead":
                clip = goatBossDeadSound;
                break;
            case "Static":
                clip = staticSound;
                break;
        }

        if (clip != null)
        {
            SFXAudioSource.PlayOneShot(clip);
        }
    }

    public void LoadMusic(string musicName)
    {
        AudioClip clip = null;

        switch (musicName)
        {
            case "BossOneMusic":
                clip = bossMusic;
                break;
            case "BossTwoMusic":
                clip = bossTwoMusic;
                break;
            case "BossThreeMusic":
                clip = bossThreeMusic;
                break;
            case "MainMenuMusic":
                clip = mainMenuMusic;
                break;
            case "BossFourMusic":
                clip = bossFourMusic;
                break;
        }

        if (clip != null)
        {
            musicAudioSource.clip = clip;
            PlayMusic();
            PauseMusic();
        }
    }

    public void PlayMusic()
    {
        musicAudioSource.Play();
    }

    public void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    public float GetSFXVolume()
    {
        return SFXAudioSource.volume;
    }

    public float GetMusicVolume()
    {
        return musicAudioSource.volume;
    }

    public void SetSFXVolume(float newVolume)
    {
        SFXAudioSource.volume = newVolume;
    }

    public void SetMusicVolume(float newVolume)
    {
        musicAudioSource.volume = newVolume;
    }
}

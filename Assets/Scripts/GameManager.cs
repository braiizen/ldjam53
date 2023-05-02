using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private Object prefabToSpawn;
    [SerializeField] private float slomoTime = 5f;
    [SerializeField] private float timeScale = .1f;
    [SerializeField] private float addedTimeAmount = 20f;
    [SerializeField] private float scoreMultiplier = 1.1f;
    [SerializeField] private GameObject PauseMenuCanvas;
    [SerializeField] private GameObject UpgradeCanvas;
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private UpgradeCardUI option1UI;
    [SerializeField] private UpgradeCardUI option2UI;
    [SerializeField] private UpgradeCardUI option3UI;
    [SerializeField] private CarController car;
    [SerializeField] private float fuelAdded = 50f;
    [SerializeField] private AudioSource sfxAudio;
    [SerializeField] private AudioClip caching;
    [SerializeField] private AudioClip crash;
    [SerializeField] private AudioSource clockSound;

    public Transform currentTarget;
    private bool clockTicking = false;

    public float timer = 20f;
    public int score;
    public float explosiveMultiplier;
    public float fuel;
    private Transform lastTarget;
    private float sloTimePassed;
    private bool isSlowedDown;
    private float timeElapsed;
    private float currentScoreMultiplier = 10f;
    public int packagesDelivered = 0;
    public int UIState = 0;

    private List<UpgradeCard> upgradeCards;
    private List<UpgradeCard> pickedCards;


    public int objectsDestroyed;
    private bool gameOver = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            StartGame();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (UIState == 0)
        {
            timer -= Time.deltaTime;
            timeElapsed += Time.deltaTime;
            if (timer < 0 && !gameOver)
            {
                GameOver();
            }

            if (timer < 10f)
            {
                SetPitch(3);
            }
            else if (timer < 20f)
            {
                SetPitch(2);
            }
            else if (timer < 30f)
            {
                SetPitch(1);
            }
            else
            {
                if (clockTicking)
                {
                    clockSound.Stop();
                    clockTicking = false;
                }
            }

        }
    }


    private void SetPitch(float pitch)
    {
        clockSound.pitch = pitch;
        if (!clockTicking)
        {
            clockSound.Play();
            clockTicking = true;
        }
    }

    private void SetupUpgrades()
    {
        upgradeCards = new List<UpgradeCard>();
        var card = new UpgradeCard("Top Speed", 1.1f, "TopSpeed");
        upgradeCards.Add(card);

        card = new UpgradeCard("Acceleration", 1.1f, "Acceleration");
        upgradeCards.Add(card);

        card = new UpgradeCard("Handling", 1.1f, "Handling");
        upgradeCards.Add(card);

        card = new UpgradeCard("Boost Multiplier", 1.1f, "BoostMultiplier");
        upgradeCards.Add(card);

        card = new UpgradeCard("Time added per Destruction", 1.1f, "TimeAdded");
        upgradeCards.Add(card);

        card = new UpgradeCard("Score Multiplier", 1.1f, "ScoreMultiplier");
        upgradeCards.Add(card);

        card = new UpgradeCard("Boost gained", 1.1f, "BoostGained");
        upgradeCards.Add(card);
    }

    private void GameOver()
    {
        Debug.Log("Gameover");
        gameOver = true;
        GameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        UIState = 3;
    }

    public void StartGame()
    {
        score = 0;
        packagesDelivered = 0;
        objectsDestroyed = 0;
        addedTimeAmount = 15f;
        scoreMultiplier = 10f;
        fuelAdded = 50f;
        timer = 30f;
        gameOver = false;
        car.ResetCar();
        Time.timeScale = 1f;
        // Add any other necessary code to start the game
        SetNextTarget();
        SetupUpgrades();
    }

    public void RestartGame()
    {
        addedTimeAmount = 15f;
        scoreMultiplier = 10f;
        fuelAdded = 50f;
        timer = 30f;
        gameOver = false;
        car.ResetCar();

        GameOverCanvas.SetActive(false);
        Time.timeScale = 1f;
        UIState = 0;
        StartGame();
    }

    private void SetNextTarget()
    {
        timeElapsed = 0f;
        Transform newTarget = null;
        do
        {
            newTarget = spawnTransforms[Random.Range(0, spawnTransforms.Length)];
        } while (newTarget == lastTarget);

        lastTarget = newTarget;
        Debug.Log("New target: " + newTarget.name);

        Instantiate(prefabToSpawn, newTarget.position, newTarget.rotation);
        currentTarget = newTarget;
    }

    public void IncreaseScore(float amount)
    {
        int modifiedAmount = Mathf.FloorToInt(amount * currentScoreMultiplier);
        score += modifiedAmount;
        // Add any other necessary code for score updates
    }

    public void AddTime(float multiplier)
    {
        timer += addedTimeAmount * multiplier;
    }

    public void Delivered()
    {
        Debug.Log("Delivered");
        float scoreAmount = Mathf.Max((addedTimeAmount - timeElapsed), 5);
        currentScoreMultiplier *= scoreMultiplier;
        packagesDelivered += 1;
        PlaySFX(caching);
        AddTime(1);
        IncreaseScore(scoreAmount);
        SetNextTarget();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.clip = clip;
        sfxAudio.Play();
    }

    public void PlayCrash()
    {
        PlaySFX(crash);
    }

    public void GetUpgrade()
    {
        PickCards();
        option1UI.SetUpgradeCard(pickedCards[0]);
        option2UI.SetUpgradeCard(pickedCards[1]);
        option3UI.SetUpgradeCard(pickedCards[2]);

        UpgradeCanvas.SetActive(true);
        Time.timeScale = 0f;
        UIState = 2;
    }

    public void SelectOption1()
    {
        Upgrade(pickedCards[0]);
        ContinueGame();
    }

    public void SelectOption2()
    {
        Upgrade(pickedCards[1]);
        ContinueGame();
    }

    public void SelectOption3()
    {
        Upgrade(pickedCards[2]);
        ContinueGame();
    }

    private void ResetCards()
    {
        if (pickedCards == null) return;
            
        foreach (var card in pickedCards)
        {
            upgradeCards.Add(card);
        }

        pickedCards = new List<UpgradeCard>();
    }

    private void Upgrade(UpgradeCard pickedCard)
    {
        switch (pickedCard.UpgradeType)
        {
            case "TopSpeed":
                car.UpgradeSpeed(pickedCard.Multiplier);
                break;
            case "Acceleration":
                car.UpgradeAcceleration(pickedCard.Multiplier);
                break;
            case "Handling":
                car.UpgradeHandling(pickedCard.Multiplier);
                break;
            case "BoostMultiplier":
                car.UpgradeBoost(pickedCard.Multiplier);
                break;
            case "TimeAdded":
                addedTimeAmount += 5;
                break;
            case "ScoreMultiplier":
                scoreMultiplier *= pickedCard.Multiplier;
                break;
            case "BoostGained":
                fuelAdded *= pickedCard.Multiplier;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void PickCards()
    {
        int count = upgradeCards.Count;
        pickedCards = new List<UpgradeCard>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, count);
            pickedCards.Add(upgradeCards[randomIndex]);
            upgradeCards.RemoveAt(randomIndex);
            count--;
        }
    }

    public void OnPause()
    {
        if (UIState == 1)
        {
            ContinueGame();
        }
        else if (UIState == 2)
        {
            ContinueGame();
        }
        else if (UIState == 3)
        {
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        UIState = 1;
    }

    public void ContinueGame()
    {
        ResetCards();
        PauseMenuCanvas.SetActive(false);
        UpgradeCanvas.SetActive(false);
        Time.timeScale = 1f;
        UIState = 0;
    }

    public void MainMenuButton()
    {
        Debug.Log("Main Menu");
        SceneManager.LoadScene(0);
    }

    private void SlowTime()
    {
        Time.timeScale = timeScale;
        isSlowedDown = true;
        timeScale = 0f;
    }

    public void AddFuel()
    {
        fuel += fuelAdded;
    }
}
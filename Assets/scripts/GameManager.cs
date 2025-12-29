using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text winstreak;
    [Header("Objects")]
    public GameObject ball;
    public GameObject hole;
    public GameObject startButton;
    public GameObject sliderGO;
    public Slider powerSlider;
    public GameObject tapTheBallText;
    public GameObject tryAgainButton;
    public GameObject victoryText;
    public GameObject missedText;

    [Header("Audio")]
    public AudioSource winAudio;
    public AudioClip winClip;
    public AudioSource missAudio;
    public AudioClip missClip;

    public static GameManager Instance { get; private set; }   // ← property с getter/setter

    
    [Header("Hole Positions (adjust to your scene)")]
    public Vector2[] holePositions = new Vector2[4] {
        new Vector2(0f, 6f),
        new Vector2(1.5f, 7.5f),
        new Vector2(-0.5f, 8f),
        new Vector2(0.8f, 6.5f)
    };

    private Ball ballScript;
    public bool isShot = false;
    private Vector3 ballStartPos;
    public PowerMeter powerMeter;
    [Header("Таймер")]
    public float missTimer = 3f;  // 3 секунды на попадание (можно менять в инспекторе)

    private float shotTimer = 0f;  // ← НОВОЕ: таймер удара
    // В OnStartButton()


    // Новый метод Shoot (теперь принимает силу 0..1)
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;                          // ← важно! выходим сразу
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);       // если нужен между сценами
    }
    private void Update()
    {
        if (isShot)
        {
            shotTimer += Time.deltaTime;
            if (shotTimer >= missTimer)
            {
                Miss();  // Проигрыш по таймеру
            }
        }
    }
    void Start()
    {
        ballScript = ball.GetComponent<Ball>();
        ballScript.gameManager = this;

        // Record ball start pos (where start button is)
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        ballStartPos = Camera.main.WorldToScreenPoint(startButton.transform.position); // Align to button pos
        ballStartPos.z = -1;
        ballStartPos = Camera.main.ScreenToWorldPoint(ballStartPos);
        ballStartPos.z = -1;

        ResetGame();

        startButton.GetComponent<Button>().onClick.AddListener(OnStartButton);
        tryAgainButton.GetComponent<Button>().onClick.AddListener(ResetGame);
    }

    void OnStartButton()
    {
        startButton.SetActive(false);
        ball.SetActive(true);
        ball.transform.position = ballStartPos;
        ballScript.readyToShoot = true;

        powerMeter.StartMoving();           // ← запуск движения!
        tapTheBallText.SetActive(true);     // или "TAP TO SHOOT!"
    }

    public void Shoot(float normalizedPower)
    {
        powerMeter.StopAndShoot(); // на всякий случай

        tapTheBallText.SetActive(false);

        // Направление — всегда к лунке
        Vector2 direction = (hole.transform.position - ball.transform.position).normalized;

        float force = normalizedPower * 18f; // максимальная сила — подбери (15–25)
        ball.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        shotTimer = 0f;  // ← Сброс таймера при ударе
        isShot = true;
    }

    public void Win()
    {
        shotTimer = 0f;
        PlayerPrefs.SetInt("win",PlayerPrefs.GetInt("win")+1);
        winstreak.text = "WIN STREAK: " + PlayerPrefs.GetInt("wins");
        isShot = false;
        victoryText.SetActive(true);
        if (winAudio && winClip && PlayerPrefs.GetInt("sound")==1) winAudio.PlayOneShot(winClip);
        tryAgainButton.SetActive(true);

    }

    public void Miss()
    {
        PlayerPrefs.SetInt("win",0);
        winstreak.text = "WIN STREAK: " + PlayerPrefs.GetInt("wins");

        shotTimer = 0f;
        isShot = false;
        missedText.SetActive(true);
        if (missAudio && missClip && PlayerPrefs.GetInt("sound") == 1) missAudio.PlayOneShot(missClip);
        tryAgainButton.SetActive(true);
    }

    void ResetGame()
    {
        shotTimer = 0f;
        ball.SetActive(false);
        sliderGO.SetActive(false);
        tapTheBallText.SetActive(false);
        victoryText.SetActive(false);
        missedText.SetActive(false);
        tryAgainButton.SetActive(false);
        startButton.SetActive(true);

        // Random hole
        int rand = Random.Range(0, holePositions.Length);
        hole.transform.position = holePositions[rand];

        ballScript.readyToShoot = false;
        if (ball.activeInHierarchy) ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        isShot = false;
    }
}
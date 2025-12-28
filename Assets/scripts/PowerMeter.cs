using UnityEngine;
using UnityEngine.UI;

public class PowerMeter : MonoBehaviour
{
    [Header("Основные настройки")]
    public Slider slider;                 // основной Slider (должен быть на этом же объекте или дочернем)
    public float speed = 1.4f;            // скорость движения индикатора (1.2–1.8 — комфортный диапазон)

    [Header("Зоны силы")]
    [Range(0.7f, 0.95f)]
    public float greenZoneStart = 0.80f;  // зелёная зона начинается с 80%
    [Range(0.01f, 0.15f)]
    public float greenZoneWidth = 0.12f;  // ширина зелёной зоны (можно сделать 0.08–0.10 для большей сложности)

    private bool movingRight = true;
    private bool isActive = false;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    void Start()
    {
        gameObject.SetActive(false);
        slider.interactable = false;
    }

    public void StartMoving()
    {
        gameObject.SetActive(true);
        isActive = true;
        slider.value = 0f;
        movingRight = true;
    }

    public void StopAndShoot()
    {
        if (!isActive) return;

        isActive = false;
        float power = CalculatePower();

        // Передаём силу в GameManager
        GameManager.Instance.Shoot(power);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        if (movingRight)
        {
            slider.value += speed * Time.deltaTime;
            if (slider.value >= 1f)
            {
                slider.value = 1f;
                movingRight = false;
            }
        }
        else
        {
            slider.value -= speed * Time.deltaTime;
            if (slider.value <= 0f)
            {
                slider.value = 0f;
                movingRight = true;
            }
        }
    }

    // Возвращает силу от 0.3 до 1.0
    private float CalculatePower()
    {
        float val = slider.value;

        float greenEnd = Mathf.Min(1.0f, greenZoneStart + greenZoneWidth);

        // Зелёная зона — максимальная сила
        if (val >= greenZoneStart && val <= greenEnd)
        {
            return 1.0f;
        }
        // Близко к зелёной (жёлтая зона)
        else if (val >= (greenZoneStart - 0.25f) && val < greenZoneStart ||
                 val > greenEnd && val <= (greenEnd + 0.25f))
        {
            float distance = Mathf.Min(
                Mathf.Abs(val - greenZoneStart),
                Mathf.Abs(val - greenEnd)
            );

            // 0.65–0.95 в жёлтых зонах
            return Mathf.Lerp(0.65f, 0.95f, 1f - (distance / 0.25f));
        }
        // Красные зоны (слабый/слишком сильный удар)
        else
        {
            return Mathf.Lerp(0.3f, 0.65f, val < 0.5f ? val : (1f - val));
        }
    }

    // Для удобства отладки (можно убрать позже)
    // public float CurrentValue => slider.value;
}
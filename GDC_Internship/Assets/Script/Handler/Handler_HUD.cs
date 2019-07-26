using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handler_HUD : MonoBehaviour
{
    [SerializeField] private PlayerController Player;

    private Timer Timer;

    private Image ImageTimer;
    private Image ImageTimerShadow;
    private Image ImageHealth;
    private Image ImageDodgeCooldown;
    private Image ImageHealCooldown;
    private Image ImageChargeCooldown;
    private Transform Mode_Ranged;
    private Transform Mode_Melee;
    private Text TextHerbCount;

    private void Awake()
    {
        ImageTimer = transform.Find("Timebar").Find("Bar").GetComponent<Image>();
        ImageTimerShadow = transform.Find("Timebar").Find("Border").GetComponent<Image>();
        ImageHealth = transform.Find("PlayerPortrait").Find("Health").GetComponent<Image>();
        ImageDodgeCooldown = transform.Find("Skills").Find("Skill_Dodge").Find("Cooldown").GetComponent<Image>();
        ImageHealCooldown = transform.Find("Skills").Find("Skill_Heal").Find("Cooldown").GetComponent<Image>();
        ImageChargeCooldown = transform.Find("Skills").Find("Skill_Charge").Find("Cooldown").GetComponent<Image>();
        TextHerbCount = transform.Find("Herb").Find("HerbCount").GetComponent<Text>();

        Mode_Ranged = transform.Find("Mode").Find("Toggle_Ranged");
        Mode_Melee = transform.Find("Mode").Find("Toggle_Melee");

        Timer = new Timer();
    }
    private void Update()
    {
        ImageTimer.fillAmount = Timer.GetTimerNormalized();
        ImageTimerShadow.fillAmount = Timer.GetTimerNormalized();
        ImageHealth.fillAmount = Player.GetNormalizedHealth();
        ImageDodgeCooldown.fillAmount = Player.GetNormalizedDodgeCooldown();
        ImageHealCooldown.fillAmount = Player.GetNormalizedHealCooldown();
        ImageChargeCooldown.fillAmount = Player.GetNormalizedChargeCooldown();
        TextHerbCount.text = Player.GetHerbAmount().ToString();

        UpdateWeaponMode();
        Timer.DecreaseTime();
    }
    private void UpdateWeaponMode()
    {
        if (Player.GetRangedCheck() == true && Mode_Ranged.GetSiblingIndex() == 0)
        {
            Mode_Ranged.SetAsLastSibling();
        }
        else if (Player.GetRangedCheck() == false && Mode_Ranged.GetSiblingIndex() == 1)
        {
            Mode_Ranged.SetAsFirstSibling();
        }
    }
}

public class Timer
{
    public const float Max_Time_Amount = 15*60;// ~15 Minutes
    private float timeRemaining;
    private const float timeDecrement = 1f;

    public Timer()
    {
        timeRemaining = Max_Time_Amount;
    }

    public void DecreaseTime() //Not Implemented, Do NOT Use yet
    {
        if(timeRemaining > 0)
        {
            timeRemaining -= timeDecrement * Time.deltaTime;
        }
    }

    public void IncreaseTime(float amount)
    {
        float threshold = timeRemaining + amount;
        if (threshold > Max_Time_Amount)
        {
            threshold = Max_Time_Amount;
        }
    }
    public float GetTimerNormalized()
    {
        return timeRemaining / Max_Time_Amount;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    private PlayerController Player;

    private TimerClass Timer;
    private SaveState State;

    private Image ImageTimer;
    private Image ImageTimerShadow;
    private Image ImageHealth;
    private Image ImageDodgeCooldown;
    private Image ImageHealCooldown;
    private Image ImageChargeCooldown;
    private Image FadingLayer;
    private Transform Mode_Ranged;
    private Transform Mode_Melee;
    private Text TextHerbCount;

    private bool DefeatedFlag = false;
    private bool VictoriousFlag = false;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        SaveSystem.init();
        State = SaveSystem.LoadGame();
        if (Player)
        {
            FadingLayer = transform.Find("BlackLayer").GetComponent<Image>();
            ImageTimer = transform.Find("Timebar").Find("Bar").GetComponent<Image>();
            ImageTimerShadow = transform.Find("Timebar").Find("Border").GetComponent<Image>();
            ImageHealth = transform.Find("PlayerPortrait").Find("Health").GetComponent<Image>();
            ImageDodgeCooldown = transform.Find("Skills").Find("Skill_Dodge").Find("Cooldown").GetComponent<Image>();
            ImageHealCooldown = transform.Find("Skills").Find("Skill_Heal").Find("Cooldown").GetComponent<Image>();
            ImageChargeCooldown = transform.Find("Skills").Find("Skill_Charge").Find("Cooldown").GetComponent<Image>();
            TextHerbCount = transform.Find("Herb").Find("HerbCount").GetComponent<Text>();

            Mode_Ranged = transform.Find("Mode").Find("Toggle_Ranged");
            Mode_Melee = transform.Find("Mode").Find("Toggle_Melee");

            if (State.ActiveChapter == 0)
            {
                State.ActiveChapter = 1;
            }

            int HerbFlux = Mathf.Clamp(State.ChapterStates[State.ActiveChapter - 1].Herb, 0, 2);
            float AddedTime = (State.ChapterStates[State.ActiveChapter - 1].Herb - HerbFlux) * 60 * 1.5f;

            Timer = new TimerClass(State.ChapterStates[State.ActiveChapter-1].Time + AddedTime);
            Player.SetHerb(HerbFlux);
        }
    }
    private void Update()
    {
        if (Player)
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
    }
    public void PlayerIsDefeated()
    {
        DefeatedFlag = true;
        StartCoroutine(ActivateBlackLayer(true));
    }
    public void BossIsDefeated()
    {
        VictoriousFlag = true;
        if(State.ActiveChapter < 4)
        {
            State.ChapterStates[State.ActiveChapter + 1].Unlocked = true;
        }
        SaveSystem.SaveGame(State);
        StartCoroutine(ActivateBlackLayer(false));
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

    private class TimerClass
    {
        public const float Max_Time_Amount = Constants.Max_Time;
        private float timeRemaining;
        private const float timeDecrement = 10f;

        public TimerClass(float time)
        {
            Debug.Log("Time for this chapter :"+time);
            timeRemaining = time;
        }

        public void DecreaseTime()
        {
            if (timeRemaining > 0)
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
    IEnumerator ActivateBlackLayer(bool Defeated)
    {
        if (!Defeated)
        {
            yield return new WaitForSeconds(3f);
        }
        Color mycolour = Color.black;
        float colourfade = 0;
        while (colourfade < 1)
        {
            colourfade += 0.03f;
            mycolour.a = colourfade;
            FadingLayer.color = mycolour;
            yield return null;
        }
        if (Defeated)
        {
            transform.Find("BlackLayer").GetComponent<ChapterLoader>().IsDefeated();
        }
        else
        {
            transform.Find("BlackLayer").GetComponent<ChapterLoader>().IsVictorious();
        }
    }
    public bool GetVictoryStatus()
    {
        return VictoriousFlag;
    }
}



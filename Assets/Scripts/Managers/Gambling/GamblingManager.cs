using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;


public class GamblingManager : PersistenSingleton<GamblingManager>
{
    [SerializeField] private RewardStruct[] rewards;

    [SerializeField] private Button RollButton;
    [SerializeField] private Button ReturnButton;

    [SerializeField] private Image jokeIcon;
    [SerializeField] private TextMeshProUGUI joketext;

    public float modifier;
    private float _time = 0.83f;

    [Header("Particles")]
    [SerializeField] private GameObject _positiveParticles;
    [SerializeField] private GameObject _neutralParticles;
    [SerializeField] private GameObject _negativeParticles;

    protected override void OnSingletonAwake() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void Roll()
    {
        float total = rewards.Sum(r => 
            r.type == RewardType.Positive
            ? r.weight * modifier
            : r.weight
         );
        float roll = Random.Range(0, total);
        float acc = 0;
        foreach (var r in rewards)
        {
            if (r.type == RewardType.Positive) acc += r.weight * modifier;
            else acc += r.weight;
            if (roll < acc)
            {
                Debug.Log("Rolled " + r.type);
                ApplyEffect(r.type);
                StartCoroutine(WaitForChest(_time, r.reward));
                break;
            }
        }
        modifier = 1;
    }

    private void ApplyReward(RewardsData reward)
    {
        var ctx = new RewardContext();
        foreach (var effect in reward.rewardEffect)
        {
            effect.AddReward(ctx);
        }
    }

    public void GetHermesJoke(HermesJoke joke)
    {
        if (joke.jokeIcon != null)
        {
            jokeIcon.gameObject.SetActive(true);
            jokeIcon.sprite = joke.jokeIcon;
        }
        else
        {
            jokeIcon.gameObject.SetActive(true);
            joketext.text = joke.joke;
        }
    }
    
    private void ApplyEffect(RewardType type)
    {
        switch(type)
        {
            case RewardType.Positive: _positiveParticles.SetActive(true); break;
            case RewardType.Negative: _negativeParticles.SetActive(true);break;
            case RewardType.Neutral: _neutralParticles.SetActive(true); break;
        }
           
    }

    public void HideParticles()
    {
        _positiveParticles.SetActive(false);
        _negativeParticles.SetActive(false);
        _neutralParticles.SetActive(false);
    }

    public void Modify(float newModifier)
    {
        modifier = newModifier;
    }

    IEnumerator WaitForChest(float time, RewardsData reward)
    {
        yield return new WaitForSeconds(time);
        ApplyReward(reward);
    }
}

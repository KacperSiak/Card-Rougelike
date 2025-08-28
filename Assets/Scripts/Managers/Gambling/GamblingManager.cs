using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;



public class GamblingManager : PersistenSingleton<GamblingManager>
{
    [SerializeField] private RewardStruct[] rewards;

    [SerializeField] private Button RollButton;
    [SerializeField] private Button ReturnButton;

    [SerializeField] private Image jokeIcon;
    [SerializeField] private TextMeshProUGUI joketext;

    public float modifier;

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
                ApplyReward(r.reward);
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

    public void Modify(float newModifier)
    {
        modifier = newModifier;
    }    
}

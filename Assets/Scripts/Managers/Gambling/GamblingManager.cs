using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;



public class GamblingManager : MonoBehaviour
{
    [SerializeField] private RewardStruct[] rewards;

    [SerializeField] private Button RollButton;
    [SerializeField] private Button ReturnButton;

    [SerializeField] private Image jokeIcon;
    [SerializeField] private TextMeshProUGUI joketext;
    public static GamblingManager instance;

    

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        RollButton.onClick.AddListener(Roll);
    }

    private void OnDisable()
    {
        RollButton.onClick.RemoveAllListeners();
    }

    private void Roll()
    {
        int total = rewards.Sum(r => r.weight);
        int roll = Random.Range(0, total);
        int acc = 0;
        foreach (var r in rewards)
        {
            acc += r.weight;
            if (roll<acc)
            {
                ApplyReward(r.reward);
                break;
            }
        }

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
            jokeIcon.sprite = joke.jokeIcon;
        }
        else
        {
            joketext.text = joke.joke;
        }
    }
}

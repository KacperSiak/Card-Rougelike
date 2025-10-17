using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Runtime.CompilerServices;


public class GamblingManager : PersistenSingleton<GamblingManager>
{
    [SerializeField] private RewardStruct[] rewards;

    [SerializeField] private Button RollButton;
    [SerializeField] private Button ReturnButton;


    [Header("ShowReward")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Button _accButton;

    public float modifier;
    private float potionModifier;
    private bool potionModified;
    private float _time = 1.5f;
    private Material[] _jokesMaterials;
    private RewardsData _currentReward;
    private RewardContext _ctx;

    [Header("Particles")]
    [SerializeField] private GameObject _positiveParticles;
    [SerializeField] private GameObject _neutralParticles;
    [SerializeField] private GameObject _negativeParticles;

    protected override void OnSingletonAwake() 
    {
        _jokesMaterials = Resources.LoadAll<Material>("JokesMaterials");
        _accButton.onClick.AddListener(ApplyReward);
        _ctx = new RewardContext();
    }

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
                ApplyEffect(r.type);
                StartCoroutine(WaitForChest(_time, r.reward, r.type));
                GamblingSceneManager.Instance.ShowReward(r);
                break;
            }
        }
        if (potionModified)
        {
            modifier /= potionModifier;
            potionModified = false;
        }
    }

    private void ApplyReward()
    {
        foreach (var effect in _currentReward.rewardEffect)
        {
            effect.AddReward(_ctx);
        }
    }

    public void ShowHermesJoke(HermesJoke joke)
    {
        Sprite sp = AtlasHelper.Cutoff(
            joke.atlas,
            joke.rows,
            joke.cols,
            joke.row,
            joke.col,
            rowZeroTop: true
        );
        _rewardImage.sprite = sp;
        _rewardImage.rectTransform.rotation = Quaternion.Euler(0, 0, -90f);
        _panel.SetActive(true);
        _rewardImage.gameObject.SetActive(true);
        _accButton.onClick.RemoveAllListeners();
        _accButton.onClick.AddListener(Close);
        
            
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

    private void ShowReward(RewardsData reward, RewardType type)
    {
        
        if(type != RewardType.Neutral)
        {
            _panel.SetActive(true);
            _rewardText.gameObject.SetActive(true);
            _rewardText.text = reward.rewardDesc;
            _currentReward = reward;
        }
        else
        {
            foreach (var effect in reward.rewardEffect)
            {
                effect.AddReward(_ctx);
            }
        } 
    }

    private void Close()
    {
        _panel.SetActive(false);
    }

    public void HideParticles()
    {
        _positiveParticles.SetActive(false);
        _negativeParticles.SetActive(false);
        _neutralParticles.SetActive(false);
    }

    public void Modify(float newModifier)
    {
        //if (potion)
        //{
        //    potionModifier = newModifier;
        //    potionModified = true;
        //}

        //if (reset) modifier /= newModifier;
        //else modifier *= newModifier;

    }

    IEnumerator WaitForChest(float time, RewardsData reward, RewardType type)
    {
        yield return new WaitForSeconds(time);
        GamblingSceneManager.Instance.CompleteTransaction();
        ShowReward(reward, type); // ...
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengesUI : MonoBehaviour
{
    [SerializeField] private Image _challengeIcon;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;

    private ChallengesData _currentChallenge;
    private void Awake()
    {
        _acceptButton.onClick.AddListener(AcceptChallenge);
    }
    public void Setup(ChallengesData challenge)
    {
        _currentChallenge = challenge;
        _name.text = challenge.challengeName;
        _description.text = challenge.challengeDescription;
        _challengeIcon.sprite = challenge.challIcon;
    }

    private void AcceptChallenge()
    {
        ChallengesMenager.Instance.ChallengeAccepted(_currentChallenge);
    }
}

using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyTease : MonoBehaviour
{
    [SerializeField] private GameObject _teasePanel;
    [SerializeField] private TextMeshProUGUI _teaseText;
    [SerializeField] private string[] _teaseLines;

    private float _teaseDelay = .03f;
    private float _afterGenerationWaitTime = 2;
    private bool _isTeasing = false;

    public void Tease()
    {
        if (_isTeasing) return;
        StartCoroutine("showWindow");
    }

    IEnumerator showWindow()
    {
        _teasePanel.SetActive(true);
        _isTeasing = true;

        _teaseText.maxVisibleCharacters = 0;
        _teaseText.text = _teaseLines[Random.Range(0, _teaseLines.Length-1)];

        for (int i = 0; i < _teaseText.text.Length; i++)
        {
            _teaseText.maxVisibleCharacters++;
            yield return new WaitForSeconds(_teaseDelay);
        }

        yield return new WaitForSeconds(_afterGenerationWaitTime);

        _teasePanel.SetActive(false);
        _isTeasing = false;
    }
}

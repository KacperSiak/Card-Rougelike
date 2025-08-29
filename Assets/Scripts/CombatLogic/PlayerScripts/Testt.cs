using UnityEngine;

public class Testt : MonoBehaviour
{
    public AnimationCurve curve;

    private float maxTime;
    private float _currentTime;

	private void Awake()
	{
		maxTime = curve.keys[^1].time;
	}

	// Update is called once per frame
	void Update()
    {
		_currentTime += Time.deltaTime;
		transform.position = new Vector3(0, curve.Evaluate(_currentTime), 0);
		if(_currentTime > maxTime)
			_currentTime = 0;
    }
}

using UnityEngine;

public class ValidateExample : MonoBehaviour
{
    [SerializeField] private Foo f;

	private void OnValidate()
	{
			f = GetComponent<Foo>();
	}
}

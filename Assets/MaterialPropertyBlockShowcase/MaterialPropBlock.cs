using UnityEngine;

public class MaterialPropBlock : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Texture2D _tex;
    [SerializeField] private Renderer _renderer;

	private MaterialPropertyBlock _block;

	private void OnValidate()
	{
		if(!_renderer)
			_renderer = GetComponent<Renderer>();

		if (_renderer)
		{
			Set();
		}
	}

	private void Awake()
	{
		Set();
	}

	private void Set()
	{
		_block = new();
		//_block.SetColor("_Color", _color);
		_renderer.SetPropertyBlock(_block);
	}
}

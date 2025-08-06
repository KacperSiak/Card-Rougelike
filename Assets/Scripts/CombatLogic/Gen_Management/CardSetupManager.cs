using UnityEngine;

public class CardSetupManager : MonoBehaviour
{
    public static CardSetupManager Instance { get; private set; }

    [SerializeField] private Material _back;
    [SerializeField] private Material _front;

    private MaterialPropertyBlock _block;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        _block = new();

    }

    public void SetupCardPhysics(GameObject card)
    {
        //setup card scale
        card.AddComponent<BoxCollider>();
        card.AddComponent<PlayerCardLogic>();
        //setup box collider scale
    }

    //still not working, need to read a bit more
    public void SetupCardVisual(MeshRenderer card, int col, int row)
    {   
        _block.SetFloat("_ColIndex", col);
        _block.SetFloat("_RowIndex", row);

        card.SetPropertyBlock(_block);

        card.materials[0] = _front;
        card.materials[1] = _back;
    }
}

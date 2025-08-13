using UnityEngine;

//here changes
public class CardSetupManager : MonoBehaviour
{
    public static CardSetupManager Instance { get; private set; }

    [SerializeField] private Material _front;
    [SerializeField] private Material _back;
    [SerializeField] private Material _sides;

    private MaterialPropertyBlock _block;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        _block = new();
    }

    //improve
    public void SetupCardPhysics(GameObject card)
    {
        //setup card scale
        card.AddComponent<BoxCollider>();
        card.AddComponent<PlayerCardLogic>();
        //card.GetComponent<Transform>().rotation = Quaternion.Euler(200f, 0f, 0f);
        card.transform.rotation = Quaternion.Euler(200f, 0f, 0f);
        card.GetComponent<BoxCollider>().size += new Vector3(0f, 0.01f, 0f);
        //setup box collider scale
    }

    //maybe improve 
    public void SetupCardVisual(MeshRenderer card, int col, int row)
    {
        Material[] mats = card.materials;
        mats[0] = _front;
        mats[1] = _back;
        mats[2] = _sides;
        card.materials = mats;

        _block.SetFloat("_ColIndex", col);
        _block.SetFloat("_RowIndex", row);

        card.SetPropertyBlock(_block);

        card.SetPropertyBlock(_block, 0);
    }
}

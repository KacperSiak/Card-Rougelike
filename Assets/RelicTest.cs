using UnityEngine;

public class RelicTest : MonoBehaviour
{
    [SerializeField] private RelicData relic1;
    [SerializeField] private RelicData relic2;
    [SerializeField] private RelicData relic3;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            RelicEventsSystem.RelicAcquired(relic1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            RelicEventsSystem.RelicAcquired(relic2);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            RelicEventsSystem.RelicAcquired(relic3);
        }
    }
}

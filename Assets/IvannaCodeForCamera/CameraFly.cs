using UnityEngine;

public class CameraFly : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    private int currentIndex = 0;
    private bool isFlying = false;
    private Vector3 defaultPosition;

    void Start()
    {
        defaultPosition = Camera.main.transform.position;
    }
    void OnMouseDown()
    {
        
            if (!isFlying)
        {
            //Transform target = waypoints[currentIndex];
            //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            Camera.main.transform.LookAt(target);
            isFlying = true;
            return;

           // if (Vector3.Distance(transform.position, target.position) < 0.1f)
           //{
           // currentIndex++;
           //}
        }
        Camera.main.transform.position = defaultPosition;
        isFlying = false ;
    }
}
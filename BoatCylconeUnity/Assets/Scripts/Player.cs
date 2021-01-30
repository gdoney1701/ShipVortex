using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    class PlayerConfig {

        public PlayerConfig(GameObject _parent) {

        }
        
        public float speed;


    }

    PlayerConfig playerConfig;

    Rigidbody rb;
    GameObject ph_Pointer;
    float ph_pointerRadius;
    private void Awake()
    {

    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerConfig = new PlayerConfig(gameObject);

        playerConfig.speed = 5;

        ph_Pointer = transform.Find("ph_pointer").gameObject;
        ph_pointerRadius = (transform.position - ph_Pointer.transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementFromKeys();
    }

   



    private void updateMovementFromKeys() {
        //implement Brackeys whatever soon
        
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        updateReticlePosition();
        rb.position += new Vector3(xInput, 0, yInput) * Time.deltaTime * playerConfig.speed;

    }

    private void updateReticlePosition() {

        //cast a ray from the mouse at the angle of the camera and stop it at the plane of the character

        //ok lots of math
        Debug.Log(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, -rb.position.y);

        float distance;
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            mouseWorldPosition = ray.GetPoint(distance);
            
            
        }

        Vector3 vectorToMouse = mouseWorldPosition - rb.position;

        Debug.DrawLine(rb.position, mouseWorldPosition, Color.red);
        ph_Pointer.transform.position = transform.position + (vectorToMouse.normalized * ph_pointerRadius);
        /*
        var mouseStart = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        
        //RaycastHit hit = Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        RaycastHit hit;
        if (Physics.Raycast(mouseStart, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(mouseStart, Camera.main.transform.forward * hit.distance, Color.yellow);
        }

        */
    }
}

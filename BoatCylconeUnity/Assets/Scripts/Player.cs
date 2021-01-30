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

    Vector3 vectorToMouse;

    //Delegates
    public delegate void OnFireDelegate();
    public static OnFireDelegate fireDelegate;
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

        fireDelegate += fireFeedback;
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementFromKeys();

        if (Input.GetButtonDown("Fire1")) {
            fireDelegate();
        }
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
        Plane playerPlane = new Plane(Vector3.up, -rb.position.y);

        float distance;
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (playerPlane.Raycast(ray, out distance))
        {
            mouseWorldPosition = ray.GetPoint(distance);
            
            
        }

        vectorToMouse = mouseWorldPosition - rb.position;
        vectorToMouse = vectorToMouse.normalized;
        Vector3 finalPosition = transform.position + (vectorToMouse.normalized * ph_pointerRadius);

        ph_Pointer.transform.position = Vector3.Lerp(ph_Pointer.transform.position, finalPosition, 0.1f);
    
    }

    void fireFeedback() {
        Debug.Log("fire");

    }
}

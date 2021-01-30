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

    private void Awake()
    {

    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerConfig = new PlayerConfig(gameObject);

        playerConfig.speed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        updateMovementFromKeys();
    }

   



    private void updateMovementFromKeys() {
        //implement Brackeys whatever soon
        /*
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        rb.position += new Vector3(xInput, 0, yInput) * Time.deltaTime * playerConfig.speed;

        */

        Debug.Log(Input.GetAxis("Horizontal"));



    }
}

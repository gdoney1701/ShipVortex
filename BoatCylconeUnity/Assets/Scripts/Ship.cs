using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 velocity;

    private GameManager GM;
    private Rigidbody rb;
    private Vector3 perpendicularVector = new Vector3();
    void Start()
    {
        GM = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        updateVelocityForCyclone(true);
        

    }


    private void updateVelocityForCyclone(bool maintainRadius) {
        Vector3 vectorToCenterOfCyclone = GM.getVectorToCyclone(rb.position);

        var startRad = vectorToCenterOfCyclone.magnitude;
        //Do not care about the y
        vectorToCenterOfCyclone.Scale(new Vector3(1, 0, 1));

        Vector3 perpendicularVector = Quaternion.Euler(0, 90, 0) * vectorToCenterOfCyclone;

        Debug.DrawRay(transform.position, perpendicularVector);

        //slowly getting wider 
        rb.position += perpendicularVector * Time.deltaTime * GM.cycloneConfig.speed;
        vectorToCenterOfCyclone = GM.getVectorToCyclone(rb.position);

        if (maintainRadius) {
            rb.position += vectorToCenterOfCyclone.normalized * (vectorToCenterOfCyclone.magnitude - startRad);
        }
    }

}

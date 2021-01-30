using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject cycloneParent;
    public class CycloneConfig {
        
        public float speed;
        //outer edges
        public float minPull;

        public float maxPull;

        public float innerRadius;
        public float outerRadius;
        public GameObject parent;
        public CycloneConfig(GameObject _parent)
        {
            parent = _parent;
        }
        public void drawDebug() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(parent.transform.position, innerRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(parent.transform.position, outerRadius);

        }
    }


    public CycloneConfig cycloneConfig;
    void Start()
    {
        cycloneConfig = new CycloneConfig(cycloneParent);
        cycloneConfig.speed = 5;
        cycloneConfig.minPull = 5;
        cycloneConfig.maxPull = 5;
        cycloneConfig.innerRadius = 1;
        cycloneConfig.outerRadius = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        try
        {
            //cycloneConfig.drawDebug();
        }
        catch (System.Exception e) {
        }
       
    }

    public Vector3 getVectorToCyclone(Vector3 shipPosition) {

        return cycloneParent.transform.position - shipPosition;

    }
    public float calculatePull(float distance)
    {
        return 0;
    }
}



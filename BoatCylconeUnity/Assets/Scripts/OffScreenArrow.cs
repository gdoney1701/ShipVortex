using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenArrow : MonoBehaviour
{
    public Canvas mainCanvas;
    public Sprite arrowPrefab;

    public GameObject[] portPool;
    public Image[] arrowPool;

    // Update is called once per frame
    void LateUpdate()
    {
        makeArrows();
    }


    void makeArrows()
    {
        for(int i = 0; i < portPool.Length; i++)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(portPool[i].transform.position);
            if (screenpos.z>0 && 
                screenpos.x >0 && screenpos.x < Screen.width && 
                screenpos.y>0 && screenpos.y < Screen.height)
            {
                arrowPool[i].enabled = false;
                string statement = portPool[i].name + " is Off Screen";
                Debug.Log(statement);
            }
            else
            {
                string statement = portPool[i].name + " is Off Screen";
                Debug.Log(statement);
                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

                //Centers the screen origin instead of it being in the bottom left corner
                screenpos -= screenCenter;

                //find angle from center of screen to mouse position
                float angle = Mathf.Atan2(screenpos.y, screenpos.x);
                angle -= 90 * Mathf.Deg2Rad;

                float cosTheta = Mathf.Cos(angle);
                float sinTheta = Mathf.Sin(angle);

                screenpos = screenCenter + new Vector3(sinTheta * 150, cosTheta * 150, 0);

                float m = cosTheta / sinTheta;

                Vector3 screenBounds = screenCenter * 0.9f;

                if (cosTheta > 0)
                {
                    screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                }
                else
                {
                    screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                }

                if (screenpos.x > screenBounds.x)
                {
                    screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                }else if (screenpos.x < -screenBounds.x)
                {
                    screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                }

                screenpos += screenCenter;
                arrowPool[i].enabled = true;

                arrowPool[i].transform.localPosition = screenpos;
                arrowPool[i].transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);




            }
        }
    }
}

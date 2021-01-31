using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TeamEnum
{
    Red,
    Yellow,
    Green,
    Blue
}
public class EndZone : MonoBehaviour
{
    // Start is called before the first frame update

    private GameManager GM;
    void Start()
    {
        GM = FindObjectOfType<GameManager>();
    }
    

    public TeamEnum assocTeam;
    public GameObject flag;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ship>()) {
            Ship assocShip = other.GetComponent<Ship>();
            if (assocShip.GetTeam() == assocTeam) {
                GM.modScore(5f);
            }
            else{
                GM.modScore(1f);
            }

            assocShip.dockShip(this);
        }
    }
}

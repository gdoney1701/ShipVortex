using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ship : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 velocity;

    private GameManager GM;
    private Rigidbody rb;
    private Vector3 perpendicularVector = new Vector3();
    public AudioClip impactClip;
    public AudioClip travelingClip;

    
    private float cycloneWeight = 1;
    private AudioSource soundSource;

    public TeamEnum assocTeam;

    public class IgnoreState
    {
        public bool ignoreWind;
        public bool ignoreCyclone = false;
    }
    public IgnoreState ignoreState;
    public class WindInteraction {
        
        public float amount = 0;
        public float windTime = 0.4f;

        public Vector3 windDirection;
        public float lifeTime;
        public AnimationCurve fallOff;
        public WindInteraction(Projectile _projectile, float _lifetime, AnimationCurve _fallOff) {
            windDirection = _projectile.direction;
            lifeTime = _lifetime;
            fallOff = _fallOff;
        }


        public Vector3 updateWind() {
            amount += Time.deltaTime * (1f/ windTime);
            return windDirection * fallOff.Evaluate(amount);
        }
    }
    
    private HashSet<WindInteraction> gustsAffecting = new HashSet<WindInteraction>();
    private HashSet<Projectile> projectilesHit = new HashSet<Projectile>();

    public delegate void OnDockDelegate();
    public static OnDockDelegate onDock;

    private void Awake()
    {
        ignoreState = new IgnoreState();
    }
    void Start()
    {
        
        
        GM = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
        soundSource = GetComponent<AudioSource>();
        soundSource.clip = impactClip;

        assocTeam = TeamEnum.Blue;

        
    }

    // Update is called once per frame
    void Update()
    {

        
        updateVelocityForCyclone(true);
        updateAllGusts();

    }
    public void dockShip(EndZone port) {
        // onDock();
        ignoreState.ignoreWind = true;
        ignoreState.ignoreCyclone = true;
        dockShipTween(port.gameObject.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ignoreState.ignoreWind) {
            return;
        }
        if (other.GetComponent<Projectile>()) {
            WindInteraction newWind = new WindInteraction(other.GetComponent<Projectile>(), 1, GM.windFalloff);
            
            if (projectilesHit.Contains(other.GetComponent<Projectile>())){

                return;
            }
            soundSource.Play();
            
            projectilesHit.Add(other.GetComponent<Projectile>());
            gustsAffecting.Add(newWind);
        }
        
    }
    private void updateAllGusts() {

        if (ignoreState.ignoreWind)
        {
            return;
        }
        List<WindInteraction> pendingDeletion = new List<WindInteraction>();
        float highestGust = 0;
        foreach (WindInteraction wind in gustsAffecting) {
            if (wind.amount >= 1) {
                pendingDeletion.Add(wind);
                continue;
            }
            highestGust = Mathf.Max(highestGust, wind.amount);
            Vector3 windVector = wind.updateWind() / 10;

            rb.position += windVector;

        }
        //highest gust high if there is a lot of wind effecting it
        /*
        if (highestGust > 0 && highestGust < 0.5f)
        {
            cycloneWeight = 0;
            Debug.Log("i frames!!!");
        }
        else if (highestGust == 0) {
            cycloneWeight = 1;
        }
        else
        {
            cycloneWeight = (highestGust-0.5f) * (1/0.5f);
        }
        */
        //cycloneWeight = 0;


        foreach (WindInteraction wind in pendingDeletion) {
            gustsAffecting.Remove(wind);
        }
    }

    private void dockShipTween(Vector3 dockCenter) {
        

        Sequence dockSequence = DOTween.Sequence();
        dockSequence.Append(rb.DOMove(dockCenter, 0.5f).SetEase(Ease.OutQuad));
        dockSequence.Append(transform.DOScale(Vector3.one *2, 0.4f).SetEase(Ease.InOutQuad));
        dockSequence.Append(transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InQuad));
        dockSequence.onComplete += () => {
            Destroy(gameObject);

            GM.levelManager.checkForEndOfLevel();
        };
        //dockSequence.Append()
    }


    private void blendVelocities() {
        //blend velocity after hitting
    }

    private void updateVelocityForCyclone(bool maintainRadius) {

        if (ignoreState.ignoreCyclone)
        {
            return;
        }

        Vector3 vectorToCenterOfCyclone = GM.getVectorToCyclone(rb.position);

        var startRad = vectorToCenterOfCyclone.magnitude;
        //Do not care about the y
        vectorToCenterOfCyclone.Scale(new Vector3(1, 0, 1));

        Vector3 perpendicularVector = Quaternion.Euler(0, 90, 0) * vectorToCenterOfCyclone;

        Debug.DrawRay(transform.position, perpendicularVector);

        //slowly getting wider 
        
        rb.position += perpendicularVector.normalized * Time.deltaTime * GM.cycloneConfig.speed * cycloneWeight;
        vectorToCenterOfCyclone = GM.getVectorToCyclone(rb.position);
        
        if (maintainRadius) {
            rb.position += vectorToCenterOfCyclone.normalized * (vectorToCenterOfCyclone.magnitude - startRad);
        }

        vectorToCenterOfCyclone.Scale(new Vector3(1, 0, 1));
        rb.position += vectorToCenterOfCyclone.normalized * GM.calculatePull(vectorToCenterOfCyclone.magnitude) * Time.deltaTime * cycloneWeight;

    }

    public TeamEnum GetTeam() {
        return assocTeam;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    class PlayerConfig {
        public PlayerConfig(GameObject _parent) {

        }
        public float speed;
    }

    class TweenData {
        public bool positionTweening = false;
    }

    PlayerConfig playerConfig;
    TweenData tweenData;
    Rigidbody rb;
    GameObject ph_Pointer;
    GameObject body;
    public GameObject projectilePrefab;
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
        tweenData = new TweenData();
        playerConfig.speed = 5;

        ph_Pointer = transform.Find("ph_pointer").gameObject;
        body = transform.Find("body").gameObject;
        ph_pointerRadius = (transform.position - ph_Pointer.transform.position).magnitude;

        fireDelegate += fireFeedback;
        fireDelegate += fireProjectile;
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
    void fireProjectile() {
        Projectile newProjectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
        newProjectile.gameObject.transform.position = ph_Pointer.transform.position;
        
        
        
        newProjectile.initialize(ph_Pointer.transform.position, vectorToMouse);


    }

    void fireFeedback() {
        if (tweenData.positionTweening) {
            return;
        }
        tweenData.positionTweening = true;
        

        Vector3 startPos = body.transform.localPosition;
        Sequence feedBackTween = DOTween.Sequence();
        feedBackTween.Append(body.transform.DOLocalMove(startPos - vectorToMouse * 0.3f, 0.2f).SetEase(Ease.OutCubic));
        feedBackTween.Append(body.transform.DOLocalMove(startPos, 0.2f).SetEase(Ease.InCubic));

        feedBackTween.onComplete += () =>
        {
            tweenData.positionTweening = false;
        };
    }
}

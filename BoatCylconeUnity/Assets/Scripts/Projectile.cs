using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    public Vector3 direction = new Vector3();

    class ProjectileConfig {
        public float speed;
        public float lifetime;
        public float maxScale;


    }
    ProjectileConfig projectileConfig;
    void Awake()
    {
        
        projectileConfig = new ProjectileConfig();
        projectileConfig.speed = 40f;
        projectileConfig.lifetime = 4f;
        projectileConfig.maxScale = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        rb.position += direction * projectileConfig.speed * Time.deltaTime;
    }

    public void initialize(Vector3 pos, Vector3 _direction) {
        rb = GetComponent<Rigidbody>();

        handleSizeTween();
        rb.position = pos;
        direction = _direction;

        
    }

    public void handleSizeTween() {
        Sequence scaleSequence = DOTween.Sequence();
        transform.localScale = Vector3.zero;
        scaleSequence.Append(transform.DOScale(projectileConfig.maxScale, projectileConfig.lifetime * 0.1f).SetEase(Ease.InOutSine));
        scaleSequence.Append(transform.DOScale(0, projectileConfig.lifetime * 0.9f).SetEase(Ease.InOutSine));
        scaleSequence.onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}

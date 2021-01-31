using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("UI and such")]
    public TMP_Text scoreText;

    public delegate void OnScoreChange();
    public static OnScoreChange scoreDelegate;

    public GameObject cycloneParent;
    public class CycloneConfig {
        
        public float speed;
        //outer edges
        public float minPull;

        public float maxPull;

        public float innerRadius;
        public float outerRadius;
        public GameObject parent;
        public bool scoreTweening = false;
        [SerializeField]
        
        public CycloneConfig(GameObject _parent)
        {
            parent = _parent;
            scoreTweening = false;
        }
        public void drawDebug() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(parent.transform.position, innerRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(parent.transform.position, outerRadius);

        }


    }

    public class LevelManager {
        int currentLevel = 1;
       public bool levelIsLoading = false;
        public GameObject levelParent;
        GameObject currentLevelGO;
        private float levelLoadTime = 2f;
        int shipsLeft;
        int shipsGone;
        int totalLevels;

        public delegate void onGameOverDelegate();
        public static onGameOverDelegate onGameOver;

        public LevelManager(GameObject _levelParent) {
            levelParent = _levelParent;
        }

        public void loadLevel(int level) {
            levelIsLoading = true;
            totalLevels = levelParent.transform.childCount;
            currentLevelGO = levelParent.transform.GetChild(level - 1).gameObject;
            currentLevelGO.SetActive(true);
            Ship[] shipsInLevel = currentLevelGO.GetComponentsInChildren<Ship>();
            shipsLeft = shipsInLevel.Length;
            shipsGone = 0;
            foreach (Ship s in shipsInLevel) {
                Debug.Log(s);
                s.ignoreState.ignoreWind = true;
                s.ignoreState.ignoreCyclone = true;
            }
            
            Vector3 startPos = currentLevelGO.transform.position - Vector3.up * 3;
            Sequence levelLoadSequence = DOTween.Sequence();
            levelLoadSequence.Append(currentLevelGO.transform.DOMove(startPos, levelLoadTime).From().SetEase(Ease.OutBack));
            levelLoadSequence.onComplete += () => {
                levelIsLoading = false;
                foreach (Ship s in shipsInLevel)
                {
                    s.ignoreState.ignoreWind = false;
                    s.ignoreState.ignoreCyclone = false;
                }
            };

            onGameOver += () => {
                Debug.Log("Game over");
            };
        }

        public void checkForEndOfLevel() {
            shipsGone++;

            if (shipsLeft == shipsGone) {
                currentLevel++;
                if (currentLevel > totalLevels)
                {
                    onGameOver();
                }
                else {
                    loadLevel(currentLevel);
                }
                
            }
        }

    }

    public float score;
    public AnimationCurve speedFalloff;
    public AnimationCurve windFalloff;
    public CycloneConfig cycloneConfig;

    public LevelManager levelManager;
    public GameObject levelParent;
    void Start()
    {
        cycloneConfig = new CycloneConfig(cycloneParent);
        levelManager = new LevelManager(levelParent);
        cycloneConfig.speed = 5;
        cycloneConfig.maxPull = 4;
        cycloneConfig.innerRadius = 1;
        cycloneConfig.outerRadius = 10;
        scoreDelegate += scaleScore;
        Debug.Log(speedFalloff.Evaluate(0.5f));
        levelManager.loadLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        try
        {
            cycloneConfig.drawDebug();
        }
        catch (System.Exception e) {
        }
       
    }

    public Vector3 getVectorToCyclone(Vector3 shipPosition) {

        return cycloneParent.transform.position - shipPosition;

    }
    private void scaleScore() {
        if (cycloneConfig.scoreTweening) {
            return;
        }
        cycloneConfig.scoreTweening = true;
        Sequence s = DOTween.Sequence();

        s.Append(scoreText.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuad));
        s.Append(scoreText.transform.DOScale(1f, 0.3f).SetEase(Ease.InQuad));


        s.onComplete += () =>
        {
            cycloneConfig.scoreTweening = false;
        };
    }
    public void modScore(float mod) {
        score += mod;
        scoreText.SetText("Score: " + score.ToString());
        scoreDelegate();
    }

    public float calculatePull(float distance)
    {
        if (distance < cycloneConfig.innerRadius)
        {
            
            return 0;
        }

        else if (distance > cycloneConfig.outerRadius) {
            return 1;
        }
        
        float modDistance = distance - cycloneConfig.innerRadius;
        float percent = 1-(modDistance / (cycloneConfig.outerRadius - cycloneConfig.innerRadius));


        return speedFalloff.Evaluate(percent) * cycloneConfig.maxPull;
    }

    public void spawnShip() {
        Vector3 pointToSpawn = new Vector3(0, 0, 0);
    }
}



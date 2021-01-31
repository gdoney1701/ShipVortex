using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("UI and such")]
    public TMP_Text scoreText;
    public TMP_Text tutorialText;
    public Image blackPanel;
    public TMP_Text gameEndText;
    public TMP_Text endScore;
    public TMP_Text numShots;
    public TMP_Text finalMessage;
    public GameObject gameOverParent;

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
    public AudioSource shipSpawnSound;
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
        private GameManager GM;
        public LevelManager(GameObject _levelParent, GameManager _GM) {
            levelParent = _levelParent;
            GM = _GM;
        }

        public void loadLevel(int level) {
            GM.shipSpawnSound.Play();
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
                GM.gameOver = true;
                GM.cameraManager.gameOverCameraMove();
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
    public CameraManager cameraManager;
    public class CameraManager {
        Camera cam;
        Vector3 rootPos;
        Quaternion rootRot;
        GameManager GM;
        Vector3 vertOffset = new Vector3(0, 3, 0);
        bool finalCamFired = false;
        public CameraManager(Camera _cam, GameManager _GM) {
            cam = _cam;
            rootPos = cam.transform.position;
            rootRot = cam.transform.rotation;
            GM = _GM;
        }

        public void lerpToPlaceAndLookAtFlag(Sequence s, TeamEnum team, string tutorialMessage) {
            Vector3 toFlagVector;
            s.AppendCallback(() => {
                GM.tutorialText.SetText(tutorialMessage);
            });
            s.Append(cam.transform.DOMove(GM.endzoneDictionary[team].transform.position + vertOffset, 2).SetEase(Ease.InOutSine));
            toFlagVector = GM.endzoneDictionary[team].flag.transform.position - (GM.endzoneDictionary[team].transform.position + vertOffset);
            s.Append(cam.transform.DORotateQuaternion(Quaternion.LookRotation(toFlagVector), 2).SetEase(Ease.InOutSine));

        }
        public void tutorialCameraAnim() {
            Sequence tutSeq = DOTween.Sequence();
            tutSeq.AppendInterval(3f);
            GM.tutorialText.SetText("Welcome to Cyclone Sentry!");
            lerpToPlaceAndLookAtFlag(tutSeq, TeamEnum.Red, "in this game, you play as a god, guiding ships lost at sea");
            lerpToPlaceAndLookAtFlag(tutSeq, TeamEnum.Green, "beware the cyclone in the middle of the map!");
            lerpToPlaceAndLookAtFlag(tutSeq, TeamEnum.Blue, "blow the ships towards the port that has the same color as their flag");
            lerpToPlaceAndLookAtFlag(tutSeq, TeamEnum.Yellow, "use WASD to move, and click to shoot:)");
            tutSeq.AppendCallback(() => {

                cam.transform.DORotateQuaternion(rootRot, 2).SetEase(Ease.InOutSine);
            });
            tutSeq.Append(cam.transform.DOMove(rootPos, 2).SetEase(Ease.InOutSine));
            tutSeq.AppendCallback(() => {

                GM.tutorialText.SetText("try moving around and shooting! a ship will spawn shortly...");
            });
            tutSeq.AppendInterval(5f);
            tutSeq.onComplete += () =>
            {
                GM.tutorialText.SetText("");
                GM.levelManager.loadLevel(1);
            };

        }

        public void gameOverCameraMove() {
            if (finalCamFired) {
                return;
            }
            finalCamFired = true;
            Sequence s = DOTween.Sequence();
            
            s.AppendInterval(0.5f);
            s.Append(GM.blackPanel.DOFade(1f, 1f));
            cam.transform.DOMove(GM.cycloneParent.transform.position, 2f).SetEase(Ease.InOutSine);
            cam.transform.DORotateQuaternion(Quaternion.LookRotation(-Vector3.up), 2f).SetEase(Ease.InOutSine);
            GM.numShots.SetText("You fired wind " + GM.numberTimesFired.ToString() + " times");
            GM.endScore.SetText("Score: " + GM.score);
            s.AppendInterval(0.5f);
            s.Append(GM.gameEndText.DOColor(Color.white, 1f));
            s.Append(GM.endScore.DOColor(Color.white, 1f));
            s.Append(GM.numShots.DOColor(Color.white, 1f));
            s.Append(GM.finalMessage.DOColor(Color.white, 1f));
            s.AppendInterval(3f);
            s.AppendCallback(() => {
                SceneManager.LoadScene(0);
            });
        }



    }

    Dictionary<TeamEnum, EndZone> endzoneDictionary = new Dictionary<TeamEnum, EndZone>();
    public int numberTimesFired = 0;
    public bool gameOver = false;
    void Start()
    {
        cycloneConfig = new CycloneConfig(cycloneParent);
        levelManager = new LevelManager(levelParent, this);
        cameraManager = new CameraManager(Camera.main, this);
        cycloneConfig.speed = 10;
        cycloneConfig.maxPull = 4;
        cycloneConfig.innerRadius = 10;
        cycloneConfig.outerRadius = 30;
        scoreDelegate += scaleScore;
        //levelManager.loadLevel(1);
        
        foreach (EndZone e in FindObjectsOfType<EndZone>()) {
            endzoneDictionary.Add(e.assocTeam, e);
        }
        blackPanel.color = Color.clear;
        gameEndText.color = Color.clear;
        endScore.color = Color.clear;
        numShots.color = Color.clear;
        finalMessage.color = Color.clear;
        cameraManager.tutorialCameraAnim();
        //levelManager.loadLevel(1);
    
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
            
            return -1;
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



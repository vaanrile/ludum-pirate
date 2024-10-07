using BBX.Dialogue.GUI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool IsOn;

    [SerializeField]
    public int nbMoskitos = 0;

    [SerializeField]
    private GameObject mostikoPrefab;


    [SerializeField]
    private Player player;

    [SerializeField]
    private Encens encens;

    [SerializeField]
    private S_Radiateur radiateur;

    [SerializeField]
    private WatchScript Time;

    [SerializeField]
    private S_Telephone tel;

    [SerializeField]
    private BoxCollider moskitoBox;

    [SerializeField]
    private int nbStingBeforeLose = 3;

    [SerializeField]
    private List <Moskito> moskitoList = new List<Moskito>();

    private int nbPiqure = 0;

    public TextMeshAnimator textMeshAnimator;
    private bool _leftMainMenu;
    public CanvasGroup canvasGroupMainMenu;
    public CanvasGroup canvasGroupWin;
    public CanvasGroup canvasGroupLose;
    public Animator playerAnim;

    public TextMeshProUGUI textStat;
    public TextMeshProUGUI textFun;
    public List<string> resultText;

    private bool _endGame;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is already an instance of GameManager in the scene, only one can be instanciated.");
        }

        if (mostikoPrefab == null)
        {
            Debug.LogError("MoskitoPrefab is not referenced, please add moskito prefab in game manager.");
        }
        else
        {
            for (int i = 0; i < nbMoskitos; i++)
            {
                var bounds = moskitoBox.bounds;
                Vector3 randomLocInBox = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z));
                var moskito = Instantiate(mostikoPrefab, randomLocInBox, Quaternion.identity).GetComponent<Moskito>();
                moskito.SetPlayer(player);
                moskito.SetEncens(encens);
                moskito.SetRadiateur(radiateur);
                moskito.SetTel(tel);
                moskito.SetMoskitoBox(moskitoBox);
                moskitoList.Add(moskito);
            }
        }
        playerAnim.speed = 0;

    }

    public void StartMenu()
    {
        textMeshAnimator.InitRead();
        AudioManager.instance.StartSound();
        canvasGroupMainMenu.DOFade(0, 1).OnComplete(() =>
        {
            canvasGroupMainMenu.gameObject.SetActive(false);
            playerAnim.speed = 1;
        });
    }

    public void lightSet(bool isOn)
    {
        foreach(Moskito kito in moskitoList)
        {
            kito.setParticle(isOn);
        }
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    public void GameReload()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void SetRandomTargetMoskito(GameObject randomTargetMoskito)
    {
        var bounds = moskitoBox.bounds;

        Vector3 newpos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));

        if((newpos - tel.transform.position).magnitude < tel.GetRadius())
        {
            newpos = radiateur.transform.position;
        }

        randomTargetMoskito.transform.position = newpos;
    }

    public void GameStart()
    {
        player.StartMove();
        foreach(Moskito mos in moskitoList)
        {
            mos.StartMove();
        }
        Time.StartTime();
    }

    public void MoskitoKill(Moskito kito)
    {
        nbMoskitos--;
        moskitoList.Remove(kito);
        if(nbMoskitos == 0)
        {
            var heure = Time.remainingHours;
            var minutes = Time.remainingMinutes;

            var minutesTmp = heure * 60 + minutes;

            minutesTmp = 420 - minutesTmp;

            int heureRestante = (int)(minutesTmp / 60);
            int minutesRestante = minutesTmp%60;

            textStat.text = "Hours slept : " + heureRestante + "h" + minutesRestante + "\r\nMosquitos bites : " + nbPiqure + "\r\n";

            if (heure < 1)
            {
                textFun.text = resultText[0];
            }
            else if (heure < 3)
            {
                textFun.text = resultText[1];
            }
            else if (heure < 5)
            {
                textFun.text = resultText[2];
            }
            else
            {
                textFun.text = resultText[3];
            }
            canvasGroupWin.DOFade(1, 1);
            _endGame = true;
        }
    }

    public void PlayerScream()
    {

    }

    public void PlayerPique()
    {
        nbPiqure++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&!_leftMainMenu) 
        {
            StartMenu();
            _leftMainMenu = true;
        }
        if (!_endGame)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameReload();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameQuit();
        }
    }

    public void LoseCondition()
    {
        canvasGroupLose.DOFade(1, 1);
        _endGame = true;
    }

}

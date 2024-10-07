using BBX.Dialogue.GUI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    private BoxCollider moskitoBox;

    [SerializeField]
    private int nbStingBeforeLose = 3;

    [SerializeField]
    private List <Moskito> moskitoList = new List<Moskito>();

    public TextMeshAnimator textMeshAnimator;
    private bool _leftMainMenu;
    public CanvasGroup canvasGroupMainMenu;
    public Animator playerAnim;

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

    public void GameQuit()
    {

    }
    public void GameReload()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GameStart()
    {
        player.StartMove();
        foreach(Moskito mos in moskitoList)
        {
            mos.StartMove();
        }
        
    }

    public void MoskitoKill(Moskito kito)
    {
        nbMoskitos--;
        moskitoList.Remove(kito);
        if(nbMoskitos == 0)
        {
            Debug.Log("WIN CONDITION");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&!_leftMainMenu) 
        {
            StartMenu();
            _leftMainMenu = true;
        }
    }

}

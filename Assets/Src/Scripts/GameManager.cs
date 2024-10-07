using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool IsOn;

    [SerializeField]
    public int nbMoskitos = 0;

    [SerializeField]
    private GameObject mostikoPrefab;

    public GameObject randomTargetMoskito;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Encens encens;

    [SerializeField]
    private S_Radiateur radiateur;

    [SerializeField]
    private S_Telephone tel;

    [SerializeField]
    private BoxCollider moskitoBox;

    [SerializeField]
    private int nbStingBeforeLose = 3;

    [SerializeField]
    private List <Moskito> moskitoList = new List<Moskito>();

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
        
    }

    public GameObject SetRandomTargetMoskito()
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

        return randomTargetMoskito;
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Parent : MonoBehaviour
{
    protected Game game;

    private void Awake()
    {
        game = GameObject.Find("Game").GetComponentInChildren<Game>();
    }

}

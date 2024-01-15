using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Vector2Int CurrentPos => currentPos;

    protected Vector2Int currentPos;
    
    public void Kill()
    {
        GameMaster.instance.UnregisterCharacter(currentPos,this);
        Destroy(gameObject);
    }

    private void Start()
    {
        GameMaster.instance.RegisterCharacter(this);
        currentPos = ((Vector2)transform.position).Round();
        Messenger.AddListener("EndGame",OnEndGame);
    }

    void Update()
    {
        ///Сообщаем свою позицию игровому мастеру
        var pos = ((Vector2)transform.position).Round();
        if (pos != currentPos)
        {
            GameMaster.instance.MoveCharacter(currentPos,pos,this);
            currentPos = pos;
        }
    }

    void OnEndGame()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("EndGame",OnEndGame);
    }
    
}

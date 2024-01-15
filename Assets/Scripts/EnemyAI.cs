using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        //При достижении центра клетки мы перерасчитываем путь к игроку с помощью
        //пасфайндера и двигаемся в направлении следующей точки маршрута
        if (!characterMovement.IsMoving)
        {
            var pos = ((Vector2)transform.position).Round();
            if (GameMaster.instance.Player != null)
            {
                var path = AstarPathfinder.Pathfind(pos,
                    GameMaster.instance.Player.CurrentPos, GameMaster.instance.Grid);

                if (path != null && path.Count > 1)
                {
                    characterMovement.Move(path[^2] - pos);
                }
            }
        }
    }
}

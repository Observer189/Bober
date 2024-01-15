using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject spawnPrefab;
    [SerializeField]
    protected float timeBetweenSpawns;
    [SerializeField]
    protected Transform[] spawnPoints;
    [Tooltip("Радиус в клетках от игрока, в котором нельзя делать спавн.")]
    [SerializeField]
    protected int safeZoneAroundPlayerRadius;
    
    protected float lastSpawnTime = float.MinValue;

    private void Start()
    {
        Messenger.AddListener("EndGame",OnEndGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMaster.instance.GameState == GameState.Game)
        {
            //При достижении времени спавна выбираем случайный спавнер, чтобы на нем заспавнить противника
            if (Time.time - lastSpawnTime > timeBetweenSpawns)
            {
                var spawnerList = new List<Vector2Int>();
                var playerPos = GameMaster.instance.Player.CurrentPos;
                //К случайному спавну допускаем только спавнеры, которые находятся на достаточном удалении от игрока
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    var pos = ((Vector2)spawnPoints[i].position).Round();
                    if ((playerPos - pos).ManhattanMagnitude() > safeZoneAroundPlayerRadius)
                    {
                        spawnerList.Add(pos);
                    }
                }

                var randInd = Random.Range(0,spawnerList.Count);

                Instantiate(spawnPrefab, spawnerList[randInd].ToVector2(), Quaternion.identity);

                lastSpawnTime = Time.time;
            }
        }
    }

    void OnEndGame()
    {
        lastSpawnTime = float.MinValue;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("EndGame", OnEndGame);
    }
}

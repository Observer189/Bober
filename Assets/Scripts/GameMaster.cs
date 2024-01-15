using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DefaultNamespace;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //Синглтон
    public static GameMaster instance { get; private set; }

    public CellState[,] Grid => grid;
    public Character Player => player;

    public GameState GameState => gameState;

    [SerializeField]
    protected int mapWidth;
    [SerializeField]
    protected int mapHeight;
    [SerializeField]
    protected Transform wallParent;
    [SerializeField]
    protected Transform playerSpawn;
    [SerializeField]
    protected GameObject playerPrefab;
    [SerializeField]
    protected CinemachineVirtualCamera camera;
    [SerializeField]
    protected RectTransform mainMenu;
    [SerializeField]
    protected RectTransform pauseMenu;
    [SerializeField]
    protected RectTransform deathMenu;
    [SerializeField]
    protected RectTransform winMenu;
    [SerializeField]
    protected Transform[] winPositions;

    protected Vector3 cameraMainMenuPos;

    protected CellState[,] grid;

    protected Dictionary<Vector2Int, List<Character>> characters;

    protected Character player;

    protected GameState gameState;

    public GameMaster()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Awake()
    {
        grid = new CellState[mapWidth,mapHeight];
        characters = new Dictionary<Vector2Int, List<Character>>();
    }

    void Start()
    {
        //В начале игры включаем главное меню и запоминаем позицию камеры для него
        cameraMainMenuPos = camera.transform.position;
        pauseMenu.gameObject.SetActive(false);
        deathMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameState == GameState.Game)
            {
                PauseGame();
            }
            else if(gameState == GameState.Pause)
            {
                UnPauseGame();
            }
        }
        //Проверяем что игрок дошел до конца и если это произошло, то заканциваем игру победой
        if (player != null && gameState == GameState.Game)
        {
            for (int i = 0; i < winPositions.Length; i++)
            {
                var pos = ((Vector2)winPositions[i].position).Round();
                if (pos == player.CurrentPos)
                {
                    WinGame();
                }
            }
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        ScanGrid();
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity).GetComponent<Character>();
        //Да я знаю что этой штукой лучше не пользоваться, но в данном случае так будет быстрее чем прописывать
        //Ссылку на transform в какой-то компонент
        camera.Follow = player.transform.Find("CameraTarget");

        pauseMenu.gameObject.SetActive(false);
        deathMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        gameState = GameState.Game;
    }

    public void PauseGame()
    {
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
        gameState = GameState.Pause;
    }

    public void UnPauseGame()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameState = GameState.Game;
    }

    protected void EndGame()
    {
        player = null;
        //Сообщаем всем кому надо, что игра окончена и пора бы самоуничтожиться
        Messenger.Broadcast("EndGame");
        characters.Clear();
        //Очищаем сетку
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = CellState.Empty;
            }
        }
    }

    public void ResetGame()
    {
        EndGame();
        StartGame();
    }

    public void ExitToMainMenu()
    {
        EndGame();
        pauseMenu.gameObject.SetActive(false);
        deathMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        camera.transform.position = cameraMainMenuPos;
        gameState = GameState.MainMenu;
    }

    protected void WinGame()
    {
        Time.timeScale = 0;
        winMenu.gameObject.SetActive(true);
        gameState = GameState.Death;
    }

    protected void LoseGame()
    {
        Time.timeScale = 0;
        deathMenu.gameObject.SetActive(true);
        gameState = GameState.Win;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    

    public CellState GetCellType(Vector2Int position)
    {
        if (position.x < 0 || position.x >= grid.GetLength(0) || position.y < 0 || position.y >= grid.GetLength(1))
        {
            return CellState.Obstacle;
        }

        return grid[position.x, position.y];
    }
    //Помечаем что в клетке находится бомба, чтобы в нее нельзя было поставить еще одну
    public void SetBomb(Vector2Int position, bool set = true)
    {
        if (set)
        {
            grid[position.x, position.y] = CellState.Bomb;
        }
        else
        {
            grid[position.x, position.y] = CellState.Empty;
        }
    }
    //функция сообщает мастеру что заданная точка была под атакой и надо убить всех персонажей в ней
    //Либо только игрока если это атака противника
    public void HitPoint(Vector2Int position, bool affectEnemies)
    {
        if (characters.TryGetValue(position, out List<Character> list))
        {
            var removeList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (affectEnemies || list[i] == player)
                {
                    list[i].Kill();
                    removeList.Add(i);
                }
            }

            /*for (int i = removeList.Count - 1; i >= 0; i--)
            {
                Debug.Log($"i = {i} + rem = {removeList[i]}");
                list.RemoveAt(removeList[i]);
            }*/
        }
    }
    
    public void RegisterCharacter(Character character)
    {
        var pos = ((Vector2)character.transform.position).Round();
        AddCharacterToPosition(pos, character);
    }

    public void MoveCharacter(Vector2Int from, Vector2Int to, Character character)
    {
        characters[from].Remove(character);
        AddCharacterToPosition(to,character);
    }

    public void UnregisterCharacter(Vector2Int position, Character character)
    {
        //Если убитый персонаж - игрок, то заканчиваем игру поражением
        if (gameState == GameState.Game && character == player)
        {
            LoseGame();
        }

        characters[position].Remove(character);
    }
    
    protected void AddCharacterToPosition(Vector2Int position, Character character)
    {
        if (characters.TryGetValue(position, out List<Character> chars))
        {
            chars.Add(character);
        }
        else
        {
            var list = new List<Character>();
            list.Add(character);
            characters[position] = list;
        }
    }
    //Сканируем стены и записываем в массив данные о том какие клетки свободны, а какие нет
    protected void ScanGrid()
    {
        for (int i = 0; i < wallParent.childCount; i++)
        {
            var pos = new Vector2Int(Mathf.RoundToInt(wallParent.GetChild(i).position.x),
                Mathf.RoundToInt(wallParent.GetChild(i).position.y));

            if (pos.x >= 0 && pos.x < grid.GetLength(0) && pos.y >= 0 && pos.y < grid.GetLength(1))
            {
                grid[pos.x, pos.y] = CellState.Obstacle;
            }
        }
    }
    //Отображаем свободные и занятые клетки
    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var color = (grid[i, j] == CellState.Obstacle) ? Color.red : Color.green;
                    Gizmos.color = color;
                    Gizmos.DrawCube(new Vector3(i, j, 0), Vector3.one);
                }
            }
        }
    }
}

public enum CellState
{
    Empty, Obstacle, Bomb
}

public enum GameState
{
    MainMenu, Game, Pause, Death, Win
}

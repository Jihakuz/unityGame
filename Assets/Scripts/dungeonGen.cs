using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class dungeonGen : MonoBehaviour
{
    public int columns = 10;
    public int rows = 10;
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;
    public GameObject[] innerWallTiles;
    public GameObject player;
    public System.Random r = new System.Random();
    private List<Vector3> gridPostions;
    public Dictionary<Vector3, GameObject> room;
    private GameObject iworld;
    public GameObject world_canvas;

    private void initPos(int rows, int columns)
    {
        for (int x = -1; x < rows - 1; x++)
        {
            for (int y = -1; y < columns - 1; y++)
            {
                gridPostions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private Dictionary<Vector3, GameObject> genDungeon()
    {
        Dictionary<Vector3, GameObject> room = new Dictionary<Vector3, GameObject>();

        for (int x = -1; x < rows + 1; x++)
        {
            for (int y = -1; y < columns + 1; y++)
            {
                GameObject toInstantiate;
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = wallTiles[Random.Range(0, (wallTiles.Length - 1))];
                }
                    
                else
                {
                    int rnd = r.Next(1, 10);
                    if (rnd > 0 && rnd < 2)
                    {
                        toInstantiate = innerWallTiles[Random.Range(0, innerWallTiles.Length)];
                    }
                        
                    else
                    {
                        toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    }  
                }

                room.Add(new Vector3(x, y, 0f), toInstantiate);
                
            }
        }

        return room;

    }

    void instantiate_dungeon(Dictionary<Vector3, GameObject> tiles)
    {
        bool flag = false;
        int counter = 0;
        iworld = Instantiate(new GameObject(), new Vector2(0,0), Quaternion.identity);
        Vector2[] d_xy = new Vector2[] {new Vector3(-1, 4), new Vector3(-1, 5), new Vector3(4, -1), new Vector3(5, -1), new Vector3(4, 10), new Vector3(5, 10), new Vector3(10, 4), new Vector3(10, 5)};

        foreach(KeyValuePair<Vector3, GameObject> tile in tiles)
        {
            Vector3 tilexy = tile.Key;
            foreach(Vector3 xy in d_xy)
            {
                if (xy == tilexy)
                {
                    flag = true;
                    GameObject door = floorTiles[Random.Range(0, (floorTiles.Length -1))];
                    GameObject doorObj = Instantiate(door, xy, Quaternion.identity) as GameObject;
                    // doorObj.AddComponent<doorScript>();
                    doorObj.transform.SetParent(GameObject.Find("New Game Object(Clone)").transform);
                    doorObj.name = $"Door {counter}";

                    counter ++;
                    break;
                }
            }
            if (!flag)
            {
                GameObject instance = Instantiate(tile.Value, tile.Key, Quaternion.identity) as GameObject;
                instance.transform.SetParent(iworld.transform);
            }
            flag = false;
        }
    }

    void instantiate_player(float x, float y){
        GameObject playerInstance = Instantiate(player, new Vector3(x, y, -0.2f), Quaternion.identity);
        playerInstance.transform.SetParent(iworld.transform);
    }
    void dungeonCleanUp()
    {   
        Destroy(GameObject.Find("New Game Object"));
        
        GameObject floor = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], new Vector3(0f,0f,0f), Quaternion.identity);
        floor.transform.SetParent(iworld.transform);
    }   

    void instantiate_buttons()
    {
        Instantiate(GameObject.Find("GameManager").GetComponent<gameManager>().eventSystem);
        GameObject UI = Instantiate(world_canvas);
        UI.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"Lives: {GameObject.Find("GameManager").GetComponent<gameManager>().lives}";
        GameObject.Find("World_Canvas(Clone)").transform.GetChild(2).gameObject.GetComponentInChildren<Text>().text = $"Difficulty: {GameObject.Find("GameManager").GetComponent<gameManager>().difficulty}";
    }
    
    public void generate_dungeon(Dictionary<Vector3, GameObject> room, float x, float y)
    {
        instantiate_dungeon(room);
        dungeonCleanUp();
        instantiate_player(x, y);
        instantiate_buttons();
    }

    public void new_dungeon(float x, float y)
    {
        Vector2 newxy = new Vector2(0f,0f);
        GameObject parentObject = GameObject.Find("New Game Object(Clone)").gameObject;
        int childs = parentObject.transform.GetChildCount();
        for (int i = 0; i < childs; i++)
        {
            Destroy(parentObject.transform.GetChild(i).gameObject);
        }
        Destroy(parentObject);

        int pos = -1;

        Vector2[] doors = new Vector2[] {new Vector3(-1, 4), new Vector3(-1, 5), new Vector3(4, -1), new Vector3(5, -1), new Vector3(4, 10), new Vector3(5, 10), new Vector3(10, 4), new Vector3(10, 5)};
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == new Vector2(x, y))
            {
                pos = i;
                break;
            }
        }

        switch(pos)
        {
            case 0:
                newxy = new Vector2(9, 4);
                break;
            case 1:
                newxy = new Vector2(9, 5);
                break;
            case 2:
                newxy = new Vector2(4, 9);
                break;
            case 3:
                newxy = new Vector2(5, 9);
                break;
            case 4:
                newxy = new Vector2(4, 0);
                break;
            case 5:
                newxy = new Vector2(5, 0);
                break;
            case 6:
                newxy = new Vector2(0, 4);
                break;
            case 7:
                newxy = new Vector2(0, 5);
                break;
        }

        room = genDungeon();
        generate_dungeon(room, newxy.x, newxy.y);
    }

    void Start()
    {
        
        gridPostions = new List<Vector3>();
        initPos(rows, columns);
        room = genDungeon();
        generate_dungeon(room, 0f, 0f);

        GameObject.Find("GameManager").GetComponent<gameManager>().change_play();
    }

}
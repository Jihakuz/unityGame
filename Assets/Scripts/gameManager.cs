using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class gameManager : MonoBehaviour
{
    public int lives = 3;
    public bool play = false;
    public GameObject canvas;
    public GameObject eventSystem;
    public string scene = "World";
    private Dictionary<Vector3, GameObject> current_dugeon;
    private float[] PlayerXY;
    public GameObject enemy;
    public GameObject playerf;
    public GameObject fightUI;  
    public Dictionary<string, int> player_stats;
    public string difficulty;
    public float damage_mod;
    public int points;
    
    public void change_play()
    {
        switch(play)
        {
            case false:
                play = true;
                break;
            case true:
                play = false;
                break;
        }
    }

    private float[] get_xy()
    {
        float[] xy = new float[2];
        xy[0] = GameObject.Find("Player(Clone)").GetComponent<PlayerController>().x;
        xy[1] = GameObject.Find("Player(Clone)").GetComponent<PlayerController>().y;
        return xy;
    }

    public void scene_to_fight()
    {
        Debug.Log("Changing to fight scene.");
        change_play();
        scene="Fight";
        current_dugeon = GameObject.Find("GameManager").GetComponent<dungeonGen>().room;
        PlayerXY = get_xy();

        Destroy(GameObject.Find("New Game Object(Clone)"));
        Destroy(GameObject.Find("World_Canvas(Clone)"));
        Destroy(GameObject.Find("EventSystem(Clone)"));

        Instantiate(enemy);
        Instantiate(playerf);
        Instantiate(eventSystem);
        GameObject UI = Instantiate(fightUI);

        GameObject.Find("GameManager").GetComponent<fightManager>().first();
        GameObject.Find("GameManager").GetComponent<fightManager>().Pstats = player_stats;
        GameObject.Find("GameManager").GetComponent<fightManager>().turn = true;
        GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(false);
    
    }

    public void scene_to_ui(){
        Debug.Log("Changing to level scene.");
        change_play();
        scene = "UI";
        current_dugeon = GameObject.Find("GameManager").GetComponent<dungeonGen>().room;
        PlayerXY = get_xy();

        Destroy(GameObject.Find("New Game Object(Clone)"));
        Destroy(GameObject.Find("World_Canvas(Clone)"));
        Destroy(GameObject.Find("EventSystem(Clone)"));


        Instantiate(canvas, new Vector3(0f,0f,0f), Quaternion.identity) ;
        Instantiate(eventSystem, new Vector3(0f, 0f, 0f), Quaternion.identity);

        GameObject.Find("EventSystem(Clone)").GetComponent<uiGen>().start_ui(canvas, eventSystem, player_stats);

    }

    public void scene_to_world(bool from_fight)
    {
        Debug.Log("Changing to world scene.");
        scene = "World";
        
        Destroy(GameObject.Find("EventSystem(Clone)"));
        Destroy(GameObject.Find("Canvas(Clone)"));
        if (from_fight)
        {
            Destroy(GameObject.Find("EventSystem(Clone)"));
            Destroy(GameObject.Find("fightCanvas(Clone)"));
            Destroy(GameObject.Find("01(Clone)"));
            Destroy(GameObject.Find("Player(Clone)"));
        }

        GameObject.Find("GameManager").GetComponent<dungeonGen>().generate_dungeon(current_dugeon, PlayerXY[0], PlayerXY[1]);
        change_play();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E))
        {
            switch(scene)
            {
                case "World":
                    scene_to_ui();
                    break;
                case "UI":
                    scene_to_world(false);
                    break;
            }
        }
    }

    private void Start() {
        difficulty = "Normal";
        player_stats = new Dictionary<string, int>();
        player_stats.Add("Health", Random.Range(200, 300));
        player_stats.Add("Attack", Random.Range(1, 10));
        player_stats.Add("Defence", Random.Range(1, 10));
        player_stats.Add("Speed", Random.Range(1, 10));
        player_stats.Add("Points", 5);        
    }
}

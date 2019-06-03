using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
public class fightManager : MonoBehaviour
{
    public Dictionary<string, int> Pstats;
    public Dictionary<string, int> Estats;
    public Dictionary<string, List<float>> moves_list;
    private List<string> move_names;
    public GameObject moveCanvas;
    private GameObject UI;
    private string effect;
    public bool ended;
    public bool turn;

    void Start()
    {
        Estats = new Dictionary<string, int>();

        moves_list = new Dictionary<string, List<float>>();
        moves_list.Add("Punch", new List<float>{20, 8, 20});
        moves_list.Add("Kick", new List<float>{40, 8, 10});
        moves_list.Add("Protect", new List<float>{0, 10, 5});
        moves_list.Add("Rest", new List<float>{0, 10, 5});

        move_names = new List<string>();

        foreach(KeyValuePair<string, List<float>> kvp in moves_list)
            move_names.Add(kvp.Key);
    }

    private void Update() {
        // Debug.Log(DateTime.UtcNow);
    }

    public void first()
    {
        Estats["Health"] = Random.Range(200, 300);
        Estats["Attack"] = Random.Range(1, 10);
        Estats["Defence"] = Random.Range(1, 10);
        Estats["Speed"] = Random.Range(1, 10);
        Estats["Points"] = 5;

        updateHealth();
    }

    private string doMove(string move_name)
    {
        float damage = 0;

        List<float> move_stats = GameObject.Find("GameManager").GetComponent<fightManager>().moves_list[move_name];

        if (Random.Range(0, 10) <= (move_stats[1]))
        {
            if ((move_name == "Punch" || move_name == "Kick"))
            {
                if (effect == "Protect")
                {
                    effect = "";
                    switch (GameObject.Find("GameManager").GetComponent<fightManager>().turn)
                    {
                        case false:
                            return $"Player protected itself.";
                        
                        case true:
                            return $"Enemy protected itself.";
                    }
                }
                else
                {
                    switch (GameObject.Find("GameManager").GetComponent<fightManager>().turn)
                    {
                        case false:
                            GameObject.Find("GameManager").GetComponent<fightManager>().Pstats["Health"] -= (int) (move_stats[0] * GameObject.Find("GameManager").GetComponent<gameManager>().damage_mod) + GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Attack"] - GameObject.Find("GameManager").GetComponent<fightManager>().Pstats["Defence"];;
                            return $"Enemy used: {move_name}";
                        
                        case true:
                            GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Health"] -= (int) (move_stats[0] * GameObject.Find("GameManager").GetComponent<gameManager>().damage_mod) + GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Attack"] - GameObject.Find("GameManager").GetComponent<fightManager>().Pstats["Defence"];;
                            return $"Player used: {move_name}";
                    }
                }
            }

            else if (move_name == "Protect")
            {
                effect = "Protect";
                switch (GameObject.Find("GameManager").GetComponent<fightManager>().turn)
                {
                    case false:
                        return $"Enemy used: {move_name}";
                    
                    case true:
                        return $"Player used: {move_name}";
                }
            }

            else if (move_name == "Rest")
            {
                switch (GameObject.Find("GameManager").GetComponent<fightManager>().turn)
                {
                    case true:
                        GameObject.Find("GameManager").GetComponent<fightManager>().Pstats["Health"] += 10;
                        return $"Player used: {move_name}";
                    
                    case false:
                    GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Health"] += 10;
                        return $"Enemy used: {move_name}";
                }
            }
        }
        else
        {
            switch (GameObject.Find("GameManager").GetComponent<fightManager>().turn)
                {
                    case true:
                        return $"Player missed";
                    
                    case false:
                        return $"Enemy missed";
                }
        }

        return "";
    }

    private void updateHealth() 
    {
        UI = GameObject.Find("fightCanvas(Clone)");
        for(int i = 0; i < UI.transform.GetChildCount(); i++)
        {
            if (UI.transform.GetChild(i).gameObject.name == "fHealth")
                UI.transform.GetChild(i).gameObject.GetComponent<Text>().text = $"Player: {GameObject.Find("GameManager").GetComponent<gameManager>().player_stats["Health"]}";
            else if (UI.transform.GetChild(i).gameObject.name == "eHealth")
                UI.transform.GetChild(i).gameObject.GetComponent<Text>().text = $"Enemy: {GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Health"]}";
        }
    }

    private void enemyTurn()
    {
        GameObject.Find("GameManager").GetComponent<fightManager>().turn = false;
        string move = GameObject.Find("GameManager").GetComponent<fightManager>().move_names[Random.Range(1,2)];
        
        GameObject.Find("fightCanvas(Clone)").transform.GetChild(5).gameObject.GetComponent<Text>().text = doMove(move);

        updateHealth();

        if (GameObject.Find("GameManager").GetComponent<fightManager>().Pstats["Health"] <= 0)
        {
            UI.transform.GetChild(3).gameObject.GetComponent<Text>().text = "Player: 0";
            endGame(false);
        }
        else
        {
            updateHealth();

            GameObject.Find("GameManager").GetComponent<fightManager>().turn = true;
            GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(true);
        }
    }

    private void endGame(bool win)
    {
        GameObject.Find("GameManager").GetComponent<fightManager>().ended = true;
        GameObject.Find("fightCanvas(Clone)").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("fightCanvas(Clone)").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("fightCanvas(Clone)").transform.GetChild(2).gameObject.SetActive(false);

        switch(win)
        {
            case true:
                GameObject.Find("fightCanvas(Clone)").transform.GetChild(5).gameObject.GetComponent<Text>().text = "You win!";
                GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(true);
                GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.transform.GetChild(0).GetComponent<Text>().text = "End fight";
                break;
            case false:
                if (!checkDead())
                {
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(5).gameObject.GetComponent<Text>().text = "You lose!";
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(true);
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.transform.GetChild(0).GetComponent<Text>().text = "End fight";
                }
                break;
        }

        // GameObject.Find("GameManager").GetComponent<fightManager>().Stopwatch.Stop();
        // GameObject.Find("GameManager").GetComponent<gameManager>().Points += (1 / GameObject.Find("GameManager").GetComponent<fightManager>().Stopwatch.ElapsedMilliseconds) * 1000;
        // Debug.Log((1 / GameObject.Find("GameManager").GetComponent<fightManager>().Stopwatch.ElapsedMilliseconds) * 1000);
    }
    
    private bool checkDead()
    {
        if ((GameObject.Find("GameManager").GetComponent<gameManager>().lives-1) == 0)
        {
            Destroy(GameObject.Find("01(Clone)"));
            Destroy(GameObject.Find("Player(Clone)"));

            for(int i = 0; i < 6; i ++)
            {
                if (i != 6)
                {
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(i).GetComponentInChildren<Text>().text = "You have died.";
                }
            }

            return true;
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<gameManager>().lives--;
            return false;
        }
    }
    public void nextTurnClick()
    {
        if (GameObject.Find("GameManager").GetComponent<fightManager>().ended)
            GameObject.Find("GameManager").GetComponent<gameManager>().scene_to_world(true);
        else
        {
            GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(false);
            switch(GameObject.Find("GameManager").GetComponent<fightManager>().turn)
            {
                case true:
                    GameObject.Find("GameManager").GetComponent<fightManager>().turn = false;
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(0).gameObject.SetActive(true);
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(1).gameObject.SetActive(true);
                    GameObject.Find("fightCanvas(Clone)").transform.GetChild(2).gameObject.SetActive(true);
                    break;

                case false:
                    GameObject.Find("GameManager").GetComponent<fightManager>().turn = true;
                    enemyTurn();
                    break;
            }
        }
    }

    public void onClick(GameObject go)
    {
        switch (go.name)
        {
            case "Fight":
                fight_click();
                break;
            
            case "Run":
                run_click();
                break;

            case "Bag":
                break;
        }
    }
    
    public void move_click(GameObject go)
    {
        Destroy(GameObject.Find("moveCanvas(Clone)"));

        if (GameObject.Find("GameManager").GetComponent<fightManager>().Estats["Health"] <= 0)
        {
            UI.transform.GetChild(4).gameObject.GetComponent<Text>().text = "Enemy: 0";
            endGame(true);
        }

        else
        {
            Instantiate(GameObject.Find("GameManager").GetComponent<gameManager>().fightUI);
            Instantiate(GameObject.Find("GameManager").GetComponent<gameManager>().playerf);
            Instantiate(GameObject.Find("GameManager").GetComponent<gameManager>().enemy);

            GameObject.Find("fightCanvas(Clone)").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("fightCanvas(Clone)").transform.GetChild(1).gameObject.SetActive(false);
            GameObject.Find("fightCanvas(Clone)").transform.GetChild(2).gameObject.SetActive(false);

            GameObject.Find("fightCanvas(Clone)").transform.GetChild(5).gameObject.GetComponent<Text>().text = doMove(go.GetComponent<Text>().text);
            updateHealth();

            GameObject.Find("GameManager").GetComponent<fightManager>().turn = false;
            GameObject.Find("fightCanvas(Clone)").transform.GetChild(6).gameObject.SetActive(true);
        }
    }

    private void fight_click()
    {   
        Debug.Log("fight click");
        string[] to_destroy = new string[] {"fightCanvas(Clone)", "01(Clone)", "Player(Clone)"};
        foreach (string item in to_destroy)
            Destroy(GameObject.Find(item));
        moveCanvas = GameObject.Find("GameManager").GetComponent<fightManager>().moveCanvas;
        Instantiate(moveCanvas);
    }
    private void run_click()
    {
        int rnd = Random.Range(0, 20);
        if (rnd < 15)
            Destroy(GameObject.Find("fightCanvas(Clone)"));
            GameObject.Find("GameManager").GetComponent<gameManager>().scene_to_world(true);
    }
}

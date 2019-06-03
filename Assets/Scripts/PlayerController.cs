using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rg2d;
    private BoxCollider2D boxCollider;
    public float x;
    public float y;
    private Dictionary<KeyCode, Vector2> controls = new Dictionary<KeyCode, Vector2>();
    private bool first = true;

    private void Start()
    {
        rg2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        x = rg2d.transform.position.x;
        y = rg2d.transform.position.y;

        controls.Add(KeyCode.W, new Vector2(0, 1));
        controls.Add(KeyCode.S, new Vector2(0, -1));
        controls.Add(KeyCode.D, new Vector2(1, 0));
        controls.Add(KeyCode.A, new Vector2(-1, 0));
        controls.Add(KeyCode.E, new Vector2(0,0));
    }
    
    private void Update()
    {
        if (get_gm().play)
        {
            move();
        }
    }

    private gameManager get_gm()
    {
        return GameObject.Find("GameManager").GetComponent<gameManager>();
    }

    private void move()
    {
        foreach (KeyValuePair<KeyCode, Vector2> kvp in controls)
        {
            if ((Input.GetKeyDown(kvp.Key)) && checkEmpty(kvp))
            {
                if (first)
                {
                    Destroy(GameObject.Find("World_Canvas(Clone)").transform.GetChild(2).gameObject);
                    first = false;
                    switch (GameObject.Find("GameManager").GetComponent<gameManager>().difficulty)
                    {
                        case "Easy":
                            GameObject.Find("GameManager").GetComponent<gameManager>().damage_mod = 0.5f;
                            break;
                        case "Normal":
                            GameObject.Find("GameManager").GetComponent<gameManager>().damage_mod = 1;
                            break;
                        case "Hard":
                            GameObject.Find("GameManager").GetComponent<gameManager>().damage_mod = 2;
                            break;
                    }
                }

                x += kvp.Value.x;
                y += kvp.Value.y;
                
                Vector2 end = kvp.Value + rg2d.position;
                rg2d.MovePosition(end);
                if (checkDoors())
                {
                    GameObject.Find("GameManager").GetComponent<dungeonGen>().new_dungeon(x, y);
                    break;
                }
                isFight();
            }
        }
    }

    private void isFight()
    {
        int rnd = Random.Range(0, 10);
        if (rnd >= 5)
        {
            get_gm().scene_to_fight();
            Debug.Log("Fight.");
        }   
        else
        {
            Debug.Log("No Fight.");
        }
    }
    
    private Boolean checkDoors()
    {
        bool check = false;
        Vector2[] doors = new Vector2[] {new Vector3(-1, 4), new Vector3(-1, 5), new Vector3(4, -1), new Vector3(5, -1), new Vector3(4, 10), new Vector3(5, 10), new Vector3(10, 4), new Vector3(10, 5)};
        foreach (Vector2 xy in doors)
        {
            if (xy == new Vector2(x, y)) check = true;
        }
        
        return check;
    }

    private Boolean checkEmpty(KeyValuePair<KeyCode, Vector2> key)
    {
        Vector2 start = rg2d.position;
        Vector2 end = start + key.Value;
        boxCollider.enabled = false;

        var hit = Physics2D.Linecast(start, end);
        boxCollider.enabled = true;

        if (hit.transform == null)
            return true;
        else
            return false;
    }

    public void changeDifficulty()
    {
        string current_diff = GameObject.Find("GameManager").GetComponent<gameManager>().difficulty;
        string new_diff = "";
        switch (current_diff)
        {
            case "Easy":
                new_diff = "Normal";
                break;

            case "Normal":
                new_diff= "Hard";
                break;

            case "Hard":
                new_diff = "Easy";
                break;
        }

        GameObject.Find("World_Canvas(Clone)").transform.GetChild(2).gameObject.GetComponentInChildren<Text>().text = $"Difficulty: {new_diff}";
        GameObject.Find("GameManager").GetComponent<gameManager>().difficulty = new_diff;
    }
}

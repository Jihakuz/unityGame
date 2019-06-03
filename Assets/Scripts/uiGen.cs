using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class uiGen : MonoBehaviour
{
    private GameObject _c;
    public Dictionary<string, int> _ps;
    public int start_points;
    public void start_ui(GameObject canvas, GameObject eventSystem, Dictionary<string, int> player_stats) 
    {
        _ps = player_stats;

        _c = GameObject.Find("Canvas(Clone)");

        start_points = GameObject.Find("GameManager").GetComponent<gameManager>().player_stats["Points"];

        update_text();

    }

    public void update_text()
    {   
        foreach(Transform t in _c.transform)
        {
            foreach (KeyValuePair<string, int> kvp in _ps)
            {                
                if (t.name == kvp.Key)
                {
                    Text text = t.GetComponent<Text>();
                    text.fontSize = 25;
                    text.text = $"{kvp.Key}: {kvp.Value}";
                }
            }
        }
    }
    
}

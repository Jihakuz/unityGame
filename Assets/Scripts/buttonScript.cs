using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour
{
    private Dictionary<string, string> int_stats;
    private Dictionary<string, int> _ps;
    private int startPoints;

    private void Start() {
        int_stats = new Dictionary<string, string>();
        int_stats.Add("_1", "Health");
        int_stats.Add("_2", "Attack");
        int_stats.Add("_3", "Defence");
        int_stats.Add("_4", "Speed");
    }    

    private string get_name(string bn)
    {
        char[] chars = bn.ToCharArray();
        List<char> nameL =  new List<char>();

        foreach(char c in chars)
        {
            if(c.ToString() == "_")
                break;
            else
                nameL.Add(c);
        }
        
        return new string(nameL.ToArray());
    }

    public void change_stats(GameObject button)
    {
        _ps = GameObject.Find("EventSystem(Clone)").GetComponent<uiGen>()._ps;

        string buttonName = button.name;
        string statName = get_name(buttonName);
        string type = buttonName.Substring((statName.Length + 1)).Trim();
    
        switch(type)
        {
            case "Plus":
                if (_ps["Points"]>0)
                {    
                    _ps["Points"]--;
                    _ps[statName]++;
                    GameObject.Find("EventSystem(Clone)").GetComponent<uiGen>().update_text();
                }
                break;
              
            case "Minus":
                if((_ps["Points"]+1)<=GameObject.Find("EventSystem(Clone)").GetComponent<uiGen>().start_points)
                {
                    _ps["Points"]++;
                    _ps[statName]--;
                    GameObject.Find("EventSystem(Clone)").GetComponent<uiGen>().update_text();
                }
                break;
        }
    }
}

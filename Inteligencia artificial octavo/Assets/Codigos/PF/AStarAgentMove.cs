using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgentMove : MonoBehaviour
{
    public static List<AStarAgentMove> selectableobject = new List<AStarAgentMove>();
    private bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        selectableobject.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && selected)
        {
            

        }
    }

    void OnMouseDown()
    {
        selected = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        foreach (AStarAgentMove obj in selectableobject)
        {
            if(obj != this)
            {
                obj.selected = false;
                obj.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

        }
        foreach (AStarAgentMove objselected in selectableobject)
        {
            if (objselected == this)
            {
                selected = false;
                Debug.Log("Desactivado");

            }
        }
    }
}

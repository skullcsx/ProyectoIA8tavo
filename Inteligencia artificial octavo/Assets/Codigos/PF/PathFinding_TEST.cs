using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding_TEST : MonoBehaviour
{

    void Start()
    {
        //ClassGrid myTest = new ClassGrid(5, 5);
        //myTest.DepthFirstSearch(0, 0, 4, 4);
        ClassGridTarea1BFS myTest = new ClassGridTarea1BFS(5, 5);
        myTest.BreadthFirstSearch(0, 0, 4, 4);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

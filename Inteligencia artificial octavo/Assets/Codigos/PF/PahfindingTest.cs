using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// [ExecuteInEditMode]
public class PahfindingTest : MonoBehaviour
{
    public MeshFilter BlackSphere = null;

    // private PFGrid<TileNode> myGrid;
    // private Pathfinder myPathfinder;


    public int iWidth = 5;
    public int iHeight = 5;
    public float fTileSize = 10.0f;

    public ClassGrid myGrid;

    private void Awake()
    {
        myGrid = new ClassGrid(iWidth, iHeight, fTileSize);

        // Le deja la misma posición z que en la que inició, para que se pueda ajustar desde el editor.
        Camera.main.transform.position = new Vector3(iWidth * 0.5f * fTileSize,
                                                    iHeight * 0.5f * fTileSize,
                                                    Camera.main.transform.position.z);

    }

    // Start is called before the first frame update
    void Start()
    {
        // this.runInEditMode= true;




        // myTest.DepthFirstSearch(0, 0, 4, 4);
        // myTest.BreadthFirstSearch(2, 2, 1, 1);
        // myTest.BestFirstSearch(0, 0, 2, 0);
        // myTest.DjikstraSearch(0, 0, 4, 4);
        // List<Node> Pathfinding_result = myTest.AStarSearch(StartPosition.x, StartPosition.y, EndPosition.x, EndPosition.y);  // Mío, corregido



        // Asignar ruta a seguir al agente de pathfinding.
        // myAgent.


        //[0, 1]
        //[0, 0] [1, 0] [2, 0] [3, 0] [4, 0]






        // myGrid = new PFGrid<TileNode>(20, 10, 10f, Vector3.zero,
        //     (PFGrid<TileNode> g, int x, int y) => new TileNode(g, x, y));

        // Estos los usamos para probar que efectivamente termina cuando no hay camino.
        //myGrid.GetTile(1, 0).SetIsWalkable(false);
        //myGrid.GetTile(1, 1).SetIsWalkable(false);
        //myGrid.GetTile(0, 1).SetIsWalkable(false);

        // myPathfinder = new Pathfinder(myGrid);
        //RouteFound = myPathfinder.DepthFirstSearch(0, 0, 9, 9);
        //if (RouteFound != null)
        //{
        //    // Entonces le pedimos las coordenadas de mundo de la ruta
        //    RouteCoords = myPathfinder.GetRouteInWorldCoordinates(RouteFound);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
        }



    }

    public void OnDrawGizmos()
    {
        //if (RouteCoords != null && Application.isPlaying)
        //{
        //    float fTileSize = myGrid.GetTileSize() / 4.0f;
        //    Gizmos.color = Color.white;
        //    Color currentColor = Color.black;
        //    Color ColorIncrease = new Color(1.0f / RouteCoords.Count, 1.0f / RouteCoords.Count, 1.0f / RouteCoords.Count, 1);
        //    // dibujamos gizmos en los tiles de la ruta encontrada
        //    foreach (Vector3 tilePosition in RouteCoords)
        //    {
        //        Gizmos.color = currentColor + ColorIncrease;
        //        currentColor += ColorIncrease;
        //        // print(currentColor);
        //        Gizmos.DrawSphere(tilePosition, fTileSize);
        //    }
        //}
    }
}

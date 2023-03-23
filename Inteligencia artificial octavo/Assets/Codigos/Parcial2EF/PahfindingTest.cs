using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// [ExecuteInEditMode]
public class PahfindingTest : MonoBehaviour
{
    public MeshFilter BlackSphere = null;

    //definimos variables y gameObjects

    public int iWidth = 5;
    public int iHeight = 5;
    public float fTileSize = 10.0f;

    public ClassGrid myGrid;

    public GameObject ST_Node;
    public GameObject EN_Node;

    public bool Inicio = false;
    public bool Final = false;

    public bool Comienzo = false;

    public bool CaminoL = false;

    public List<Node> PathFinding_test;


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
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
        }

        //hacemos una comprobacion para saber si ya tenemos los nodos inicio y final
        if(Inicio == true && Final == true)
        {
            Comienzo = true;
        }

        //comprobamos el resultado anterior y agregamos la tecla spacio para poder lanzar al agente
        if(Comienzo == true && Input.GetKeyDown("space"))
        {
            //igualamos los componentes de inicio y final que obtuvimos en el codigo check
            check Check_PInicio = ST_Node.GetComponent<check>();
            check Check_PFinal = EN_Node.GetComponent<check>();

            //utilizamos las coordenadas que seteamos en el codigo check y ClassGrid
            PathFinding_test = myGrid.AStarSearch(Check_PInicio.CordX, Check_PInicio.CordY, Check_PFinal.CordX, Check_PFinal.CordY);
            //creamos una nueva lista
            List<Vector3> wPositionPF = new List<Vector3>();
            
            //avanzamos en la lista
            foreach (Node n in PathFinding_test)
            {
                //agregamos la world position
                wPositionPF.Add(myGrid.GetWorldPosition(n.x, n.y));
            }

            //pasamos a true la booleana de Camino Listo
            CaminoL = true;

        }



    }

    public void OnDrawGizmos()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class check : MonoBehaviour
{
    //hacemos una referencia al PahfindingTest para poder utilizarlo en este codigo
    public PahfindingTest pathfindingTest;

    //definimos variables
    public bool ch_inicio, ch_final;

    public int CordX, CordY;

    // Start is called before the first frame update
    void Start()
    {
        //hacemos una asignacion al pathfindingTest utilizando el tag de Grid
        pathfindingTest = GameObject.FindGameObjectWithTag("Grid").GetComponent<PahfindingTest>();
    }

    //funcion que detecta si el mouse esta arriba del objeto
    private void OnMouseOver()
    {
        //comprobamos si tenemos un nodo final con una booleana, si es falso
        if (pathfindingTest.Inicio == false)
        {
            Debug.Log("Aun no hay un Nodo de Inicio");

            // hacemos una comprobacion de si hacemos click con el boton derecho del mouse y comprobamos el tag para evitar usar el AStarAgent como nodo
            if (Input.GetMouseButtonDown(0) && gameObject.CompareTag("Grid"))
            {
                //hacemos una comprobacion de si ya se utilizo este nodo como final, si si, se sustituye para que ahora sea el nodo inicio

                if (ch_final == true)
                {
                    pathfindingTest.EN_Node = null;
                    pathfindingTest.Final = false;
                    ch_final = false;
                }
                //obtenemos el gameObject para utilizar ese nodo como final
                pathfindingTest.ST_Node = gameObject;
                pathfindingTest.Inicio = true;
                ch_inicio = true;
                
            }
        }
        //comprobamos si tenemos un nodo final con una booleana, si es falso
        if(pathfindingTest.Final == false)
        {
            // hacemos una comprobacion de si hacemos click con el boton derecho del mouse y comprobamos el tag para evitar usar el AStarAgent como nodo
            Debug.Log("Falta el Nodo de Fin");
            if(Input.GetMouseButtonDown(1)&& gameObject.CompareTag("Grid"))
            {
                //hacemos una comprobacion de si ya se utilizo este nodo como inicio, si si, se sustituye para que ahora sea el nodo final
                if (ch_inicio == true)
                {
                    pathfindingTest.ST_Node = null;
                    pathfindingTest.Inicio = false;
                    ch_inicio = false;
                }
                //obtenemos el gameObject para utilizar ese nodo como final
                pathfindingTest.EN_Node = gameObject;
                pathfindingTest.Final = true;
                ch_final = true;
            }
        }
        
    }

    //obtenemos las coordenadas que nos seran utiles en el codigo de ClassGrid en la seccion de CreateWorldText2
    public void getCoords(int x, int y)
    {
        CordX = x;
        CordY = y;
    }

    private void OnDrawGizmos()
    {
        
        if (ch_inicio == true)
        {
            //dibujamos la esfera de color verde para el inicio utilizando los gizmos
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1);
        }
        
        if (ch_final == true)
        {
            //dibujamos el cuadrado de color rojo para el final del camino
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, new Vector3 (1,1,1));
        }
    }
}

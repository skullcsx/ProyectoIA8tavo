using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AStarAgent : NewSteeringBehaviors
{
    public int2 StartPosition = int2.zero;
    public int2 EndPosition = int2.zero;

    // Lista donde guardaremos los puntos que nos regrese el método de A*
    public List<Vector3> Route;

    // Qué tan cerca tiene que estar el agente del punto objetivo para cambiar al siguiente punto.
    public float fDistanceThreshold;

    PahfindingTest _PathfindingReference;

    ClassGrid _GridReference;

    int iCurrentRoutePoint = 0;


    // Start is called before the first frame update
    void Start()
    {
        _PathfindingReference = GameObject.FindGameObjectWithTag("grid").GetComponent<PahfindingTest>();

        _GridReference = _PathfindingReference.myGrid;

        // Guardamos el resultado de nuestro A*
        List<Node> AStarResult = _GridReference.AStarSearch(0, 0, 4, 0);

        // Usamos esa lista de nodos para sacar las posiciones de mundo a las cuales hacer Seek o Arrive.
        Route = _GridReference.ConvertBacktrackToWorldPos(AStarResult);
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void FixedUpdate()
    {
        Vector3 v3SteeringForce = Vector3.zero;

        if (Route != null)
        {
            // Queremos saber si estamos cerca de nuestra posición objetivo.
            // Para ello, calculamos la distancia entre la posición Route[iCurrentRoutePoint] y la actual.
            float fDistanceToPoint = (Route[iCurrentRoutePoint] - transform.position).magnitude;
            Debug.Log("fDistance to Point is: " + fDistanceToPoint);

            // Si esta distancia es menor o igual a un umbral, cambiamos al siguiente punto de la lista.
            if (fDistanceToPoint < fDistanceThreshold)
            {
                iCurrentRoutePoint++;
                iCurrentRoutePoint = math.min(iCurrentRoutePoint, Route.Count - 1);
                // Hay que checar si es el último punto de la ruta a seguir.
                //if (iCurrentRoutePoint >= Route.Count)
                //{
                //    // Ya nos acabamos los puntos de esta ruta.
                //    // Podemos borrar esta ruta que terminamos.
                //    Route = null;
                //    iCurrentRoutePoint = -1;
                //    bUseArrive = false;
                //    return;
                //}
            }

            if (iCurrentRoutePoint == Route.Count - 1)
            {
                bUseArrive = true;
                v3SteeringForce = Seek(Route[iCurrentRoutePoint]);
            }
            else
            {
                // Ahora sí, hay que mover el agente hacia el punto destino.
                v3SteeringForce = Seek(Route[iCurrentRoutePoint]);
            }
        }



        // Idealmente, usaríamos el ForceMode de Force, para tomar en cuenta la masa del objeto.
        // Aquí ya no usamos el deltaTime porque viene integrado en cómo funciona AddForce.
        myRigidbody.AddForce(v3SteeringForce /* Time.deltaTime*/, ForceMode.Acceleration);

        // Hacemos un Clamp para que no exceda la velocidad máxima que puede tener el agente
        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);
    }

}

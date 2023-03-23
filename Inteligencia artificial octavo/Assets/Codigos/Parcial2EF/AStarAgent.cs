using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AStarAgent : NewSteeringBehaviors
{
    //cree la lista que nos permita ver cuantos agentes tendremos en la escena
    public static List<AStarAgent> selectableobject = new List<AStarAgent>();

    // variable booleana para saber si el agente esta seleccionado
    public bool selected = false;

    public int2 StartPosition = int2.zero;
    public int2 EndPosition = int2.zero;

    // Lista donde guardaremos los puntos que nos regrese el método de A*
    public List<Vector3> Route = null;

    // Qué tan cerca tiene que estar el agente del punto objetivo para cambiar al siguiente punto.
    public float fDistanceThreshold;

    public PahfindingTest _PathfindingReference;

    ClassGrid _GridReference;

    public bool ch_inicio = false, ch_final = false;

    public int CordX, CordY;


    public int iCurrentRoutePoint = 0;


    // Start is called before the first frame update
    void Start()
    {
        //selectableobject.Add(this);
        _PathfindingReference = GameObject.FindGameObjectWithTag("Grid").GetComponent<PahfindingTest>();


        _GridReference = _PathfindingReference.myGrid;

        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (_PathfindingReference.CaminoL == true)
        {
            Route = _GridReference.ConvertBacktrackToWorldPos(_PathfindingReference.PathFinding_test);
            _PathfindingReference.CaminoL = false;
        }

    }

    private void FixedUpdate()
    {
        Vector3 v3SteeringForce = Vector3.zero;

        if (Route != null && selected == true)
        {
            // Queremos saber si estamos cerca de nuestra posición objetivo.
            // Para ello, calculamos la distancia entre la posición Route[iCurrentRoutePoint] y la actual.
            float fDistanceToPoint = (Route[iCurrentRoutePoint] - transform.position).magnitude;
            //Debug.Log("fDistance to Point is: " + fDistanceToPoint);

            // Si esta distancia es menor o igual a un umbral, cambiamos al siguiente punto de la lista.
            if (fDistanceToPoint < fDistanceThreshold && iCurrentRoutePoint != Route.Count - 1)
            {
                iCurrentRoutePoint++;
                iCurrentRoutePoint = math.min(iCurrentRoutePoint, Route.Count - 1);

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

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //si damos click izquierdo sobre algun agente cambiaremos la booleana de seleccionado a true
            selected = true;
            //haremos que el color del objeto cambie a azul
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            //aqui buscaremos en la lista para saber siel objeto esta seleccionado
            foreach (AStarAgent obj in selectableobject)
            {
                // si el objeto seleccionado es diferente al que esta seleccionado
                if (obj != this)
                {
                    //pondremos la booleana en falso 
                    obj.selected = false;
                    //cambiaremos su color a blanco
                    obj.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }

            }
        }
        //si damos click derecho sobre algun agente cambiaremos la booleana de seleccionado a false
        if (Input.GetMouseButtonDown(1))
        {
            selected = false;
            //cambiaremos su color a blanco
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

}

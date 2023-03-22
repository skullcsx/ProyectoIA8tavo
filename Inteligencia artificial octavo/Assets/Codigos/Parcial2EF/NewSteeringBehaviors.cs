using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using static UnityEngine.RuleTile.TilingRuleOutput;

public class NewSteeringBehaviors : MonoBehaviour
{
    // Porque son redundantes
    // protected Vector2 currentPosition = Vector2.zero;  // transform.position
    // protected Vector2 currentVelocity = Vector2.zero; // rigidbody.velocity

    public Rigidbody myRigidbody = null;

    // Vector2 TargetPosition = Vector2.zero;
    public float fMaxSpeed = 1.0f;
    public float fMaxForce = 0.5f;
    // Cu�ntos fixedDeltaTime en el futuro usar� para las funciones Pursuit y Evade.
    public float fPredictionSteps = 10.0f;
    // La distancia a partir de la cual el agente comienza a desacelerar
    public float fArriveRadius = 3.0f;
    public bool bUseArrive = true;

    public enum SteeringBehavior { Seek, Flee, Pursue, Evade, Arrive }
    public SteeringBehavior currentBehavior = SteeringBehavior.Seek;

    //public SteeringBehavior CurrentBehaviorProperty
    //{
    //    get { return currentBehavior; }
    //    set 
    //    { 
    //        currentBehavior = CurrentBehaviorProperty;
    //        SetPursueTarget();
    //    }
    //}

    // simple_agent myTarget= null;
    GameObject PursuitTarget = null;  // objetivo a perseguir o evadir seg�n sea el caso
    Rigidbody PursuitTargetRB = null;   // el rigidbody de a quien estamos persiguiendo o evadiendo

    Vector3 v3TargetPosition = Vector3.zero;
    Vector3 v3SteeringForceAux = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Usamos la funci�n GetComponent para obtener el Rigidbody de este agente, y as� 
        // poder aplicarle las fuerzas resultantes de los steering behaviors.
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            // Entonces, no se encontr� el componente Rigidbody que deber�a tener este GameObject
            Debug.LogError("No Rigidbody component found for this agent's steering behavior.");
            return;
        }

    }

    private void OnValidate()
    {
        if (currentBehavior == SteeringBehavior.Pursue ||
            currentBehavior == SteeringBehavior.Evade)
        {
            // Entonces buscamos y asignamos un target rigidbody.
            SetPursueTarget();
        }
    }

    private float ArriveFunction(Vector3 in_v3DesiredDirection)
    {
        // check if it's in the radius
        float fDistance = in_v3DesiredDirection.magnitude;
        float fDesiredMagnitude = fMaxSpeed;
        if (fDistance < fArriveRadius)
        {
            // entonces, estamos dentro del radio de desaceleraci�n
            // y remplazamos la magnitud deseada por una interpolaci�n entre 0 y el radio del arrive.
            fDesiredMagnitude = Mathf.InverseLerp(0.0f, fArriveRadius, fDistance);
            // print("deaccelerating, inverse lerp is: " + fDesiredMagnitude);
        }
        return fDesiredMagnitude;
    }

    public Vector3 Seek(Vector3 in_v3TargetPosition)
    {
        // Direcci�n deseada es punta ("a d�nde quiero llegar") - cola (d�nde estoy ahorita)
        Vector3 v3DesiredDirection = in_v3TargetPosition - transform.position;
        float fDesiredMagnitude = fMaxSpeed;
        if (bUseArrive)
        {
            fDesiredMagnitude = ArriveFunction(v3DesiredDirection);
        }
        Vector3 v3DesiredVelocity = v3DesiredDirection.normalized * fDesiredMagnitude;


        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aqu�, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    Vector3 Flee(Vector3 in_v3TargetPosition)
    {
        // Direcci�n deseada es punta ("a d�nde quiero llegar") - cola (d�nde estoy ahorita)
        Vector3 v3DesiredDirection = -1.0f * (in_v3TargetPosition - transform.position);
        Vector3 v3DesiredVelocity = v3DesiredDirection.normalized * fMaxSpeed;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aqu�, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    void SetPursueTarget()
    {
        Debug.Log("entr� a setPursueTarget");
        // Ahora, buscamos un GameObject en la escena que tenga el nombre que
        // nosotros designemos, y el cual deber� tener un rigidbody para poder aplicarle
        // las funciones de Pursuit y Evade.
        PursuitTarget = GameObject.Find("PursuitTarget");
        if (PursuitTarget == null)
        {
            // Entonces no encontr� dicho objeto, es un error
            Debug.LogError("No PursuitTarget gameobject found in scene.");
            return;
        }

        PursuitTargetRB = PursuitTarget.GetComponent<Rigidbody>();
        if (PursuitTargetRB == null)
        {
            Debug.LogError("No Rigidbody present on GameObject PursuitTarget but it should.");
            return;
        }
    }

    Vector3 Pursuit(Rigidbody in_target)
    {
        // Es importante que hagamos una copia de la posici�n del objetivo para no modificarla
        // directamente.
        Vector3 v3TargetPosition = in_target.transform.position;

        // A�adimos a dicha posici�n el movimiento equivalente a
        // fPredictionSteps-veces el deltaTime. Es decir, n-cuadros en el futuro.
        v3TargetPosition += in_target.velocity * Time.fixedDeltaTime * fPredictionSteps;

        return Seek(v3TargetPosition);
    }

    Vector3 Evade(Rigidbody in_target)
    {
        // Es importante que hagamos una copia de la posici�n del objetivo para no modificarla
        // directamente.
        Vector3 v3TargetPosition = in_target.transform.position;

        // A�adimos a dicha posici�n el movimiento equivalente a
        // fPredictionSteps-veces el deltaTime. Es decir, n-cuadros en el futuro.
        v3TargetPosition += in_target.velocity * Time.fixedDeltaTime * fPredictionSteps;

        return Flee(v3TargetPosition);
    }

    void DrawPursuit(Vector3 in_v3PredictedPosition)
    {
        // Vamos a dibujar una figura en la posici�n a la que se est� haciendo el Pursuit.
        // Aqu� podemos 

    }

    Vector3 Arrive(Vector3 in_v3TargetPosition)
    {
        // check if it's in the radius
        Vector3 v3Diff = in_v3TargetPosition - transform.position;
        float fDistance = v3Diff.magnitude;
        float fDesiredMagnitude = fMaxSpeed;
        if (fDistance < fArriveRadius)
        {
            // entonces, estamos dentro del radio de desaceleraci�n
            // y remplazamos la magnitud deseada por una interpolaci�n entre 0 y el radio del arrive.
            fDesiredMagnitude = Mathf.InverseLerp(0.0f, fArriveRadius, fDistance);

            print("deaccelerating, inverse lerp is: " + fDesiredMagnitude);
        }

        // else, do not deaccelerate and just do Seek normally
        Vector3 v3DesiredVelocity = v3Diff.normalized * fDesiredMagnitude;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aqu�, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        // �sto lo us�bamos para hacer que el mouse fuera la posici�n target, 
        // pero por el momento ya no lo usaremos para Pursuit ni Evade.
        Vector3 TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TargetPosition.z = 0.0f;  // Si no hacemos esto, tendr� la z de la c�mara.

        Vector3 v3SteeringForce = Vector3.zero;
        switch (currentBehavior)
        {
            case SteeringBehavior.Seek:
                v3SteeringForce = Seek(TargetPosition);
                break;
            case SteeringBehavior.Flee:
                v3SteeringForce = Flee(TargetPosition);
                break;
            case SteeringBehavior.Pursue:
                v3SteeringForce = Pursuit(PursuitTargetRB);
                break;
            case SteeringBehavior.Evade:
                v3SteeringForce = Evade(PursuitTargetRB);
                break;
            case SteeringBehavior.Arrive:
                v3SteeringForce = Arrive(TargetPosition);
                // esto lo hice aqu� para poder accederlo en la drawgizmos
                v3TargetPosition = TargetPosition;
                break;

        }
        //currentVelocity += v3SteeringForce * Time.deltaTime;
        v3SteeringForceAux = v3SteeringForce;  // Esta es solo para poder dibujarla en el drawGizmos

        // Idealmente, usar�amos el ForceMode de Force, para tomar en cuenta la masa del objeto.
        // Aqu� ya no usamos el deltaTime porque viene integrado en c�mo funciona AddForce.
        myRigidbody.AddForce(v3SteeringForce /* Time.deltaTime*/, ForceMode.Force);

        // Hacemos un Clamp para que no exceda la velocidad m�xima que puede tener el agente
        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);
        //if (myRigidbody.velocity.magnitude > fMaxSpeed)
        //{
        //    Debug.Log("Velocity exceeded max speed");
        //}

        // Ya no es necesario llamar estas l�neas, porque el motor de f�sicas lo hace por nosotros
        // currentPosition += currentVelocity * Time.deltaTime;
        // transform.position = currentPosition;
    }

    private void OnDrawGizmos()
    {

        if (currentBehavior == SteeringBehavior.Pursue ||
            currentBehavior == SteeringBehavior.Evade)
        {
            Gizmos.color = Color.yellow;
            Vector3 nextPosition = PursuitTargetRB.transform.position +
                PursuitTargetRB.velocity * Time.fixedDeltaTime * fPredictionSteps;

            Gizmos.DrawSphere(nextPosition, 0.25f);
        }

        // Dibujamos una l�nea de la velocidad
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + myRigidbody.velocity);

        // Ahora dibujamos una l�nea de la fuerza
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + v3SteeringForceAux);

        // Dibujamos una esfera del radio en el que debe dejar de acelerar.
        if (currentBehavior == SteeringBehavior.Arrive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(v3TargetPosition, fArriveRadius);
        }
    }
}
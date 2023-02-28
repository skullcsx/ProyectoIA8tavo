using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steeringbehaviur : MonoBehaviour
{

    Vector2 currentPosition = Vector2.zero;
    Vector2 currentVelocity = Vector2.zero;

    Vector2 TargetPosition = Vector2.zero;
    public float maxSpeed = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
    }

    Vector2  Seek(Vector2 in_TargetPosition)
    {
        // Direccion deseada es punta ("a donde quiero llegar") - cola (donde estoy ahorita)
        Vector2 DesiredDirection = TargetPosition - currentPosition;
        Vector2 DesiredVelocity = DesiredDirection.normalized * maxSpeed;

        Vector2 SteeringForce = DesiredVelocity - currentVelocity;
        return SteeringForce;
    }

    Vector2 Flee (Vector2 in_TargetPosition)
    {
        return Seek(in_TargetPosition) * -1.0f;
    }
    // Update is called once per frame
    void Update()
    {
        TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 SteeringForce = Seek(TargetPosition);

        currentVelocity += SteeringForce * Time.deltaTime;

        currentPosition += currentVelocity * Time.deltaTime;

        transform.position = currentPosition;

        //c= sqrt(a^2 + b^2)
        //vector (1,2,5)
        //1*1,2*2,5*5 (magnitud del vector) = 30

        // vector (0,0,0)
        // 0*0,0*0,0*0 = 0 
        //if()

    }
}

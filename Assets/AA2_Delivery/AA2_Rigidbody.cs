using System;
using static AA2_Cloth;

[System.Serializable]
public class AA2_Rigidbody
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
    }
    public SettingsCollision settingsCollision;

    

    [System.Serializable]
    public struct CubeRigidbody
    {
        
        public Vector3C position;
        public Vector3C lastPosition;
        public Vector3C size;
        public Vector3C euler;
        public QuatC rotation;


        public CubeRigidbody(Vector3C _position, Vector3C _size, Vector3C _euler, QuatC _rotation)
        {
            lastPosition = _position;
            position = _position;
            size = _size;
            euler = _euler;
            rotation = new QuatC(1.0f, 1.0f, 0.0f, 0.0f);

        }

        public void Euler(Vector3C force, float dt)
        {
            lastPosition = position;
            euler += force * dt;
            position += euler * dt;
        }

        public void RotiationEuler(Vector3C axis, float angle)
        {
            float halfAngle = angle * 0.5f;
            float s = (float)Math.Sin(halfAngle);

            QuatC rotationQuat = new QuatC((float)Math.Cos(halfAngle), axis.x * s, axis.y * s, axis.z * s);
            rotation = QuatC.Multiply(rotationQuat, rotation);
        }


    }
    public CubeRigidbody crb = new CubeRigidbody(Vector3C.zero, new(.1f,.1f,.1f), Vector3C.zero, QuatC.zero);
    public void Update(float dt)
    {


        crb.Euler(settings.gravity,dt);
        if (crb.euler.x < 10) 
        {
            crb.euler = QuatC.RotateVector(crb.rotation, crb.euler);
        }
        //for (int i = 0; i < settingsCollision.planes.Length; i++) 
        //{
        //    CollisionPlane(crb, settingsCollision.planes[i]);
        //}

    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
    }

    

    public void CollisionPlane(CubeRigidbody cubo, PlaneC plano)
    {
        // Comprobar si la partícula está delante o detrás del plano
        float dotProduct = Vector3C.Dot(plano.normal, cubo.position - plano.position);
        UnityEngine.Debug.Log("dot product" + dotProduct);
        if (dotProduct <= 0)
        {
            // Trazar una línea/rayo desde la posición anterior a la posición actual de la partícula
            Vector3C rayo = (cubo.position - cubo.lastPosition).normalized;

            // Calcular la intersección de la línea con el plano
            float t = Vector3C.Dot(plano.position - cubo.lastPosition, plano.normal) / Vector3C.Dot(rayo, plano.normal);
            Vector3C intersectionPoint = cubo.lastPosition + rayo * t;

            // El punto de intersección será la nueva posición de la partícula
            cubo.position = intersectionPoint;

            // Reflejar la velocidad de la partícula con respecto a la normal del plano
            Vector3C reflectedVelocity = Vector3C.Reflect(cubo.euler, plano.normal);

            // Multiplicar la velocidad de la partícula por el coefficient de restitución
            // Puedes ajustar este valor según sea necesario (debe estar entre 0 y 1)
            Vector3C finalVelocity = reflectedVelocity * settings.bounce;//ni idea si esta bien

            cubo.euler = finalVelocity;
            // Mostrar resultados

        }
    }
}

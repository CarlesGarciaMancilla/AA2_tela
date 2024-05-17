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
            rotation = new QuatC(1.0f, 0.0f, 1.0f, 0.0f);

        }

        public void Euler(Vector3C force, float dt)
        {
            lastPosition = position;
            euler += force * dt;
            position += euler * dt;
        }

        public void RotationEuler(Vector3C axis, float angle)
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

        crb.Euler(settings.gravity, dt);
       
            
        


        for (int i = 0; i < settingsCollision.planes.Length; i++)
        {
            CollisionPlane(crb, settingsCollision.planes[i]);

        }

    }

    public void CollisionPlane(CubeRigidbody cubo, PlaneC plano)
    {
        // Check if the particle is behind the plane
        float distance = Vector3C.Dot(cubo.position, plano.normal) - (cubo.position.magnitude - plano.position.magnitude);

        if (distance < 0)
        {
            // Calculate the ray from the previous position to the current position of the particle
            Vector3C ray = (cubo.position - cubo.lastPosition).normalized;

            // Calculate the intersection of the ray with the plane
            float t = Vector3C.Dot(plano.position - cubo.lastPosition, plano.normal) / Vector3C.Dot(ray, plano.normal);
            Vector3C intersectionPoint = cubo.lastPosition + ray * t;

            // Update the position of the particle to the intersection point
            cubo.position = intersectionPoint;

            // Reflect the velocity of the particle with respect to the plane's normal
            Vector3C reflectedVelocity = Vector3C.Reflect(cubo.euler, plano.normal);

            // Multiply the particle's velocity by the coefficient of restitution
            // Adjust this value as needed (should be between 0 and 1)
            settings.gravity -= reflectedVelocity * settings.bounce;

            // Display results or perform any additional actions
        }
        

    }


    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
    }


}

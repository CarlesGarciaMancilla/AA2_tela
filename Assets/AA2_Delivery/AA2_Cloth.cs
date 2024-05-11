using System;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AA2_Cloth
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        [Min(1)]
        public float width;
        [Min(1)]
        public float height;
        [Min(2)]
        public int xPartSize;
        [Min(2)]
        public int yPartSize;
    }
    public Settings settings;
    [System.Serializable]
    public struct ClothSettings
    {
        [Header("Structural Sring")]
        public float structuralElasticCoef;
        public float structuralDampCoef;
        public float structuralSpring;
        public float structuralMaxL;
        [Header("Shear Sring")]
        public float shearElasticCoef;
        public float shearDampCoef;
        public float shearSpring;
        public float shearMaxL;
        [Header("Bending Sring")]
        public float bendingElasticCoef;
        public float bendingDampCoef;
        public float bendingSpring;
        public float bendingMaxL;
    }
    public ClothSettings clothSettings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public SphereC sphere;
    }
    public SettingsCollision settingsCollision;
    public struct Vertex
    {
        public Vector3C lastPosition;
        public Vector3C actualPosition;
        public Vector3C velocity;
        public Vertex(Vector3C _position)
        {
            this.actualPosition = _position;
            this.lastPosition = _position;
            this.velocity = new Vector3C(0, 0, 0);
        }

        public void Euler(Vector3C force, float dt)
        {
            lastPosition = actualPosition;
            velocity += force * dt;
            actualPosition += velocity * dt;
        }
    }
    public Vertex[] points;

    public void Update(float dt)
    {
        int xVertices = settings.xPartSize + 1;
        //int yVertices = settings.yPartSize + 1;

        Vector3C[] structuralForces = new Vector3C[points.Length];
        Vector3C[] shearForces = new Vector3C[points.Length];
        Vector3C[] bendingForces = new Vector3C[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            //STRUCTURAL VERTICAL
            if (i > xVertices - 1)
            {
                float structMagnitudeY = (points[i - xVertices].actualPosition - points[i].actualPosition).magnitude
                                        - clothSettings.structuralSpring;

                structMagnitudeY *= clothSettings.structuralMaxL * (points[i - xVertices].actualPosition - points[i].actualPosition).magnitude;

                Vector3C structForceVector = (points[i - xVertices].actualPosition
                    - points[i].actualPosition).normalized * structMagnitudeY * clothSettings.structuralElasticCoef;

                Vector3C structDampForceVector = (points[i].velocity
                    - points[i - xVertices].velocity) * clothSettings.structuralDampCoef;
                structuralForces[i] = structuralForces[i] + structForceVector;
                structuralForces[i] = structuralForces[i]  - structDampForceVector;
                structuralForces[i - xVertices] = structuralForces[i - xVertices] - structForceVector;
            }

            //STRUCTURAL HORIZONTAL
            if (i%(xVertices) !=0)
            {
                float structMagnitudeX = (points[i - 1].actualPosition - points[i].actualPosition).magnitude
                                        - clothSettings.structuralSpring;

                structMagnitudeX *= clothSettings.structuralMaxL * (points[i - 1].actualPosition - points[i].actualPosition).magnitude;

                Vector3C structForceVector = (points[i - 1].actualPosition
                    - points[i].actualPosition).normalized * structMagnitudeX * clothSettings.structuralElasticCoef;

                Vector3C structDampForceVector = (points[i].velocity
                    - points[i - 1].velocity) * clothSettings.structuralDampCoef;
                structuralForces[i] = structuralForces[i] + structForceVector;
                structuralForces[i] = structuralForces[i] - structDampForceVector;
                structuralForces[i - 1] = structuralForces[i - 1] - structForceVector;
            }
            //SHEAR DIAGONAL ASCENDENTE
            if (i > xVertices && i % (xVertices) != 0) 
            {
                float shearMagnitudeDiagonalA = (points[i -xVertices + 1].actualPosition - points[i].actualPosition).magnitude
                                            - clothSettings.shearSpring;

                shearMagnitudeDiagonalA *= clothSettings.shearMaxL * (points[i - xVertices + 1].actualPosition - points[i].actualPosition).magnitude;

                Vector3C shearForceVector = (points[i - xVertices + 1].actualPosition
                    - points[i].actualPosition).normalized * shearMagnitudeDiagonalA * clothSettings.shearElasticCoef;

                Vector3C shearDampForceVector = (points[i].velocity
                    - points[i - xVertices + 1].velocity) * clothSettings.shearDampCoef;
                shearForces[i] = shearForces[i] + shearForceVector;
                shearForces[i] = shearForces[i] - shearDampForceVector;
                shearForces[i - xVertices + 1] = shearForces[i - xVertices + 1] - shearForceVector;
            }
            //SHEAR DIAGONAL DESCENDENTE
            if (i > xVertices -1 && i % (xVertices) != 0)
            {
                float shearMagnitudeDiagonalD = (points[i - xVertices - 1].actualPosition - points[i].actualPosition).magnitude
                                            - clothSettings.shearSpring;

                shearMagnitudeDiagonalD *= clothSettings.shearMaxL * (points[i - xVertices - 1].actualPosition - points[i].actualPosition).magnitude;

                Vector3C shearForceVector = (points[i - xVertices - 1].actualPosition
                    - points[i].actualPosition).normalized * shearMagnitudeDiagonalD * clothSettings.shearElasticCoef;

                Vector3C shearDampForceVector = (points[i].velocity
                    - points[i - xVertices - 1].velocity) * clothSettings.shearDampCoef;
                shearForces[i] = shearForces[i] + shearForceVector;
                shearForces[i] = shearForces[i] - shearDampForceVector;
                shearForces[i - xVertices - 1] = shearForces[i - xVertices - 1] - shearForceVector;
            }
            //BENDING VERTICAL
            if (i > ((xVertices * 2) - 1)) 
            {
                float bendingMagnitudeV = (points[i - (xVertices*2)].actualPosition - points[i].actualPosition).magnitude
                                               - clothSettings.bendingSpring;

                bendingMagnitudeV *= clothSettings.bendingMaxL * (points[i - (xVertices * 2)].actualPosition - points[i].actualPosition).magnitude;

                Vector3C bendingForceVector = (points[i - (xVertices * 2)].actualPosition
                    - points[i].actualPosition).normalized * bendingMagnitudeV * clothSettings.bendingElasticCoef;

                Vector3C bendingDampForceVector = (points[i].velocity
                    - points[i - (xVertices * 2)].velocity) * clothSettings.bendingDampCoef;
                bendingForces[i] = bendingForces[i] + bendingForceVector;
                bendingForces[i] = bendingForces[i] - bendingDampForceVector;
                bendingForces[i - (xVertices * 2)] = bendingForces[i - (xVertices * 2)] - bendingForceVector;
            }
            //BENDING HORIZONTAL
            if (i % xVertices !=0 && i>1)
            {
                float bendingMagnitudeH = (points[i - 2].actualPosition - points[i].actualPosition).magnitude
                                               - clothSettings.bendingSpring;

                bendingMagnitudeH *= clothSettings.bendingMaxL * (points[i - 2].actualPosition - points[i].actualPosition).magnitude;

                Vector3C bendingForceVector = (points[i - 2].actualPosition
                    - points[i].actualPosition).normalized * bendingMagnitudeH * clothSettings.bendingElasticCoef;

                Vector3C bendingDampForceVector = (points[i].velocity
                    - points[i - 2].velocity) * clothSettings.bendingDampCoef;
                bendingForces[i] = bendingForces[i] + bendingForceVector;
                bendingForces[i] = bendingForces[i] - bendingDampForceVector;
                bendingForces[i - 2] = bendingForces[i - 2] - bendingForceVector;
            }
        }
        for (int i = xVertices; i < points.Length; i++)
        {
            points[i].Euler(settings.gravity + structuralForces[i] + shearForces[i] + bendingForces[i], dt);
            CollisionSphere(points[i], settingsCollision.sphere);
        }
    }

    public void CollisionPlane(Vertex particle, PlaneC plano)
    {
        // Comprobar si la partícula está delante o detrás del plano
        float dotProduct = Vector3C.Dot(plano.normal, particle.actualPosition - plano.position);
        UnityEngine.Debug.Log("dot product" + dotProduct);
        if (dotProduct < 0)
        {
            // Trazar una línea/rayo desde la posición anterior a la posición actual de la partícula
            Vector3C rayo = (particle.actualPosition - particle.lastPosition).normalized;

            // Calcular la intersección de la línea con el plano
            float t = Vector3C.Dot(plano.position - particle.lastPosition, plano.normal) / Vector3C.Dot(rayo, plano.normal);
            Vector3C intersectionPoint = particle.lastPosition + rayo * t;

            // El punto de intersección será la nueva posición de la partícula
            particle.actualPosition = intersectionPoint;

            // Reflejar la velocidad de la partícula con respecto a la normal del plano
            Vector3C reflectedVelocity = Vector3C.Reflect(particle.velocity, plano.normal);

            // Multiplicar la velocidad de la partícula por el coefficient de restitución
            // Puedes ajustar este valor según sea necesario (debe estar entre 0 y 1)
            Vector3C finalVelocity = reflectedVelocity * clothSettings.bendingElasticCoef;//ni idea si esta bien

            particle.velocity = finalVelocity;
            UnityEngine.Debug.Log("vel final" + particle.velocity);
            // Mostrar resultados

        }
        else
        {

        }
    }

    public void CollisionSphere(Vertex particle, SphereC esfera)
    {


        if (esfera.IsInside(esfera, particle.actualPosition))
        {

            Vector3C rayo = (particle.actualPosition - particle.lastPosition).normalized;

            float a = Vector3C.Dot(rayo, rayo);
            float b = 2 * Vector3C.Dot(particle.lastPosition - esfera.position, rayo);
            float c = (particle.lastPosition - esfera.position).magnitude - esfera.radius * esfera.radius;
            float discriminante = b * b - 4 * a * c;

            float t = (-b + (float)Math.Sqrt(discriminante)) / (2 * a);
            Vector3C intersectionPoint = particle.lastPosition + rayo * t;
            Vector3C intersectionNormal = esfera.position - intersectionPoint;
            particle.actualPosition = intersectionPoint;

            PlaneC planoEsfera = new PlaneC(intersectionPoint, intersectionNormal.normalized);
            CollisionPlane(particle, planoEsfera);
        }
        else
        {

        }
    }

    public void Debug()
    {
        settingsCollision.sphere.Print(Vector3C.blue);

        if (points != null)
            foreach (var item in points)
            {
                item.lastPosition.Print(0.05f);
                item.actualPosition.Print(0.05f);
            }
    }
}
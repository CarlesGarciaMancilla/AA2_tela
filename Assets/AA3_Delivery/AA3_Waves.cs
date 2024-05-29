using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

[System.Serializable]
public class AA3_Waves
{
    [System.Serializable]
    public struct Settings
    {
        public float gravity;
    }
    public Settings settings;
    [System.Serializable]
    public struct WavesSettings
    {
        public float amplitude;
        public float frequency;
        public float phase;
        public float speed;
        public float waveLength;
        public Vector3C direction;

    }
    [System.Serializable]
    public struct BuoySettings 
    {
        public float density;
        public float velocity;
        public float mass;
        public float coefficient;



    
    }
    public BuoySettings buoySettings;

    public SphereC buoy;

    public WavesSettings[] wavesSettings;
    public struct Vertex
    {
        public Vector3C originalposition;
        public Vector3C position;
        public Vertex(Vector3C _position)
        {
            this.position = _position;
            this.originalposition = _position;
        }
    }
    public Vertex[] points;
    private float elapsedTime, elapsedTime2;
    public AA3_Waves()   
    {
        elapsedTime = 0.0f;
        buoy.position = new Vector3C(0,0,0);
    }
    public void Update(float dt)
    {
  
        elapsedTime += dt * wavesSettings[0].speed;
        elapsedTime2 += dt * wavesSettings[1].speed;
        //Random rnd = new Random();
        for(int i = 0; i < points.Length; i++)
        {
            //ola 1
            //x
            points[i].position.x = (points[i].originalposition.x) + 
                (wavesSettings[0].amplitude * (2.0f * MathF.PI / wavesSettings[0].waveLength) * 
                MathF.Cos( (2.0f * MathF.PI / wavesSettings[0].waveLength) * (Vector3C.Dot(points[i].originalposition,wavesSettings[0].direction.normalized)) + elapsedTime)
                + wavesSettings[0].phase) * wavesSettings[0].direction.x;
            //z
            points[i].position.z = (points[i].originalposition.z) +
               (wavesSettings[0].amplitude * (2.0f * MathF.PI / wavesSettings[0].waveLength) *
               MathF.Cos((2.0f * MathF.PI / wavesSettings[0].waveLength) * (Vector3C.Dot(points[i].originalposition, wavesSettings[0].direction.normalized)) + elapsedTime)
               + wavesSettings[0].phase) * wavesSettings[0].direction.z;

            //y
            points[i].position.y = wavesSettings[0].amplitude * MathF.Sin((Vector3C.Dot(points[i].originalposition, wavesSettings[0].direction.normalized)+ elapsedTime) + wavesSettings[0].phase);

            //ola 2
            points[i].position.x += (points[i].originalposition.x) +
               (wavesSettings[1].amplitude * (2.0f * MathF.PI / wavesSettings[1].waveLength) *
               MathF.Cos((2.0f * MathF.PI / wavesSettings[1].waveLength) * (Vector3C.Dot(points[i].originalposition, wavesSettings[1].direction.normalized)) + elapsedTime2)
               + wavesSettings[1].phase) * wavesSettings[1].direction.x;
            //z
            points[i].position.z += (points[i].originalposition.z) +
               (wavesSettings[1].amplitude * (2.0f * MathF.PI / wavesSettings[0].waveLength) *
               MathF.Cos((2.0f * MathF.PI / wavesSettings[1].waveLength) * (Vector3C.Dot(points[i].originalposition, wavesSettings[1].direction.normalized)) + elapsedTime2)
               + wavesSettings[1].phase) * wavesSettings[1].direction.z;

            //y
            points[i].position.y += wavesSettings[1].amplitude * MathF.Sin((Vector3C.Dot(points[i].originalposition, wavesSettings[1].direction.normalized)+elapsedTime2) + wavesSettings[1].phase);
        }
        float forceFlotation = buoySettings.density * VolumeBuoy(buoy) * -settings.gravity * buoySettings.coefficient;
        float weight = buoySettings.mass * settings.gravity;
        float accelerationBuoy = (forceFlotation + weight) / buoySettings.mass;
        float velocityBuoy = 0;

        velocityBuoy += accelerationBuoy * dt; 
        buoy.position.y += velocityBuoy * dt;

    }

    public float GetWaveHeight(float x, float z) 
    {
        float final;
        final = wavesSettings[0].amplitude * MathF.Sin((Vector3C.Dot(new Vector3C(x,0.0f,z), wavesSettings[0].direction.normalized)+elapsedTime) +
            wavesSettings[0].phase) + 
            wavesSettings[1].amplitude * MathF.Sin((Vector3C.Dot(new Vector3C(x, 0.0f, z), wavesSettings[1].direction.normalized)+ elapsedTime2) + wavesSettings[1].phase);


        return final;
    }

    public float VolumeBuoy(SphereC buoy) 
    {
        float volume;
        float h = GetWaveHeight(buoy.position.x, buoy.position.z) - points[0].position.y;
        volume = (MathF.PI * MathF.Pow(2, h) /3) * (3*buoy.radius - h);

        return volume;
    }

    public void Debug()
    {
        if(points != null)
        foreach (var item in points)
        {
                buoy.Print(Vector3C.red);
            item.originalposition.Print(0.05f);
            item.position.Print(0.05f);
        }
    }
}

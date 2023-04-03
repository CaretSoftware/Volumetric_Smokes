using UnityEngine;
using UnityEngine.Serialization;

public class ProceduralAnimation : MonoBehaviour {
    [SerializeField, Range(0f, 10f), 
     Tooltip("natural frequency of system\n[Hz or cycles per second]")] 
    private float f = 2.65f;
    [SerializeField, Range(0f, 5f), 
     Tooltip("damping coefficient\n[0 is undamped, >1 will not vibrate, slowly settle towards target]")] 
    private float z = 0.23f;
    [SerializeField, Range(-10f, 10f), 
     Tooltip("initial response\n[<0 will anticipate, >1 will overshoot]")]
    private float r = -10f;
    
    [SerializeField] private Transform target;
    [FormerlySerializedAs("carryObjec0t")] public Transform carryObject;
    
    private SecondOrderDynamics _secondOrderDynamics;
    
    private void Start() {
        _secondOrderDynamics = new SecondOrderDynamics(f, z, r, target.position);
    }

    private void Update() {
        if (carryObject != null)
            carryObject.position = _secondOrderDynamics.Update(Time.deltaTime, target.position);
    }

    public Vector2[] GetGraph(out float zero, out float one) {
        float resolution = .01f;
        Vector3 pos = Vector3.zero;
        SecondOrderDynamics sod = new SecondOrderDynamics(f, z, r, Vector3.zero);

        float min = 0f;
        float max = 1f;

        for (int i = 0; i < 100; i++) {
            pos = sod.Update(1f/100f, Vector3.up);
            min = Mathf.Min(min, pos.y);
            max = Mathf.Max(max, pos.y);
        }
        
        sod.SetPos(Vector3.zero);

        Vector2[] points = new Vector2[100];
        float x = 0f;
        for (int i = 0; i < 100; i++) {
            pos = sod.Update(1f/100f, Vector3.up);
            float y = Mathf.InverseLerp(min, max, pos.y);
            points[i] = new Vector2(x, Mathf.LerpUnclamped(0.01f, .99f, y));
            x += resolution;
        }

        zero = Mathf.InverseLerp(min, max, 0f);
        one  = Mathf.InverseLerp(min, max, 1f);

        return points;
    }
}

/// <summary>
/// original @Author T3ssel8r on YouTube
/// translated from pseudocode by Patrik Bergsten
/// https://youtu.be/KPoeNZZ6H4s
/// </summary>
public class SecondOrderDynamics {
    private Vector3 prevPos;    // previous input
    private Vector3 pos, vel;      // state variables
    private float k1, k2, k3;   // dynamic constants

    public SecondOrderDynamics(float f, float z, float r, Vector3 pos) {
        
        // compute constants
        k1 = z / (Mathf.PI * f);
        k2 = 1 / ((2f * Mathf.PI * f) * (2f * Mathf.PI * f));
        k3 = r * z / (2 * Mathf.PI * f);
        // initialize variables
        prevPos = pos;
        this.pos = pos;
        vel = Vector3.zero;
    }

    public Vector3 Update(float dT, Vector3 pos) {
        // estimate velocity (if true velocity is used, anticipation cannot be modeled)
        Vector3 vel = (pos - prevPos) / dT;
        prevPos = pos;
        
        return Update(dT, pos, vel);
    }
    
    private Vector3 Update(float dT, Vector3 pos, Vector3 estVel) {
        float k2Stable = Mathf.Max(k2, 
            dT*dT/2f + dT*k1/2f, dT*k1);                                    // clamp k2 to guarantee stability without jitter
        this.pos += dT * vel;                                               // integrate position by velocity
        vel += dT * (pos + k3*estVel - this.pos - k1*vel) / k2Stable;       // integrate velocity by acceleration
        
        return this.pos;
    }

    public void SetPos(Vector3 pos) {
        this.pos = pos;
        prevPos = pos;
        vel = Vector3.zero;
    }
}
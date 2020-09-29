using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPerturb : MonoBehaviour
{
    public float k = 0.2f;
    public float dampening_factor = 0.95f;
    Vector3 velocity = Vector3.zero;

    private Vector3 initPos;

    public void Start()
    {
        initPos = transform.localPosition;
    }

    public void Perturb(float amplitude)
    {
        transform.localPosition = UnityEngine.Random.onUnitSphere * amplitude;
    }

    void Update()
    {
        Vector3 displacement = initPos - transform.localPosition;
        Vector3 acceleration = k * displacement;
        velocity += acceleration;
        velocity *= dampening_factor;

        transform.localPosition += velocity;
    }
}
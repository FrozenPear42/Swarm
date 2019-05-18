using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

    public Vec3D particlePosition;
    public Vec3D particleVelocity;
    public double alpha;
    public double theta;

    public Vec3D newParticlePosition;
    public Vec3D newParticleVelocity;
    public double newAlpha;
    public double newTheta;

    private MeshRenderer meshRenderer;

    private void Start() {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void CalculatePosition() {
        transform.position = new Vector3((float)particlePosition.x, (float)particlePosition.y, (float)particlePosition.z);
        if (particleVelocity.Length2() > 0.000001)
        transform.rotation = Quaternion.LookRotation(new Vector3((float)particleVelocity.x, (float)particleVelocity.y, (float)particleVelocity.z));

        var norm = particleVelocity.Normalize();
        if(meshRenderer)
            meshRenderer.material.SetColor("_Color", new Color((float)Math.Abs(norm.x), (float)Math.Abs(norm.y), (float)Math.Abs(norm.z)));
    }

    public void ApplyNewValues() {
        particlePosition = newParticlePosition;
        particleVelocity = newParticleVelocity;
        alpha = newAlpha;
        theta = newTheta;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3((float)particleVelocity.x, (float)particleVelocity.y, (float)particleVelocity.z) * 0.02f);
    }
}

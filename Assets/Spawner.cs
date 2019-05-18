using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour {
    public GameObject birdPrefab;

    public double stepDeltaTime = 0.5f;
    public int defaultParticleNumber = 50;
    public double radius = 0.01;
    public double simulationSpeed = 1.0;
    public double noiseIntensity = 0.0;

    [Serializable]
    public class DoubleEvent : UnityEvent<double> { }

    public DoubleEvent OrderCoefficientChanged;

    private List<Bird> _birds;
    private System.Random _rng;
    private bool _running = false;

    private void Awake() {
        _rng = new System.Random();
    }

    private void Start() {
        _birds = new List<Bird>();
    }

    private void Update() {
        foreach (var b in _birds) {
            b.CalculatePosition();
        }
    }

    private void FixedUpdate() {
        if (_running)
            SimulationStep(Time.fixedDeltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (_birds != null)
            foreach (var bird in _birds) {
                Gizmos.DrawWireSphere(bird.transform.position, (float) radius);
            }
    }

    public void Init(int birdsCount) {
        foreach (var b in _birds) {
            Destroy(b.gameObject);
        }

        _birds.Clear();

        for (var i = 0; i < birdsCount; ++i) {
            var obj = Instantiate(birdPrefab);
            var bird = obj.GetComponent<Bird>();
            var x = (_rng.NextDouble() * 2) - 1;
            var y = (_rng.NextDouble() * 2) - 1;
            var z = (_rng.NextDouble() * 2) - 1;
            var alpha = _rng.NextDouble() * 2 * Math.PI;
            var theta = _rng.NextDouble() * 2 * Math.PI;
            bird.particlePosition = new Vec3D(x, y, z);
            bird.alpha = alpha;
            bird.theta = theta;
            var sinAlpha = Math.Sin(alpha);
            var cosAlpha = Math.Cos(alpha);
            var sinTheta = Math.Sin(theta);
            var cosTheta = Math.Cos(theta);

            bird.particleVelocity = new Vec3D(cosTheta * sinAlpha, sinTheta * sinAlpha, cosAlpha);

            _birds.Add(bird);
        }
    }

    public void StartSimulation() {
        _running = true;
    }

    public void StopSimulation() {
        _running = false;
    }

    public void SetSimulationSpeed(float speed) {
        simulationSpeed = speed;
    }

    public void SetRadius(float rad) {
        radius = rad;
    }

    public void SetNumberOfParticles(float num) {
        defaultParticleNumber = (int) num;
        Init(defaultParticleNumber);
    }

    public void SetNoiseIntensity(float intensity) {
        noiseIntensity = intensity;
    }

    public void FixedSimulationStep() {
        SimulationStep(stepDeltaTime);
    }

    public void FixedInit() {
        Init(defaultParticleNumber);
    }

    public void SimulationStep(double deltaTime) {
        var rad2 = radius * radius;
        Vec3D sumVelocities = new Vec3D();
        // Lex sort
        var birds = _birds.OrderBy(b => b.particlePosition.x)
            .ThenBy(b => b.particlePosition.y)
            .ThenBy(b => b.particlePosition.y)
            .ToArray();

        for (var i = 0; i < birds.Length; ++i) {
            var bird = birds[i];

            double avgAlpha = bird.alpha;
            double avgTheta = bird.theta;
            int neighborCount = 1;
            for (var k = 1;
                Distance2PBC(bird.particlePosition, birds[(i + k) % birds.Length].particlePosition) <= rad2;
                k++) {
                avgAlpha += birds[(i + k) % birds.Length].alpha;
                avgTheta += birds[(i + k) % birds.Length].theta;
                neighborCount += 1;
            }

            for (var k = 1;
                Distance2PBC(bird.particlePosition,
                    birds[((birds.Length + i - k) % birds.Length) % birds.Length].particlePosition) <= rad2;
                k++) {
                avgAlpha += birds[((birds.Length + i - k) % birds.Length) % birds.Length].alpha;
                avgTheta += birds[((birds.Length + i - k) % birds.Length) % birds.Length].theta;
                neighborCount += 1;
            }

            avgAlpha = (avgAlpha / neighborCount) % (2 * Math.PI);
            avgTheta = (avgTheta / neighborCount) % (2 * Math.PI);

            var noiseAlpha = RandomGaussian(0, 0.1);
            var noiseTheta = RandomGaussian(0, 0.1);
//
//            var noiseAlpha = (_rng.NextDouble() * 2 * Math.PI) - Math.PI;
//            var noiseTheta = (_rng.NextDouble() * 2 * Math.PI) - Math.PI;

            var alpha = avgAlpha + noiseIntensity * noiseAlpha;
            var theta = avgTheta + noiseIntensity * noiseTheta;
            var sinAlpha = Math.Sin(alpha);
            var cosAlpha = Math.Cos(alpha);
            var sinTheta = Math.Sin(theta);
            var cosTheta = Math.Cos(theta);

            var velocity = new Vec3D(cosTheta * sinAlpha, sinTheta * sinAlpha, cosAlpha);
            var position = bird.particlePosition.Add(velocity.Times(deltaTime * simulationSpeed));

            bird.newParticlePosition = PositionPBC(position);
            bird.newParticleVelocity = velocity;
            bird.newAlpha = alpha;
            bird.newTheta = theta;

            sumVelocities = sumVelocities.Add(velocity);
        }

        foreach (var b in birds) {
            b.ApplyNewValues();
        }

        var orderCoeff = sumVelocities.Length() / _birds.Count;
        OrderCoefficientChanged.Invoke(orderCoeff);
    }

    private double RandomGaussian(double mean, double stdDev) {
        double u1 = 1.0 - _rng.NextDouble();
        double u2 = 1.0 - _rng.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    private double Distance2PBC(Vec3D a, Vec3D b) {
        var dx = b.x - a.x;
        var dy = b.y - a.y;
        var dz = b.z - a.z;

        if (dx > 1) dx = dx - 2;
        else if (dx < -1) dx = dx + 2;

        if (dy > 1) dy = dy - 2;
        else if (dy < -1) dy = dy + 2;

        if (dz > 1) dz = dz - 2;
        else if (dz < -1) dz = dz + 2;

        return dx * dx + dy * dy + dz * dz;
    }

    private Vec3D PositionPBC(Vec3D v) {
        var x = v.x;
        var y = v.y;
        var z = v.z;

        if (x > 1) x = x - 2;
        else if (x < -1) x = x + 2;

        if (y > 1) y = y - 2;
        else if (y < -1) y = y + 2;

        if (z > 1) z = z - 2;
        else if (z < -1) z = z + 2;

        return new Vec3D(x, y, z);
    }
}
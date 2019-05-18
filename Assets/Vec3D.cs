using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vec3D {
    public double x;
    public double y;
    public double z;

    public Vec3D() {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vec3D(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vec3D Times(double s) {
        return new Vec3D(x * s, y * s, z * s);
    }

    public Vec3D Add(Vec3D rhs) {
        return new Vec3D(x + rhs.x, y + rhs.y, z + rhs.z);
    }

    public Vec3D Normalize() {
       var length = Math.Sqrt(x*x + y*y + z *z);
       if (length < 0.001) return new Vec3D();
        return new Vec3D(x/length, y/length, z/length);
    }

    public override string ToString() {
        return $"({x}, {y}, {z})";
    }

    public double Distance2(Vec3D rhs) {
        return (x - rhs.x) * (x - rhs.x) + (y - rhs.y) * (y - rhs.y) + (z - rhs.z) * (z - rhs.z);
    }

    public double Length2() {
        return x * x + y * y + z * z;
    }

    public double Length() {
        return Math.Sqrt(Length2());
    }
}
using UnityEngine;

using static UnityEngine.Mathf;

public static class SurfaceLib
{
    public delegate Vector3 Surface(float u, float v, float t);
    public enum SurfaceType { Wave, MultiWave, Ripple, Sphere, Torus };

    private static Surface[] surfaces = { Wave, MultiWave, Ripple, Sphere, Torus };

    public static int SurfaceCount => surfaces.Length;

    public static Surface GetSurface(SurfaceType name) => surfaces[(int)name];

    public static SurfaceType MoveCycle(SurfaceType surf) => surf < SurfaceType.Torus ? surf + 1 : 0;

    public static Vector3 Morph(float u, float v, float t, Surface from, Surface to, float progress) => Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));


    public static Surface GetFunction(int index)
    {
        switch (index)
        {
            case 0:
                return Wave;
            case 1:
                return MultiWave;
            case 2:
                return Ripple;
            case 3:
                return Sphere;
            default:
                return Torus;
        }
    }

    public static SurfaceType MoveRandom(SurfaceType surf)
    {
        var choice = (SurfaceType)Random.Range(1, surfaces.Length);
        return choice == surf ? 0 : choice;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    { 
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
		p.y = Sin(PI* (4f * d - t));
		p.y /= 1f + 10f * d;
		p.z = v;
		return p;
	}
}
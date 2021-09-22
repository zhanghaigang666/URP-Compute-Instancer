using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    private const int maxResolution = 1000;

    [SerializeField, Range(10, maxResolution)]
    private int resolution = 10;

    [SerializeField]
    private SurfaceLib.SurfaceType surface;
    [SerializeField]
    private SurfaceLib.SurfaceType aimSurface;

    public enum MoveMode { Cycle, Random }

    [SerializeField]
    MoveMode moveMode;

    [SerializeField, Min(0f)]
    private float functionDuration = 1f, transitionDuration = 1f;
    private float duration = 0f;

    private bool transitioning = false;

    [SerializeField]
    private ComputeShader computeInstancer;
    private ComputeBuffer positionsBuffer;

    [SerializeField]
    private Material material;

    [SerializeField]
    private Mesh mesh;

    private int kernel;

    static private readonly int positionsId = Shader.PropertyToID("_Positions");
    static private readonly int resolutionId = Shader.PropertyToID("_Resolution");
    static private readonly int stepId = Shader.PropertyToID("_Step");
    static private readonly int timeId = Shader.PropertyToID("_Time");
    static private readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            aimSurface = surface;
            MoveNext();
        }

        kernel = (int)surface + (int)(transitioning ? aimSurface : surface) * SurfaceLib.SurfaceCount;

        UpdateShaderResources();
        RunGPUCompute();
    }

    private void UpdateShaderResources()
    {
        float step = 2f / resolution;
        computeInstancer.SetInt(resolutionId, resolution);
        computeInstancer.SetFloat(stepId, step);
        computeInstancer.SetFloat(timeId, Time.time);
        computeInstancer.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, duration / transitionDuration));
        computeInstancer.SetBuffer(kernel, positionsId, positionsBuffer);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
    }

    private void RunGPUCompute()
    {
        int groups = Mathf.CeilToInt(resolution / 8f);

        computeInstancer.Dispatch(kernel, groups, groups, 1);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    private void MoveNext()
    {
        surface = moveMode == MoveMode.Cycle ?
            SurfaceLib.MoveCycle(surface) :
            SurfaceLib.MoveRandom(surface);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
}

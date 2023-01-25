using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUWaveManager : MonoBehaviour
{
    public ComputeShader waveCompute;
    public RenderTexture wave, wavep1, wavem1;
    public Vector2Int resolution;

    public Material waveMaterial;

    public Vector3 effect; //x,y,strength of wave effect
    public float dispersion = 0.98f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTexture(ref wave);
        InitializeTexture(ref wavep1);
        InitializeTexture(ref wavem1);

        waveMaterial.mainTexture = wave;
    }

    void InitializeTexture (ref RenderTexture tex)
    {
        tex = new RenderTexture(resolution.x, resolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();

        //this is where we step forward the waves to the new values calculated in the compute shader
        Graphics.CopyTexture(wave, wavem1);
        Graphics.CopyTexture(wavep1, wave);


        waveCompute.SetTexture(0, "wave", wave);
        waveCompute.SetTexture(0, "wavep1", wavep1);
        waveCompute.SetTexture(0, "wavem1", wavem1);
        waveCompute.SetVector("effect", effect);
        waveCompute.SetVector("resolution", new Vector2(resolution.x, resolution.y));
        waveCompute.SetFloat("dispersion", dispersion);

        waveCompute.Dispatch(0, resolution.x / 8, resolution.y / 8, 1);
    }

    void FollowMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            effect = new Vector3(hit.textureCoord.x * resolution.x, hit.textureCoord.y * resolution.y, 1);
        }
    }

}

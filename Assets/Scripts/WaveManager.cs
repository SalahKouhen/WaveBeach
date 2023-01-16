using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public Material waveMaterial;
    public Texture2D waveTexture;

    [Header("Wave matricies")]
    float[][] wave, wavep1, wavem1; // p1 is for plus 1 (in future 1 t step)

    [Header("Dimensions")]
    float Lx = 10;
    float Ly = 10;
    float dx = 0.1f;
    float dy { get => dx; }
    int nx, ny;

    float CFL = 0.5f;
    float c = 1;
    float dt;
    float t;
    [SerializeField] float floatToColorMultiplier = 2f;
    [SerializeField] float freq = 2f;


    // Start is called before the first frame update
    void Start()
    {
        nx = Mathf.FloorToInt(Lx / dx);
        ny = Mathf.FloorToInt(Ly / dy);
        waveTexture = new Texture2D(nx, ny, TextureFormat.RGBA32, false);

        // TO DO: Change this to jagged array and see if faster
        wave = new float[nx][];
        wavep1 = new float[nx][];
        wavem1 = new float[nx][];
        for (int i = 0; i < nx; i++){
            wave[i] = new float[ny];
            wavep1[i] = new float[ny];
            wavem1[i] = new float[ny];
        }
        waveMaterial.SetTexture("_MainTex", waveTexture);
        waveMaterial.SetTexture("_Displacement", waveTexture);
    }

    void WaveStep()
    {
        dt = CFL * dx / c;
        t += dt;

        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                wavem1[i][j] = wave[i][j];
                wave[i][j] = wavep1[i][j];

            }
        }

        //Add the drip
        wave[50][50] = dt * dt * 20 * Mathf.Sin(t * Mathf.Rad2Deg * freq / 100f);

        for (int i = 1; i < nx - 1; i++)
        {
            for (int j = 1; j < ny - 1; j++)
            {
                wavep1[i][j] = 2f * wave[i][j] - wavem1[i][j] + CFL * CFL * (wave[i + 1][j] + wave[i - 1][j] + wave[i][j + 1] + wave[i][j - 1] - 4 * wave[i][j]);

            }
        }
    }

    void ApplyMatrixToTexture(float[][] state, ref Texture2D tex, float floatToColorMultiplier)
    {
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                float val = floatToColorMultiplier * wave[i][j];
                tex.SetPixel(i, j, new Color(val + 0.5f, val + 0.5f, val + 0.5f, 1f));

            }
        }
        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        WaveStep();
        ApplyMatrixToTexture(wave, ref waveTexture, floatToColorMultiplier);
        
    }
}

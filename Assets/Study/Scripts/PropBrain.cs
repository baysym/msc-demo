using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBrain : MonoBehaviour
{
    public bool isOod;
    public float timeFactor = 1f;

    public ParticleSystem realParticles;
    public ParticleSystem fakeParticles;

    private Renderer rend;

    //
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    //
    void Update()
    {
        float perlin = Mathf.PerlinNoise(0f, Time.time * timeFactor);
        var rpe = realParticles.emission;
        var fpe = fakeParticles.emission;

        if (isOod)
        {
            rend.material.color = new Color(perlin, perlin, perlin);
            rend.material.SetColor("_EmissionColor", new Color(perlin, perlin, perlin));

            rpe.rateOverTime = 0f;
            fpe.rateOverTime = 0f;
        }
        else
        {
            rend.material.color = new Color(0.5f, 0.5f, 0.5f);
            rend.material.SetColor("_EmissionColor", new Color(0.5f, 0.5f, 0.5f));

            rpe.rateOverTime = perlin * 10f;
            fpe.rateOverTime = (1f - perlin) * 10f;
        }
    }
}

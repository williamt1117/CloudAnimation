using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRenderer : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float minimumRenderThreshold = 0.8f;
    public bool drawBoundingBox = true;

    public float cloudSize = 1.0f;

    private bool updated;

    private ParticleSystem ps;
    private CloudSimulator cs;

	void Start()
    {
		ps = GetComponent<ParticleSystem>();
        cs = GetComponent<CloudSimulator>();
        if (ps == null || cs == null)
        {
			throw new Exception("CloudSimulator must be attached to a GameObject with a ParticleSystem and CloudAnimator");
		}
        updated = true;
    }

    private void redrawParticles()
    {
        List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
        for (int x = 0; x < cs.cloudArray.GetLength(0); x++)
        {
			for (int y = 0; y < cs.cloudArray.GetLength(1); y++)
            {
				for (int z = 0; z < cs.cloudArray.GetLength(2); z++)
                {
					if (cs.cloudArray[x, y, z] > minimumRenderThreshold)
                    {
						ParticleSystem.Particle p = new ParticleSystem.Particle();
                        p.position = new Vector3(x * cloudSize, y * cloudSize, z * cloudSize);
                        p.startSize = cloudSize * cs.cloudArray[x, y, z];
                        particles.Add(p);
					}
				}
			}
		}

        Debug.Log("Number of particles: " + particles.Count);
        ps.SetParticles(particles.ToArray(), particles.Count);
    }

    public void renderCloud()
    {
        this.updated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updated)
        {
            redrawParticles();
            updated = true;
        }
    }

    void OnDrawGizmos()
    {
		if (drawBoundingBox)
        {
			Gizmos.color = Color.cyan;
            float xSize = cs.sizeX * cloudSize;
            float ySize = cs.sizeY * cloudSize;
            float zSize = cs.sizeZ * cloudSize;
			Gizmos.DrawWireCube(transform.position + new Vector3(xSize/2, ySize/2, zSize/2),
                                                        new Vector3(xSize, ySize, zSize));
		}
	}
}

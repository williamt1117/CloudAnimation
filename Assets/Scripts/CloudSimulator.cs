using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSimulator : MonoBehaviour
{
    public float[,,] cloudArray;
    public int sizeX = 10;
    public int sizeY = 10;
    public int sizeZ = 10;

    public float timeBetweenFrames = 0.1f;
    private float lastUpdateTime = 0.0f;

    [Range(0.0f, 1.0f)]
    public float humidity = 0.05f;
    public Vector3 wind = new Vector3(0.0f, 1.0f, 1.0f);
	private Vector3 windNormal;
	public float minimumWindContribution = 0.1f;
    public float sumWeight = 0.4f;
    [Range(0.0f, 3f)]
    public float randomDecayWeight = 0.3f;
	[Range(0.0f, 3f)]
	public float fixedProportionalDecayWeight = 0.3f;
    [Range(0.0f, 1.0f)]
    public float overCrowdedPercentage = 0.9f;
    public float overCrowdedDecayWeight = 0.3f;
    public float perlinDecayWeight = 0.3f;
    public float perlinFrequency = 0.3f;
    public float perlinOffset = 45.5f;

    private CloudRenderer cr;

    private int[,] adjacent = new int[26, 3];

    void buildAdjacencyTable()
    {
        int idx = 0;
        for (int x = -1; x <= 1; x++)
        {
			for (int y = -1; y <= 1; y++)
            {
				for (int z = -1; z <= 1; z++)
                {
					if (x == 0 && y == 0 && z == 0)
                    {
						continue;
					}
                    adjacent[idx, 0] = x;
                    adjacent[idx, 1] = y;
                    adjacent[idx, 2] = z;
                    idx++;
				}
			}
		}
    }


    void Start()
    {
        cr = GetComponent<CloudRenderer>();

        cloudArray = new float[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
			for (int y = 0; y < sizeY; y++)
            {
				for (int z = 0; z < sizeZ; z++)
                {
                    cloudArray[x, y, z] = 0.4f;
				}
			}
		}

        buildAdjacencyTable();
        windNormal = wind.normalized;
    }

    float[,,] AdjacentSumRule()
    {
		float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
                    newCloudArray[x, y, z] = 0;
                    for (int i = 0; i < 26; i++)
                    {
						int xx = x + adjacent[i, 0];
                        int yy = y + adjacent[i, 1];
                        int zz = z + adjacent[i, 2];
                        int xx_mod = (xx+sizeX) % sizeX;
                        int yy_mod = (yy+sizeY) % sizeY;
                        int zz_mod = (zz+sizeZ) % sizeZ;
                        Vector3 vec = new Vector3(x - xx, y - yy, z - zz);
                        float dot = Vector3.Dot(vec.normalized, windNormal);
                        float contribution = Mathf.Max(minimumWindContribution, dot);
                       
                        newCloudArray[x, y, z] += contribution/26 * cloudArray[xx_mod, yy_mod, zz_mod];
					}
				}
			}
		}

        return newCloudArray;
	}

    float[,,] RandomDecayRule()
    {
		float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
                    newCloudArray[x,y,z] = UnityEngine.Random.Range(-cloudArray[x,y,z], 0.0f);
				}
			}
		}

        return newCloudArray;
	}

    float[,,] FixedPorportionalDecayRule()
    {
        float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];
        float sum = 0.0f;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
				for (int z = 0; z < sizeZ; z++)
                {
					sum += cloudArray[x, y, z];
				}
			}
        }
        float average = sum / (sizeX * sizeY * sizeZ);
        for (int x = 0; x < sizeX; x++)
        {
			for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    newCloudArray[x, y, z] = -average;
                }
            }
        }
        return newCloudArray;
    }

    float[,,] DecayCrowdedRule()
    {
        float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float sum = 0;
					for (int i = 0; i < 26; i++)
                    {
                        int xx = x + adjacent[i, 0];
                        int yy = y + adjacent[i, 1];
                        int zz = z + adjacent[i, 2];
                        int xx_mod = (xx + sizeX) % sizeX;
                        int yy_mod = (yy + sizeY) % sizeY;
                        int zz_mod = (zz + sizeZ) % sizeZ;
                        sum += cloudArray[xx_mod, yy_mod, zz_mod];
                    }
                    sum /= 26;
                    newCloudArray[x, y, z] = sum > overCrowdedPercentage ? -1 : 0;
				}
            }
        }
        return newCloudArray;
    }

    public static float Perlin3D(float x, float y, float z)
    {
        float noiseXY = Mathf.PerlinNoise(x, y);
        float noiseYZ = Mathf.PerlinNoise(y, z);
        float noiseXZ = Mathf.PerlinNoise(x, z);

        float noiseYX = Mathf.PerlinNoise(y, x);
        float noiseZY = Mathf.PerlinNoise(z, y);
        float noiseZX = Mathf.PerlinNoise(z, x);

        float result = noiseXY + noiseYZ + noiseXZ + noiseYX + noiseZY + noiseZX;
        return result / 6;
    }

    public float[,,] PerlinDecayRule()
    {
        float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
			for (int y = 0; y < sizeY; y++)
            {
				for (int z = 0; z < sizeZ; z++)
                {
                    float xx = (x+perlinOffset) * perlinFrequency;
                    float yy = (y+perlinOffset) * perlinFrequency;
                    float zz = (z+perlinOffset) * perlinFrequency;
                    float perlin = Unity.Mathematics.noise.snoise(new Unity.Mathematics.float3(xx, yy, zz));
                    perlin = -perlin * cloudArray[x, y, z];
                    newCloudArray[x, y, z] = perlin;
				}
			}
		}
        return newCloudArray;
    }

    void SimulateStep()
    {
        float[,,] newCloudArray = new float[sizeX, sizeY, sizeZ];

        float[,,] adjacentSum = AdjacentSumRule();
        float[,,] randomDecay = RandomDecayRule();
        float[,,] fixedDecay = FixedPorportionalDecayRule();
        float[,,] crowdedDecay = DecayCrowdedRule();
        float[,,] perlinDecay = PerlinDecayRule();

		for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
				for (int z = 0; z < sizeZ; z++)
                {
                    newCloudArray[x, y, z] = cloudArray[x, y, z];
                    newCloudArray[x, y, z] += sumWeight * adjacentSum[x, y, z];
                    newCloudArray[x, y, z] += randomDecayWeight * randomDecay[x, y, z];
                    newCloudArray[x, y, z] += fixedProportionalDecayWeight * fixedDecay[x, y, z];
                    newCloudArray[x, y, z] += overCrowdedDecayWeight * crowdedDecay[x, y, z];
                    newCloudArray[x, y, z] += perlinDecayWeight * perlinDecay[x, y, z];
                    newCloudArray[x, y, z] = Mathf.Clamp(newCloudArray[x, y, z], humidity, 1.0f);
				}
			}
        }

        cloudArray = newCloudArray;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdateTime > timeBetweenFrames)
        {
			lastUpdateTime = Time.time;
            SimulateStep();
            if (cr != null)
			    cr.renderCloud();
		}
    }
}

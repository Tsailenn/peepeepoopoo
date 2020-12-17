using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KMeans : MonoBehaviour
{
    [SerializeField] int totalPoints = 60;
    [SerializeField] Vector3[] positions;
    [SerializeField] List<Vector3>[] groups;
    [SerializeField] List<Vector3>[][] recordedGroups;
    [SerializeField] float[] vertexDistributions;
    [SerializeField] float[][] overallDistributions;
    [SerializeField] int totalK = 3;
    [SerializeField] Vector3[][] overallKpos;
    [SerializeField] Vector3[] kPosOrigin;
    [SerializeField] Vector3[] kPos;
    [SerializeField] Vector3[] kPosPrevious;
    [SerializeField] float xLimit = 5;
    [SerializeField] float yLimit = 5;
    //internal int iterationCycle = 0;

    [SerializeField] GameObject vertexObj;
    [SerializeField] GameObject vertexKObj;
    [SerializeField] GameObject vertexKOriginObj;
    List<GameObject> vertexArray;
    [SerializeField] bool next = false;
    [SerializeField] bool trigger = false;
    [SerializeField] GameObject finishedVertex;
    GameObject finishedVertexObj;
    VertexFinished finishedVertexScript;

    internal void Commence()
    {
        positions = new Vector3[totalPoints];
        kPos = new Vector3[totalK];
        groups = new List<Vector3>[totalK];
        vertexArray = new List<GameObject>();

        for (int i = 0; i < totalPoints; i++)
        {
            positions[i] = new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-yLimit, yLimit));
        }
        Vector3 rndChosen;
        
        for (int i = 0; i < totalK; i++)
        {
            rndChosen = positions[Random.Range(0, totalPoints)];

            //Debug.LogError(kPos.Contains(rndChosen));

            while (kPos.Contains(rndChosen))
            {
                rndChosen = positions[Random.Range(0, totalPoints)];
            }
            
            kPos[i] = rndChosen;
        }
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i] = new List<Vector3>();
        }
    }

    internal void Preparation()
    {
        positions = new Vector3[totalPoints];
        kPosOrigin = new Vector3[totalK];
        overallDistributions = new float[totalK][];
        recordedGroups = new List<Vector3>[totalK][];
        vertexArray = new List<GameObject>();
        overallKpos = new Vector3[totalK][];
        //iterationCycle = 0;

        for (int i = 0; i < totalPoints; i++)
        {
            positions[i] = new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-yLimit, yLimit));
        }
        Vector3 rndChosen;

        for (int i = 0; i < totalK; i++)
        {
            rndChosen = positions[Random.Range(0, totalPoints)];
            while (kPosOrigin.Contains(rndChosen))
            {
                rndChosen = positions[Random.Range(0, totalPoints)];
            }

            kPosOrigin[i] = rndChosen;
        }


    }

    internal void Redo(int iterationNum = 0)
    {
        kPos = new Vector3[iterationNum + 1];
        kPosPrevious = new Vector3[iterationNum + 1];
        groups = new List<Vector3>[iterationNum + 1];
        vertexDistributions = new float[iterationNum + 1];

        for (int i = 0; i < kPos.Length; i++)
        {
            kPos[i] = kPosOrigin[i];
        }
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i] = new List<Vector3>();
        }
    }

    internal float CalculateDistance(Vector3 point1, Vector3 point2)
    {
        return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2));
    }

    IEnumerator FindClosestK()
    {
        foreach (Vector3 i in positions)
        {
            float dist = Mathf.Infinity;
            int kIndex = 0;
            for (int kInd = 0; kInd < kPos.Length; kInd++)
            {
                if (CalculateDistance(i, kPos[kInd]) < dist)
                {
                    dist = CalculateDistance(i, kPos[kInd]);
                    kIndex = kInd;
                }
            }
            groups[kIndex].Add(i);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator RecalculateMeans()
    {
        kPosPrevious = (Vector3[])kPos.Clone();
        for (int i = 0; i < groups.Length; i++)
        {
            float totalX = 0;
            float totalY = 0;
            for (int j = 0; j < groups[i].Count; j++)
            {
                totalX += groups[i][j].x;
                totalY += groups[i][j].y;
            }
            yield return new WaitForFixedUpdate();
            kPos[i] = new Vector3(totalX / groups[i].Count, totalY / groups[i].Count);
        }
    }

    internal bool Changed()
    {
        int trueCount = 0;
        for (int i = 0; i < kPos.Length; i++)
        {
            if (kPos[i] == kPosPrevious[i])
            {
                trueCount++;
            }
        }
        return !(trueCount == kPos.Length);

    }

    IEnumerator MajorCycle()
    {
        Preparation();
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < totalK; i++)
        {
            Redo(i);
            do
            {
                for (int j = 0; j < groups.Length; j++)
                {
                    groups[j].Clear();
                }
                yield return StartCoroutine(FindClosestK());
                yield return StartCoroutine(RecalculateMeans());

                /*
                for (int j = 0; j < groups.Length; j++)
                {
                    for (int k = 0; k < groups[j].Count; k++)
                    {
                        vertexArray.Add(Instantiate(vertexObj, groups[j][k], new Quaternion()));
                    }
                }
                for (int j = 0; j < kPos.Length; j++)
                {
                    vertexArray.Add(Instantiate(vertexKObj, kPos[j], new Quaternion()));
                    Debug.LogError(kPos[j]);
                }
                yield return new WaitUntil(() => next);

                next = false;
                for (int j = 0; j < vertexArray.Count; j++)
                {
                    Destroy(vertexArray[j]);
                }
                vertexArray.RemoveAll((el) => !el);*/
                yield return new WaitForFixedUpdate();
            } while (Changed());

            overallKpos[i] = kPos;

            for (int j = 0; j < groups.Length; j++)
            {
                vertexDistributions[j] = groups[j].Count;
            }
            overallDistributions[i] = (float[])vertexDistributions.Clone();
            yield return new WaitForFixedUpdate();
            //recordedGroups[i] = groups.Clone() as Vector3[];
            //groups.CopyTo(recordedGroups, i);
            recordedGroups[i] = new List<Vector3>[groups.Length];
            for (int j = 0; j < groups.Length; j++)
            {
                recordedGroups[i][j] = groups[j];
            }
            yield return new WaitForFixedUpdate();
        }
        float[] deviations = new float[totalK];
        for (int i = 0; i < overallDistributions.Length; i++)
        {
            float highestDeviations = Mathf.NegativeInfinity;
            float avg = 0;
            for (int j = 0; j < overallDistributions[i].Length; j++)
            {
                avg += overallDistributions[i][j];
            }
            avg /= overallDistributions[i].Length;
            for (int j = 0; j < overallDistributions[i].Length; j++)
            {
                if (Mathf.Abs(avg - overallDistributions[i][j]) > highestDeviations)
                {
                    highestDeviations = Mathf.Abs(avg - overallDistributions[i][j]);
                }
            }
            deviations[i] = highestDeviations;
            yield return new WaitForFixedUpdate();
        }
        float minVal = Mathf.Infinity;
        int targetInd = 0;
        for (int i = 1; i < deviations.Length-1; i++)
        {
            if (deviations[i] < minVal)
            {
                targetInd = i;
                minVal = deviations[i];
            }
            yield return new WaitForFixedUpdate();
        }
        NeoSpawn(recordedGroups[targetInd], overallKpos[targetInd]);
    }

    IEnumerator Cycle()
    {

        Commence(); 
        for (int i = 0; i < vertexArray.Count; i++)
        {
            Destroy(vertexArray[i]);
        }
        vertexArray.RemoveAll((el) => !el);
        do
        {
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].Clear();
            }
            yield return StartCoroutine(FindClosestK());
            yield return StartCoroutine(RecalculateMeans());
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i].Count; j++)
                {
                    vertexArray.Add(Instantiate(vertexObj, groups[i][j], new Quaternion()));
                }
            }
            for (int i = 0; i < kPos.Length; i++)
            {
                vertexArray.Add(Instantiate(vertexKObj, kPos[i], new Quaternion()));
                Debug.LogError(kPos[i]);
            }
            yield return new WaitUntil(() => next);
            
            next = false;
            for (int i = 0; i < vertexArray.Count; i++)
            {
                Destroy(vertexArray[i]);
            }
            vertexArray.RemoveAll((el) => !el);
            yield return new WaitForFixedUpdate();
        } while (Changed());

        for (int i = 0; i < groups.Length; i++)
        {
            for (int j = 0; j < groups[i].Count; j++)
            {
                vertexArray.Add(Instantiate(vertexObj, groups[i][j], new Quaternion()));
            }
        }
        for (int i = 0; i < kPos.Length; i++)
        {
            vertexArray.Add(Instantiate(vertexKObj, kPos[i], new Quaternion()));
        }
        yield return new WaitForFixedUpdate();
        SpawnFinal();
    }

    internal void Begin()
    {
        
        StartCoroutine(Cycle());
    }
    internal void Begin2()
    {
        
        StartCoroutine(MajorCycle());
    }

    private void FixedUpdate()
    {
        if (trigger)
        {
            trigger = false;
            Begin2();
        }
    }

    internal void SpawnFinal()
    {
        finishedVertexObj = Instantiate(finishedVertex);
        finishedVertexScript = finishedVertexObj.GetComponent(typeof(VertexFinished)) as VertexFinished;
        GameObject obj;
        for (int i = 0; i < groups.Length; i++)
        {
            for (int j = 0; j < groups[i].Count; j++)
            {
                obj = Instantiate(finishedVertexObj, groups[i][j], new Quaternion());
                VertexFinished script = obj.GetComponent(typeof(VertexFinished)) as VertexFinished;
                script.targetPos = kPos[i];
                obj.SetActive(true);
            }
        }
    }

    internal void NeoSpawn(List<Vector3>[] g, Vector3[] centers)
    {
        finishedVertexObj = Instantiate(finishedVertex);
        finishedVertexScript = finishedVertexObj.GetComponent(typeof(VertexFinished)) as VertexFinished;
        GameObject obj; 
        for (int i = 0; i < g.Length; i++)
        {
            for (int j = 0; j < g[i].Count; j++)
            {
                obj = Instantiate(finishedVertexObj, g[i][j], new Quaternion());
                VertexFinished script = obj.GetComponent(typeof(VertexFinished)) as VertexFinished;
                script.targetPos = centers[i];
                obj.SetActive(true);
            }
        }

        for (int i = 0; i < centers.Length; i++)
        {
            Instantiate(vertexKObj, centers[i], new Quaternion());
            Instantiate(vertexKOriginObj, kPosOrigin[i], new Quaternion());
        }
    }

}

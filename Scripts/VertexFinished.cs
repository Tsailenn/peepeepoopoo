using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexFinished : MonoBehaviour
{
    internal Vector3 originPos;
    internal Vector3 targetPos;
    [SerializeField] float progPercent = 1.2f;
    float totalProg = 0;
    int progMult = 1;

    private void Start()
    {
        originPos = transform.position;
        StartCoroutine(Commence());
        //Debug.LogError(targetPos);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(originPos, targetPos, totalProg);
        totalProg = Mathf.Clamp01(totalProg + progPercent * Time.deltaTime * progMult);
    }

    IEnumerator Commence()
    {
        while (true)
        {
            yield return new WaitUntil(() => totalProg == 1);
            progMult = -1;
            yield return new WaitUntil(() => totalProg == 0);
            progMult = 1;
        }
    }

}

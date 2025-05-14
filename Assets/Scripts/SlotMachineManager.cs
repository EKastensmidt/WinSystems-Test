
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineManager : MonoBehaviour 
{
    public List<Reel> reels;
    public float delayBetweenReels = 0.2f;

    public void StartSpin()
    {
        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        for (int i = 0; i < reels.Count; i++)
        {
            float duration = Random.Range(2f, 4f);
            StartCoroutine(reels[i].Spin(duration));
            yield return new WaitForSeconds(delayBetweenReels);
        }

        yield return new WaitForSeconds(5f); // Wait max duration
        EvaluateResult();
    }

    void EvaluateResult()
    {
        List<List<string>> results = new();

        foreach (var reel in reels)
        {
            results.Add(reel.GetVisibleSymbols());
        }

        // Check against line patterns here
    }
}

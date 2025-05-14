using System.Collections;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    [SerializeField] private Reel[] _reels;
    [SerializeField] private float _delayBetweenReels = 0.2f;

    private void Start()
    {
        SpinAllReels();
    }

    public void SpinAllReels()
    {
        StartCoroutine(SpinSequence());
    }

    IEnumerator SpinSequence()
    {
        foreach (Reel reel in _reels)
        {
            float duration = Random.Range(2f, 4f);
            reel.StartSpin(duration);
            yield return new WaitForSeconds(_delayBetweenReels);
        }

        // Optionally wait until all reels are finished, then check for win
    }
}

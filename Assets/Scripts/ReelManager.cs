using System.Collections;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    [SerializeField] private Reel[] _reels;
    [SerializeField] private float _delayBetweenReels = 0.2f;
    [SerializeField] private float _minStoppageTime = 2f, _maxStoppageTime = 4f;

    private bool _isSpinning = false;

    public void SpinButton()
    {
        if (!_isSpinning)
            StartCoroutine(SpinReels());
    }

    IEnumerator SpinReels() //STARTS EACH REEL
    {
        _isSpinning = true;

        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].StartSpin();
        }

        for (int i = 0; i < _reels.Length; i++) //STOPS REELS IN SEQUENCE FROM LEFT TO RIGHT
        {
            float spinDuration = Random.Range(_minStoppageTime, _maxStoppageTime);
            yield return new WaitForSeconds(spinDuration);
            _reels[i].ForceStop();

            yield return new WaitForSeconds(_delayBetweenReels);
        }

        yield return new WaitUntil(() => AllReelsStopped());

        EvaluateResults(); //CHECK LINE PATTERNS AFTER EVERYTHING STOPS
        _isSpinning = false;
    }

    private bool AllReelsStopped()
    {
        foreach (var reel in _reels)
        {
            if (reel.IsSpinning) return false;
        }
        return true;
    }

    private void EvaluateResults()
    {
        int[,] grid = new int[5, 3]; 

        for (int i = 0; i < _reels.Length; i++)
        {
            var symbols = _reels[i].GetVisibleSymbols(); 
            for (int j = 0; j < 3; j++)
            {
                grid[i, j] = symbols[j];
            }
        }
    }
}

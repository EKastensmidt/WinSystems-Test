using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    public Action OnSpinCompletion;
    public Action<int> OnCreditsScored;
    public Action OnSpinStart;

    [SerializeField] private Reel[] _reels;
    [SerializeField] private float _delayBetweenReels = 0.2f;
    [SerializeField] private float _minStoppageTime = 2f, _maxStoppageTime = 4f;

    [SerializeField] private LinePatternDatabase _patternDatabase;
    [SerializeField] private SymbolPayoutTable _payoutTable;

    [SerializeField] private LineDisplayManager _lineDisplayManager;

    private bool _isSpinning = false;

    public void StartSpinSequence()
    {
        if (!_isSpinning)
            StartCoroutine(SpinReels());
    }

    IEnumerator SpinReels()
    {
        OnSpinStart?.Invoke();

        _lineDisplayManager.ClearLines();

        _isSpinning = true;

        // START REELS ONE BY ONE
        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].StartSpin();
            yield return new WaitForSeconds(_delayBetweenReels);
        }

        // STOP REEL ONE BY ONE
        for (int i = 0; i < _reels.Length; i++)
        {
            float spinDuration = UnityEngine.Random.Range(_minStoppageTime, _maxStoppageTime);
            yield return new WaitForSeconds(spinDuration);
            _reels[i].ForceStop();

            yield return new WaitForSeconds(_delayBetweenReels);
        }

        yield return new WaitUntil(() => AllReelsStopped());

        EvaluateResults(); //CALCULATE RESULTS ONCE EVERYTHING STOPS
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
        Sprite[,] symbolGrid = new Sprite[5, 3]; // [REEL, ROW]

        int totalCreditsScored = 0;

        List<int[]> winningPatterns = new();

        for (int i = 0; i < _reels.Length; i++)
        {
            var visibleSymbols = _reels[i].GetVisibleSymbolsByYPositions();
            for (int j = 0; j < 3; j++)
            {
                symbolGrid[i, j] = visibleSymbols[j];
            }
        }

        foreach (var pattern in _patternDatabase.patterns)
        {
            Sprite firstSymbol = symbolGrid[0, pattern.rows[0]];
            int matchCount = 1;

            for (int i = 1; i < 5; i++)
            {
                Sprite nextSymbol = symbolGrid[i, pattern.rows[i]];
                if (nextSymbol == firstSymbol)
                    matchCount++;
                else
                    break;
            }

            if (matchCount >= 2)
            {
                int reward = _payoutTable.GetPayout(firstSymbol, matchCount);
                if (reward > 0)
                {
                    totalCreditsScored += reward;
                    Debug.Log($"Win on pattern '{pattern.patternName}' with {matchCount}x '{firstSymbol.name}' for {reward} credits.");
                }

                winningPatterns.Add(pattern.rows);
            }
        }

        _lineDisplayManager.SetWinningLines(winningPatterns); //VISUAL REPRESENTATION OF LINES

        OnCreditsScored?.Invoke(totalCreditsScored);
        OnSpinCompletion?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineDisplayManager : MonoBehaviour
{
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private RectTransform _lineParent;
    [SerializeField] private RectTransform[] _reelAnchors;
    [SerializeField] private float _symbolSpacingY = 250f;
    [SerializeField] private float _lineDisplayTime = 1f;

    private List<int[]> _winningLines = new(); 
    private List<GameObject> _activeLines = new();
    private Coroutine _loopCoroutine;

    public void SetWinningLines(List<int[]> lines)
    {
        ClearLines();
        _winningLines = lines;

        if (_loopCoroutine != null)
            StopCoroutine(_loopCoroutine);

        _loopCoroutine = StartCoroutine(ShowLinesLoop());
    }

    public void ClearLines()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        foreach (var line in _activeLines)
            Destroy(line);

        _activeLines.Clear();
        _winningLines.Clear();
    }

    private IEnumerator ShowLinesLoop()
    {
        while (true)
        {
            foreach (var linePattern in _winningLines)
            {
                //SHOW LINE
                List<GameObject> segmentLines = CreateUILineFromPattern(linePattern);
                _activeLines.AddRange(segmentLines);

                yield return new WaitForSeconds(_lineDisplayTime);

                //HIDE LINE
                foreach (var line in segmentLines)
                    Destroy(line);

                _activeLines.Clear();
            }
        }
    }

    private List<GameObject> CreateUILineFromPattern(int[] rowIndexes)
    {
        List<GameObject> created = new();
        List<Vector2> points = new();

        for (int i = 0; i < 5; i++)
        {
            Vector2 basePos = _reelAnchors[i].anchoredPosition;
            float yOffset = -rowIndexes[i] * _symbolSpacingY;
            points.Add(basePos + new Vector2(0, yOffset));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            GameObject lineGO = Instantiate(_linePrefab, _lineParent);
            RectTransform rt = lineGO.GetComponent<RectTransform>();

            Vector2 start = points[i];
            Vector2 end = points[i + 1];
            Vector2 direction = end - start;
            float distance = direction.magnitude;

            rt.sizeDelta = new Vector2(distance, rt.sizeDelta.y);
            rt.anchoredPosition = start;
            rt.rotation = Quaternion.FromToRotation(Vector3.right, direction.normalized);

            created.Add(lineGO);
        }

        return created;
    }
}
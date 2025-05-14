using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [SerializeField] private RectTransform _contentArea;
    [SerializeField] private GameObject _symbolPrefab;
    [SerializeField] private List<Sprite> _symbolSprites;

    [SerializeField] private int _visibleSymbols = 3;
    [SerializeField] private int _totalSymbols = 10;
    [SerializeField] private float _spinSpeed = 500f;

    private List<GameObject> _spawnedSymbols = new List<GameObject>();
    private bool _isSpinning = false;
    private float _stopTime;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // Clean up
        foreach (Transform child in _contentArea) Destroy(child.gameObject);
        _spawnedSymbols.Clear();

        for (int i = 0; i < _totalSymbols; i++)
        {
            var symbol = Instantiate(_symbolPrefab, _contentArea);
            var image = symbol.GetComponent<Image>();
            image.sprite = _symbolSprites[Random.Range(0, _symbolSprites.Count)];
            _spawnedSymbols.Add(symbol);
        }
    }

    public void StartSpin(float duration)
    {
        _stopTime = Time.time + duration;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        _isSpinning = true;
        while (Time.time < _stopTime)
        {
            _contentArea.anchoredPosition -= new Vector2(0, _spinSpeed * Time.deltaTime);
            if (_contentArea.anchoredPosition.y <= -_symbolPrefab.GetComponent<RectTransform>().rect.height)
            {
                // Loop content
                _contentArea.anchoredPosition += new Vector2(0, _symbolPrefab.GetComponent<RectTransform>().rect.height);
                var first = _spawnedSymbols[0];
                _spawnedSymbols.RemoveAt(0);
                _spawnedSymbols.Add(first);
                first.transform.SetAsLastSibling();
                var img = first.GetComponent<Image>();
                img.sprite = _symbolSprites[Random.Range(0, _symbolSprites.Count)];
            }
            yield return null;
        }

        _isSpinning = false;
    }

    public List<int> GetVisibleSymbols()
    {
        var visible = new List<int>();
        for (int i = 0; i < _visibleSymbols; i++)
        {
            var img = _contentArea.GetChild(i).GetComponent<Image>();
            visible.Add(_symbolSprites.IndexOf(img.sprite));
        }
        return visible;
    }
}
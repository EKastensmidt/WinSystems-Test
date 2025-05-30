﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [SerializeField] private RectTransform _contentArea;
    [SerializeField] private GameObject _symbolPrefab;
    [SerializeField] private List<Sprite> _symbolSprites;

    [SerializeField] private int _visibleSymbols = 3;
    [SerializeField] private float _spinSpeed = 500f;

    private List<GameObject> _spawnedSymbols = new List<GameObject>();
    private bool _isSpinning = false;

    public bool IsSpinning { get => _isSpinning;}

    private void Start()
    {
        Init();
    }

    public void Init() //SPAWNS SYMBOLS AND SETS THEM INTO POSITION
    {
        foreach (Transform child in _contentArea)
            Destroy(child.gameObject);

        _spawnedSymbols.Clear();

        float symbolHeight = _symbolPrefab.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < _symbolSprites.Count; i++)
        {
            var sprite = _symbolSprites[i];
            var symbol = Instantiate(_symbolPrefab, _contentArea);
            symbol.GetComponent<Image>().sprite = sprite;
            _spawnedSymbols.Add(symbol);

            var rt = symbol.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -i * symbolHeight);
        }

        _contentArea.anchoredPosition = Vector2.zero;
    }

    public void StartSpin()
    {
        _isSpinning = true;
        StartCoroutine(SpinLoop());
    }

    private IEnumerator SpinLoop()
    {
        _isSpinning = true;

        float symbolHeight = _symbolPrefab.GetComponent<RectTransform>().rect.height;
        int symbolCount = _spawnedSymbols.Count;

        while (_isSpinning)
        {
            foreach (var symbolGO in _spawnedSymbols)
            {
                RectTransform rt = symbolGO.GetComponent<RectTransform>();
                rt.anchoredPosition -= new Vector2(0, _spinSpeed * Time.deltaTime);

                // IF SYMBOL GOES OFF THE BOTTOM, WRAP IT BACK TO TOP
                if (rt.anchoredPosition.y < -symbolHeight * _visibleSymbols)
                {
                    float highestY = GetHighestSymbolY();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, highestY + symbolHeight);
                }
            }

            yield return null;
        }
    }

    private float GetHighestSymbolY()
    {
        float maxY = float.MinValue;
        foreach (var symbol in _spawnedSymbols)
        {
            float y = symbol.GetComponent<RectTransform>().anchoredPosition.y;
            if (y > maxY) maxY = y;
        }
        return maxY;
    }

    public void ForceStop()
    {
        _isSpinning = false;
        StopAllCoroutines();
        StartCoroutine(SnapToFinalPosition());
    }

    private IEnumerator SnapToFinalPosition()//SNAPS SIMBOLS TO GRID
    {
        _isSpinning = true;

        float symbolHeight = _symbolPrefab.GetComponent<RectTransform>().rect.height;
        float spinTime = 0.3f;
        float elapsed = 0f;

        while (elapsed < spinTime)
        {
            foreach (var symbolGO in _spawnedSymbols)
            {
                RectTransform rt = symbolGO.GetComponent<RectTransform>();
                rt.anchoredPosition -= new Vector2(0, _spinSpeed * 2f * Time.deltaTime);

                if (rt.anchoredPosition.y < -symbolHeight * _visibleSymbols)
                {
                    float highestY = GetHighestSymbolY();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, highestY + symbolHeight);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        AlignSymbolsToGrid(symbolHeight);

        _isSpinning = false;
    }

    private void AlignSymbolsToGrid(float symbolHeight)
    {
        float baseY = GetHighestSymbolY();
        baseY = Mathf.Round(baseY / symbolHeight) * symbolHeight;

        _spawnedSymbols.Sort((a, b) => //SORT BY Y
            b.GetComponent<RectTransform>().anchoredPosition.y.CompareTo(
                a.GetComponent<RectTransform>().anchoredPosition.y)
        );

        for (int i = 0; i < _spawnedSymbols.Count; i++)
        {
            RectTransform rt = _spawnedSymbols[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, baseY - i * symbolHeight);
        }
    }

    public List<Sprite> GetVisibleSymbolsByYPositions()
    {
        List<Sprite> visible = new List<Sprite>();

        float[] visibleYPositions = new float[] { 0f, -250f, -500f }; //Y POS OF SYMBOLS IN EACH ROW

        foreach (float targetY in visibleYPositions)
        {
            // FIND CLOSEST TO Y TARGET
            Sprite closestSprite = null;
            float closestDistance = float.MaxValue;

            foreach (var symbol in _spawnedSymbols)
            {
                float y = symbol.GetComponent<RectTransform>().anchoredPosition.y;
                float dist = Mathf.Abs(y - targetY);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestSprite = symbol.GetComponent<Image>().sprite;
                }
            }

            visible.Add(closestSprite);
        }

        return visible;
    }
}
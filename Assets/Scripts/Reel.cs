using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    public List<Sprite> symbols;              // Assign via Inspector
    public GameObject symbolPrefab;           // Assign via Inspector (must be a UI Image prefab)
    public float symbolSpacing = 100f;        // Match symbol height
    public int visibleCount = 3;

    private List<GameObject> activeSymbols = new();
    private float topY => 0f;
    private float bottomY => -symbolSpacing * (visibleCount - 1);

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Clear previous symbols
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        activeSymbols.Clear();

        // Create symbols positioned top to bottom
        for (int i = 0; i < visibleCount + 2; i++)
        {
            GameObject symbol = Instantiate(symbolPrefab, transform);
            RectTransform rt = symbol.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, topY - i * symbolSpacing);
            SetRandomSymbol(symbol);
            activeSymbols.Add(symbol);
        }
    }

    public IEnumerator Spin(float duration)
    {
        float elapsed = 0f;
        float speed = 300f;

        while (elapsed < duration)
        {
            foreach (GameObject symbol in activeSymbols)
            {
                RectTransform rt = symbol.GetComponent<RectTransform>();
                Vector2 pos = rt.anchoredPosition;
                pos.y -= speed * Time.deltaTime;

                if (pos.y < bottomY - symbolSpacing)
                {
                    pos.y = topY + symbolSpacing;
                    SetRandomSymbol(symbol);
                }

                rt.anchoredPosition = pos;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        AlignSymbols();
    }

    private void AlignSymbols()
    {
        // Sort top to bottom, snap to fixed positions
        activeSymbols.Sort((a, b) => b.GetComponent<RectTransform>().anchoredPosition.y.CompareTo(
                                      a.GetComponent<RectTransform>().anchoredPosition.y));

        for (int i = 0; i < activeSymbols.Count; i++)
        {
            RectTransform rt = activeSymbols[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, topY - i * symbolSpacing);
        }
    }

    private void SetRandomSymbol(GameObject symbol)
    {
        Sprite sprite = symbols[Random.Range(0, symbols.Count)];
        symbol.GetComponent<Image>().sprite = sprite;
    }

    public List<string> GetVisibleSymbols()
    {
        activeSymbols.Sort((a, b) => b.GetComponent<RectTransform>().anchoredPosition.y.CompareTo(
                                      a.GetComponent<RectTransform>().anchoredPosition.y));

        List<string> result = new();
        for (int i = 0; i < visibleCount; i++)
        {
            result.Add(activeSymbols[i].GetComponent<Image>().sprite.name);
        }
        return result;
    }
}

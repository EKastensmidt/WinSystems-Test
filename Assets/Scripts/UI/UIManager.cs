using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button _spinButton;
    [SerializeField] private TextMeshProUGUI _creditsText;

    [Space(10)]
    [SerializeField] private ReelManager _reelManager;

    private void OnEnable()
    {
        _reelManager.OnSpinCompletion += EnableButton;
        _reelManager.OnCreditsScored += ShowCreditsWon;
        _reelManager.OnSpinStart += HideCreditsText;
    }

    private void OnDisable()
    {
        _reelManager.OnSpinCompletion -= EnableButton;
        _reelManager.OnCreditsScored -= ShowCreditsWon;
        _reelManager.OnSpinStart -= HideCreditsText;
    }

    private void Start()
    {
        _spinButton.onClick.AddListener(ClickSpinButton);

        EnableButton();
        _creditsText.gameObject.SetActive(false);
    }

    private void EnableButton() => _spinButton.interactable = true;
    private void DisableButton() => _spinButton.interactable = false;

    private void ClickSpinButton()
    {
        _reelManager.StartSpinSequence();
        DisableButton();
    }

    private void ShowCreditsWon(int value)
    {
        _creditsText.gameObject.SetActive(true);
        _creditsText.text = $"Credits Won = {value.ToString()}";
    }

    private void HideCreditsText() => _creditsText.gameObject.SetActive(false);
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public static Loader instance;
    
    [SerializeField]
    private Ease easeType = Ease.OutQuad;

    [SerializeField] private float alphaTweenFrom = 0;

    [SerializeField] private float alphaTweenTo = 1.0f;

    private RawImage loaderContainer;

    private TextMeshProUGUI loaderText;

    public bool IsLoading { get; set; } = false;

    private List<TweenerCore<Color, Color, ColorOptions>>
        tweeners = new List<TweenerCore<Color, Color, ColorOptions>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        loaderContainer = GetComponent<RawImage>();
        loaderText = loaderContainer.transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void StartLoader()
    {
        Debug.Log("Starting Loader");

        if (tweeners.Count == 0)
        {
            TweenerCore<Color, Color, ColorOptions> x = loaderContainer.DOFade(alphaTweenFrom, alphaTweenTo).From()
                .SetEase(easeType).SetLoops(-1);
            
            TweenerCore<Color, Color, ColorOptions> y = loaderText.DOFade(alphaTweenFrom, alphaTweenTo).From()
                .SetEase(easeType).SetLoops(-1);
            
            tweeners.Add(x);
            tweeners.Add(y);
        }
        else
        {
            foreach (var t in tweeners)
            {
                t.Restart();
            }
        }

        IsLoading = true;
        gameObject.SetActive(IsLoading);
    }

    public void StopLoader()
    {
        Debug.Log("Stopping Loader");

        foreach (var t in tweeners)
        {
            t.Pause();
        }

        IsLoading = false;
        gameObject.SetActive(IsLoading);
    }
}

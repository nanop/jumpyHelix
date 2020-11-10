﻿using System.Collections;
using UnityEngine;

public class FadingHelixController : MonoBehaviour {

    private Renderer render = null;

    /// <summary>
    /// Start fading this helix
    /// </summary>
    /// <param name="color"></param>
    /// <param name="scaleFactor"></param>
    public void FadingHelix(Color color, float scaleFactor, float fadingTime)
    {
        if (render == null)
            render = GetComponent<Renderer>();
        render.material.color = color;
        transform.localScale = Vector3.one;
        StartCoroutine(Fading(scaleFactor, fadingTime));
    }
    IEnumerator Fading(float scaleFactor, float fadingTime)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(startScale.x * scaleFactor, 0, startScale.z * scaleFactor);
        Color startColor = render.material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        float t = 0;
        while (t < fadingTime)
        {
            t += Time.deltaTime;
            float factor = t / fadingTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, factor);
            render.material.color = Color.Lerp(startColor, endColor, factor);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}

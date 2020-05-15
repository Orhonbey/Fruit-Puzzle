using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScene : MonoBehaviour
{
    #region //----> Variable
    public ParticleSystem confitteExplosion;
    public ParticleSystem sparkWhite;
    public RectTransform levelPassed;
    public GameObject failLevelButton, levelOneButton;
    #endregion
    public void ChocolateCapRotate()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360, 4);
    }
    public void FinishEffectStart(float delay = 0.3f)
    {
        LeanTween.moveLocalY(gameObject, 0.1f, 0.2f).setDelay(delay).setOnComplete(
        m =>
        {
            LeanTween.scaleY(gameObject, 0.7f, 0.2f).setOnComplete(
            p =>
            {
                gameObject.LeanScaleY(1, 0.2f).setOnComplete(f=> { FinishCallback(delay); });
                confitteExplosion.Play();
            });
        });

    }
    private void FinishCallback(float delay)
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360, 0.9f);
        levelPassed.gameObject.SetActive(true);
        LeanTween.scale(levelPassed, Vector3.zero, 0);
        LeanTween.scale(levelPassed, Vector3.one, 1f).setEaseInOutBack().setOnComplete(ButtonCallback);
        LeanTween.moveLocalY(gameObject, -0.118f, 0.6f).setDelay(delay).setOnComplete(
        m =>
        {
            sparkWhite.Play();
        });

    }
    public void ButtonCallback()
    {
        failLevelButton.gameObject.SetActive(true);
        levelOneButton.gameObject.SetActive(true);
    }
}

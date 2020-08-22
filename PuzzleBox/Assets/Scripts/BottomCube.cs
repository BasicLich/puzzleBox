using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottomCube : MonoBehaviour
{
    public ParticleSystem particles;

    public GameObject FlashCube;

    public GameObject Cover;

    private void Start()
    {
        Cover.SetActive(true);
        //Activate();
    }

    public void Activate()
    {
        Cover.gameObject.SetActive(false);

        FlashCube.SetActive(true);
        Material flashMat = FlashCube.GetComponent<Renderer>().material;
        flashMat.DOFloat(0f, "_HeightOffset", 0.4f).OnComplete(() => {
            flashMat.DOFloat(-0.5f, "_HeightOffset", 0.8f).OnComplete(() =>
            {
                FlashCube.SetActive(false);
            });
        });

        particles.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Flower : MonoBehaviour
{
    public Transform Stem;

    private List<Transform> leafes = new List<Transform>();
    private List<Transform> flowers = new List<Transform>();


    private void Awake()
    {
        foreach(Transform child in transform)
        {
            if (child.name.StartsWith("Flower"))
            {
                flowers.Add(child);
            }

            if (child.name.StartsWith("Leaf"))
            {
                leafes.Add(child);
            }
        }
        transform.localScale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Bloom();
    }

   
    public void Bloom()
    {
        transform.DOScale(1.5f, 4f);
        foreach(Transform flower in flowers)
        {
            flower.DOScale(flower.localScale, 4f).SetDelay(2f);
            flower.localScale = Vector3.zero;
        }
        foreach (Transform leaf in leafes)
        {
            leaf.DOScale(leaf.localScale, 4f).SetDelay(1f);
            leaf.localScale = Vector3.zero;
        }
    }
}

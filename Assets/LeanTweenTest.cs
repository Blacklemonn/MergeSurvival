using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("dkdk");
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.move(rectTransform, new Vector3(100, 100, 0), 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

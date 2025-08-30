using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuRepeatBackground : MonoBehaviour
{
    private Vector2 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < startPosition.x - 36.3) 
        {
            transform.position = startPosition;
        }
    }
}

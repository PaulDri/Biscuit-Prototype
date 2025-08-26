using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    private int resourceCheck = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseFire();
    }

    private void MouseFire() 
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked: " + resourceCheck);
            resourceCheck--;
        }
        else if (resourceCheck < 10)
        {
            playerAnim.SetFloat("IsReached", resourceCheck);
        }
        else if(resourceCheck < 5) 
        {
            playerAnim.SetFloat("IsReached", resourceCheck);
        }
    }
}

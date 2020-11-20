using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{

    [SerializeField]
    private SpriteRenderer rend = null;

    [SerializeField]
    private Canvas canvas = null;

    [SerializeField]
    private ParticleSystem salute = null;    

    public bool IsEnable 
    { 
        
        get 
        { 

            return rend.enabled; 

        }
        
        set
        {

            rend.enabled = value;

        }

    }

    void Start()
    {

        transform.localScale = canvas.transform.localScale;

    }

    public void Set(Vector3 position)
    {

        transform.position = position;
        rend.enabled = true;        

    }

    public void Reached()
    {

        rend.enabled = false;
        salute.Play();

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    
    private void OnTriggerEnter(Collider other)
    {
        
        
            BreakOutGameController.Instance().CollisionTrigger(gameObject,other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        BreakOutGameController.Instance().CollisionTrigger(gameObject, other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BreakOutGameController.Instance().CollisionTrigger(gameObject, other.gameObject);
    }
}

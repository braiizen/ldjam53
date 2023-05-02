using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveCollision : MonoBehaviour
{
   [SerializeField] private float multiplier;
   private void OnCollisionEnter(Collision collision)
   {
      if (collision.gameObject.CompareTag("Player"))
      {
         Debug.Log("Boom!");
         var rb = GetComponent<Rigidbody>();
         
         GameManager.instance.PlayCrash();
         GameManager.instance.objectsDestroyed += 1;
         
         rb.constraints = RigidbodyConstraints.None;
         rb.useGravity = true;
         rb.AddExplosionForce(collision.relativeVelocity.magnitude * GameManager.instance.explosiveMultiplier, collision.contacts[0].point, 2);
      }
   }
}

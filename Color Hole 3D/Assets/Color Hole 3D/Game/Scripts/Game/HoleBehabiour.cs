using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehabiour : MonoBehaviour
{
    public static HoleBehabiour instance;

    [Header("Settings")]
    public float absorbForce = 10;

    [Header("References")]
    public GameObject aroundHoleMeshObject;

    private List<Rigidbody> activeCollisions = new List<Rigidbody>();

    private float sqrHoleRadius = 0.5f * 0.5f;

    public static Transform Transform
    {
        get { return instance.transform; }
    }

    public static void EnableAroundHoleMesh()
    {
        instance.aroundHoleMeshObject.SetActive(true);
    }

    public static void DisableAroundHoleMesh()
    {
        instance.aroundHoleMeshObject.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.isKinematic = false;
        activeCollisions.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        int index = activeCollisions.FindIndex(0, r => other.attachedRigidbody.GetInstanceID() == r.GetInstanceID());

        if (index >= 0)
        {
            activeCollisions.RemoveAt(index);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < activeCollisions.Count; i++)
        {
            if (activeCollisions[i].gameObject.activeInHierarchy)
            {
                Vector3 posDelta = transform.position - activeCollisions[i].transform.position;

                Vector3 force;

                if (posDelta.sqrMagnitude < sqrHoleRadius)
                {
                    force = absorbForce * Vector3.down * 5;
                }
                else
                {
                    force = posDelta.normalized * absorbForce * 0.5f;
                }


                activeCollisions[i].AddForce(force * Time.deltaTime);
            }
            else
            {
                activeCollisions.RemoveAt(i);
                i--;
            }
        }
    }

}
#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItemBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Level.Item item;
    public Level.Item Item
    {
        get { return item; }
    }

    [SerializeField]
    private ItemType type;
    public ItemType Type
    {
        get { return type; }
        private set
        {
            type = value;

            UpdateMaterial();
        }
    }

    public Transform Transform
    {
        get { return transformRef; }
    }

    [Header("References")]
    public MeshRenderer meshRenderer;
    public Rigidbody rigidbodyRef;

    public Material itemMaterial;
    public Material obstacleMaterial;

    private Transform transformRef;

    private int defaultLayer;
    private int absorbingLayer;

    public enum ItemType
    {
        Normal = 0,
        Obstacle = 1,
    }

    private void Awake()
    {
        transformRef = transform;

        defaultLayer = LayerMask.NameToLayer("Default");
        absorbingLayer = LayerMask.NameToLayer("Absorbing");
    }

    public void Init(ItemType type)
    {
        Type = type;
        rigidbodyRef.isKinematic = true;
        Activate();
    }

    public void Activate()
    {
        gameObject.layer = absorbingLayer;
    }

    public void Deactivate()
    {
        gameObject.layer = defaultLayer;
    }

    private void Update()
    {
        if (transform.position.y < -1f)
        {
            if (type == ItemType.Normal)
            {
                GameController.OnItemAbsorbed();
            }
            else
            {
                GameController.OnObstacleAbsorbed();
            }

            rigidbodyRef.velocity = Vector3.zero;
            rigidbodyRef.angularVelocity = Vector3.zero;
            rigidbodyRef.isKinematic = true;
            gameObject.SetActive(false);
        }
    }

    public void UpdateMaterial()
    {
        if (type == ItemType.Normal)
        {
            meshRenderer.material = itemMaterial;
        }
        else
        {
            meshRenderer.material = obstacleMaterial;
        }
    }
}

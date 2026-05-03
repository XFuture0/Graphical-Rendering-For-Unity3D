using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFall : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public Sprite itemSprite;
    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BagManager.Instance.AddItem(itemSprite);
            Destroy(gameObject);
        }
    }
}

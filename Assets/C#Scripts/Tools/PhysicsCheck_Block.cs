using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck_Block : MonoBehaviour
{
    public bool IsGround;
    public float offect = 0.1f;
    public float gravity = -9.81f;
    public float distance;
    public float checkRadius = 0.3f;

    private void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
    }

    private void CheckGround()
    {
        IsGround = false;

        float playerHalfHeight = transform.localScale.y / 2;
        float rayLength = playerHalfHeight + offect + 0.5f;

        Vector3[] rayStarts = new Vector3[]
        {
            transform.position + Vector3.up * 0.01f,
            transform.position + Vector3.up * 0.01f + new Vector3(checkRadius, 0, checkRadius),
            transform.position + Vector3.up * 0.01f + new Vector3(-checkRadius, 0, checkRadius),
            transform.position + Vector3.up * 0.01f + new Vector3(checkRadius, 0, -checkRadius),
            transform.position + Vector3.up * 0.01f + new Vector3(-checkRadius, 0, -checkRadius),
        };

        foreach (var start in rayStarts)
        {
            Ray ray = new Ray(start, Vector3.down);
            BlockGraphicsRayCastHit hit = new BlockGraphicsRayCastHit();

            if (GraphicsRayCast.TryBlockGraphicsRayCast(
                ray,
                GraphicsRayCast.GetRayCastPartBlocks(ray, MapManager.Instance.genPerlinNoiseMap.PartBlocks),
                out hit))
            {
                float actualDistance = hit.Distance + 0.01f;
                distance = actualDistance;
                if (actualDistance <= playerHalfHeight + offect)
                {
                    IsGround = true;
                    return;
                }
            }
        }
    }

    private void ApplyGravity()
    {
        if (!IsGround)
        {
            transform.position += Vector3.up * gravity * Time.deltaTime;
        }
    }
}

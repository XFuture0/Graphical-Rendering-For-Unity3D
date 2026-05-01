using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck_Block : MonoBehaviour
{
    public bool IsGround;
    public bool IsHitForward;
    public bool IsHitBack;
    public bool IsHitLeft;
    public bool IsHitRight;
    public float offect = 0.1f;
    public float checkRadius = 0.3f;
    public float wallCheckDistance = 0.3f;

    private void FixedUpdate()
    {
        CheckGround();
        CheckWalls();
    }

    private void CheckGround()
    {
        IsGround = false;

        float playerHalfHeight = transform.localScale.y / 2;

        Vector3[] checkPositions = new Vector3[]
        {
            transform.position,
            transform.position + new Vector3(checkRadius, 0, checkRadius),
            transform.position + new Vector3(-checkRadius, 0, checkRadius),
            transform.position + new Vector3(checkRadius, 0, -checkRadius),
            transform.position + new Vector3(-checkRadius, 0, -checkRadius),
        };

        foreach (var pos in checkPositions)
        {
            Vector3Int blockPos = new Vector3Int(
                Mathf.FloorToInt(pos.x),
                Mathf.FloorToInt(pos.y - playerHalfHeight - offect),
                Mathf.FloorToInt(pos.z)
            );

            if (MapManager.Instance.HasBlockAt(blockPos))
            {
                IsGround = true;
                return;
            }
        }
    }

    private void CheckWalls()
    {
        IsHitForward = false;
        IsHitBack = false;
        IsHitLeft = false;
        IsHitRight = false;

        float playerHalfWidth = transform.localScale.x / 2;
        float checkDistance = playerHalfWidth + wallCheckDistance;
        int playerY = Mathf.FloorToInt(transform.position.y);
        Vector3Int forwardPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x),
            playerY,
            Mathf.FloorToInt(transform.position.z + checkDistance)
        );
        if (MapManager.Instance.HasBlockAt(forwardPos))
        {
            IsHitForward = true;
        }
        Vector3Int backPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x),
            playerY,
            Mathf.FloorToInt(transform.position.z - checkDistance + 1)
        );
        if (MapManager.Instance.HasBlockAt(backPos))
        {
            IsHitBack = true;
        }
        Vector3Int leftPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x - checkDistance + 1),
            playerY,
            Mathf.FloorToInt(transform.position.z)
        );
        if (MapManager.Instance.HasBlockAt(leftPos))
        {
            IsHitLeft = true;
        }
        Vector3Int rightPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x + checkDistance),
            playerY,
            Mathf.FloorToInt(transform.position.z)
        );
        if (MapManager.Instance.HasBlockAt(rightPos))
        {
            IsHitRight = true;
        }
    }
}

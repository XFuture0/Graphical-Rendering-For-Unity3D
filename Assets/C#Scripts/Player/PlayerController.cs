using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpSpeed;
    public float MouseSensitivityX;
    public float MouseSensitivityY;
    private float YRotation;
    private float XRotation;
    private void Update()
    {
        BreakBlock();
        CreateBlock();
    }
    private void FixedUpdate()
    {
        ViewRoll();
        Move();
        Jump();
    }
    private void Move()
    {
        float MoveHorizontal = InputManager.Instance.GetKeyDown_Horizontal();
        float MoveVertical = InputManager.Instance.GetKeyDown_Vertical();
        Vector3 MoveForward = transform.forward;
        Vector3 MoveRight = transform.right;
        MoveForward.y = 0;
        MoveRight.y = 0;
        Vector3 MoveDirection = MoveForward * MoveVertical + MoveRight * MoveHorizontal;
        MoveDirection.Normalize();
        if(MoveDirection != Vector3.zero)
        {
            transform.position += MoveDirection * Speed * Time.deltaTime;
        }
    }
    private void ViewRoll()
    {
        float mouseX = InputManager.Instance.GetKey_MouseX() * MouseSensitivityX * Time.deltaTime;
        float mouseY = InputManager.Instance.GetKey_MouseY() * MouseSensitivityY * Time.deltaTime;
        YRotation += mouseX;
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
    }
    private void Jump()
    {
        if(InputManager.Instance.GetKey_Space())
        {
            transform.position += Vector3.up * JumpSpeed * Time.deltaTime;
        }
    }
    private void BreakBlock()
    {
        if (InputManager.Instance.GetKeyDown_MouseLeft())
        {
            BlockGraphicsRayCastHit hit = new BlockGraphicsRayCastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(GraphicsRayCast.TryBlockGraphicsRayCast(ray, GraphicsRayCast.GetRayCastPartBlocks(ray,MapManager.Instance.genPerlinNoiseMap.PartBlocks), out hit))
            {
                MapManager.Instance.BreakBlocks(hit.Position);
            }
        }
    }
    private void CreateBlock()
    {
        if (InputManager.Instance.GetKeyDown_MouseRight())
        {
            BlockGraphicsRayCastHit hit = new BlockGraphicsRayCastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (GraphicsRayCast.TryBlockGraphicsRayCast(ray, GraphicsRayCast.GetRayCastPartBlocks(ray, MapManager.Instance.genPerlinNoiseMap.PartBlocks), out hit))
            {
                Vector3 blockCenter = hit.Position.GetPosition();
                Vector3 hitPoint = ray.origin + ray.direction * hit.Distance;
                Vector3 localHit = hitPoint - blockCenter;
                Vector3 faceNormal = GetFaceNormal(ray.direction, localHit);
                Vector3 newBlockPos = blockCenter + faceNormal;
                Matrix4x4 newBlock = Matrix4x4.TRS(newBlockPos, Quaternion.identity, Vector3.one);
                MapManager.Instance.CreateBlocks(newBlock);
            }
        }
    }
    private Vector3 GetFaceNormal(Vector3 rayDir, Vector3 localHit)
    {
        localHit.x = Mathf.Clamp(localHit.x, -0.5f, 0.5f);
        localHit.y = Mathf.Clamp(localHit.y, -0.5f, 0.5f);
        localHit.z = Mathf.Clamp(localHit.z, -0.5f, 0.5f);
        float distToPosX = 0.5f - localHit.x;
        float distToNegX = localHit.x + 0.5f;
        float distToPosY = 0.5f - localHit.y;
        float distToNegY = localHit.y + 0.5f;
        float distToPosZ = 0.5f - localHit.z;
        float distToNegZ = localHit.z + 0.5f;
        float minDist = Mathf.Min(distToPosX, distToNegX, distToPosY, distToNegY, distToPosZ, distToNegZ);
        if (minDist == distToPosX) return Vector3.right;
        if (minDist == distToNegX) return Vector3.left;
        if (minDist == distToPosY) return Vector3.up;
        if (minDist == distToNegY) return Vector3.down;
        if (minDist == distToPosZ) return Vector3.forward;
        return Vector3.back;
    }
}

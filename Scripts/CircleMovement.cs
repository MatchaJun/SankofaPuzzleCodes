using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public Transform Target;

    public float CircleRadius = 1;

    public float RotationSpeed = 1;

    public float WalkingSpeed = 3;

    public float ElevationOffset = 0;

    [Range(0, 360)]
    public float StartAngle = 0;

    public bool UseTargetCoordinateSystem = false;

    public bool LookAtTarget = false;

    private float angle;

    public Animator anim;

    public SpriteRenderer spriteRenderer;

    public ParticleSystem walkParticle;

    private void Awake()
    {
       walkParticle.Stop();
       angle = StartAngle;
    }

    private void Update()
    {
        float speedControle = Input.GetAxis("Horizontal") * -WalkingSpeed;

        if(Input.GetAxis("Horizontal") != 0)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("WalkingAnim"))
            {
                walkParticle.Play();
                anim.SetTrigger("Walking");
            }
            RotationSpeed = speedControle;
        }
        else
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("StillAnim"))
            {
                walkParticle.Stop();
                anim.SetTrigger("Stop");
            }
            RotationSpeed = 0;
        }

        if(Input.GetAxis("Horizontal") > 0)
        {
            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
        }else if(Input.GetAxis("Horizontal") < 0)
        {
            if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void LateUpdate()
    {

        // Define the position the object must rotate around
        Vector3 position = Target != null ? Target.position : Vector3.zero;

        Vector3 positionOffset = ComputePositionOffset(angle);

        // Assign new position
        transform.position = position + positionOffset;

        // Rotate object so as to look at the target
        if (LookAtTarget)
            transform.rotation = Quaternion.LookRotation(position - transform.position, Target == null ? Vector3.up : Target.up);

        angle += Time.deltaTime * RotationSpeed;
    }

    private Vector3 ComputePositionOffset(float a)
    {
        a *= Mathf.Deg2Rad;

        // Compute the position of the object
        Vector3 positionOffset = new Vector3(
            Mathf.Cos(a) * CircleRadius,
            ElevationOffset,
            Mathf.Sin(a) * CircleRadius
        );

        // Change position if the object must rotate in the coordinate system of the target
        // (i.e in the local space of the target)
        if (Target != null && UseTargetCoordinateSystem)
            positionOffset = Target.TransformVector(positionOffset);

        return positionOffset;
    }

#if UNITY_EDITOR

    [SerializeField]
    private bool drawGizmos = true;

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        // Draw an arc around the target
        Vector3 position = Target != null ? Target.position : Vector3.zero;
        Vector3 normal = Vector3.up;
        Vector3 forward = Vector3.forward;
        Vector3 labelPosition;

        Vector3 positionOffset = ComputePositionOffset(StartAngle);
        Vector3 verticalOffset;


        if (Target != null && UseTargetCoordinateSystem)
        {
            normal = Target.up;
            forward = Target.forward;
        }
        verticalOffset = positionOffset.y * normal;

        // Draw label to indicate elevation
        if (Mathf.Abs(positionOffset.y) > 0.1)
        {
            UnityEditor.Handles.DrawDottedLine(position, position + verticalOffset, 5);
            labelPosition = position + verticalOffset * 0.5f;
            labelPosition += Vector3.Cross(verticalOffset.normalized, Target != null && UseTargetCoordinateSystem ? Target.forward : Vector3.forward) * 0.25f;
            UnityEditor.Handles.Label(labelPosition, ElevationOffset.ToString("0.00"));
        }

        position += verticalOffset;
        positionOffset -= verticalOffset;

        UnityEditor.Handles.DrawWireArc(position, normal, forward, 360, CircleRadius);

        // Draw label to indicate radius
        UnityEditor.Handles.DrawLine(position, position + positionOffset);
        labelPosition = position + positionOffset * 0.5f;
        labelPosition += Vector3.Cross(positionOffset.normalized, Target != null && UseTargetCoordinateSystem ? Target.up : Vector3.up) * 0.25f;
        UnityEditor.Handles.Label(labelPosition, CircleRadius.ToString("0.00"));
    }

#endif
}

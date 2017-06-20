/*
 * Copyright (C) 2017 3ivr. All rights reserved.
 *
 * Author: Lucas(Wu Pengcheng)
 * Date  : 2017/06/19 08:08
 */

using UnityEngine;
using System.Collections;

/// Implementation of II3vrPointer for a laser pointer visual.
/// This script should be attached to the controller object.
/// The laser visual is important to help users locate their cursor
/// when its not directly in their field of view.
[RequireComponent(typeof(LineRenderer))]
public class I3vrLaserPointer : I3vrBasePointer
{
    /// Small offset to prevent z-fighting of the reticle (meters).
    private const float Z_OFFSET_EPSILON = 0.1f;

    /// Size of the reticle in meters as seen from 1 meter.
    private const float RETICLE_SIZE = 0.01f;

    private LineRenderer lineRenderer;
    private bool isPointerIntersecting;
    private Vector3 pointerIntersection;
    private Ray pointerIntersectionRay;

    /// Color of the laser pointer including alpha transparency
    public Color laserColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    /// Maximum distance of the pointer (meters).
    [Range(0.0f, 10.0f)]
    public float maxLaserDistance = 0.75f;

    /// Maximum distance of the reticle (meters).
    [Range(0.4f, 10.0f)]
    public float maxReticleDistance = 2.5f;

    public GameObject reticle;

    void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void LateUpdate()
    {
        // Set the reticle's position and scale
        if (reticle != null)
        {
            if (isPointerIntersecting)
            {
                Vector3 difference = pointerIntersection - pointerIntersectionRay.origin;
                Vector3 clampedDifference = Vector3.ClampMagnitude(difference, maxReticleDistance);
                Vector3 clampedPosition = pointerIntersectionRay.origin + clampedDifference;
                reticle.transform.position = clampedPosition;
            }
            else
            {
                reticle.transform.localPosition = new Vector3(0, 0, maxReticleDistance);
            }

            float reticleDistanceFromCamera = (reticle.transform.position - Camera.main.transform.position).magnitude;
            float scale = RETICLE_SIZE * reticleDistanceFromCamera;
            reticle.transform.localScale = new Vector3(scale, scale, scale);
        }

        // Set the line renderer positions.
        lineRenderer.SetPosition(0, transform.position);
        Vector3 lineEndPoint =
          isPointerIntersecting && Vector3.Distance(transform.position, pointerIntersection) < maxLaserDistance ?
          pointerIntersection :
          transform.position + (transform.forward * maxLaserDistance);
        lineRenderer.SetPosition(1, lineEndPoint);

        // Adjust transparency
        float alpha = I3vrArmModel.Instance.alphaValue;
        lineRenderer.SetColors(Color.Lerp(Color.clear, laserColor, alpha), Color.clear);
    }

    public override void OnInputModuleEnabled()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    public override void OnInputModuleDisabled()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    public override void OnPointerEnter(GameObject targetObject, Vector3 intersectionPosition,
        Ray intersectionRay, bool isInteractive)
    {
        pointerIntersection = intersectionPosition;
        pointerIntersectionRay = intersectionRay;
        isPointerIntersecting = true;
    }

    public override void OnPointerHover(GameObject targetObject, Vector3 intersectionPosition,
        Ray intersectionRay, bool isInteractive)
    {
        pointerIntersection = intersectionPosition;
        pointerIntersectionRay = intersectionRay;
    }

    public override void OnPointerExit(GameObject targetObject)
    {
        pointerIntersection = Vector3.zero;
        pointerIntersectionRay = new Ray();
        isPointerIntersecting = false;
    }

    public override void OnPointerClickDown()
    {
        // User has performed a click on the target.  In a derived class, you could
        // handle visual feedback such as laser or cursor color changes here.
    }

    public override void OnPointerClickUp()
    {
        // User has released a click from the target.  In a derived class, you could
        // handle visual feedback such as laser or cursor color changes here.
    }

    public override float GetMaxPointerDistance()
    {
        return maxReticleDistance;
    }

    public override void GetPointerRadius(out float enterRadius, out float exitRadius)
    {
        if (reticle != null)
        {
            float reticleScale = reticle.transform.localScale.x;

            // Fixed size for enter radius to avoid flickering.
            // This will cause some slight variability based on the distance of the object
            // from the camera, and is optimized for the average case.
            enterRadius = RETICLE_SIZE * 0.5f;

            // Dynamic size for exit radius.
            // Always correct because we know the intersection point of the object and can
            // therefore use the correct radius based on the object's distance from the camera.
            exitRadius = reticleScale;
        }
        else
        {
            enterRadius = 0.0f;
            exitRadius = 0.0f;
        }
    }
}

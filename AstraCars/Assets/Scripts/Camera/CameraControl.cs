using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Objeto que la cámara debe seguir
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float smoothTime;

    [SerializeField]
    private Vector3 offset;

    private Vector3 velocity = Vector3.zero;


    private void LateUpdate()
    {

        // 0, 15, -8
        // 0, 8, -12

        if (target == null) return;

        // Calcula la posición objetivo de la cámara en función del offset
        float altura = offset.y;
        float desplazamientoAdelante = offset.z;
        Vector3 forwardOffset = Vector3.forward * desplazamientoAdelante;
        Vector3 targetPosition = target.position + new Vector3(0, altura, 0) + forwardOffset;

        // Suaviza el movimiento de la cámara hacia la posición objetivo
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition, ref velocity, smoothTime);
        // Fija la rotación de la cámara (vista cenital)
        cameraTransform.rotation = Quaternion.Euler(90f, 0f, 0f);



    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

using UnityEngine;
using Cinemachine;
using static UnityEngine.UI.Image;
using System;
using UnityEngine.InputSystem.XR;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows;
using UnityEngine.UIElements;
using System.Security.Cryptography;

public class ControladorjugadorVR : Controladorjugador
{
    private float xRotation = 0f; // Rotación en el eje X (vertical)

    void PlayerLook()
        {
            // Obtener la rotación de la cabeza (VR)
            Quaternion headRotation = Camera.main.transform.rotation;
            Vector3 eulerRotation = headRotation.eulerAngles;

            float lookX = eulerRotation.y; // Rotación horizontal (Yaw)
            float lookY = eulerRotation.x; // Rotación vertical (Pitch)

            // Ajustar valores para evitar problemas con ángulos grandes
            if (lookY > 180f) lookY -= 360f;
            if (lookX > 180f) lookX -= 360f;

            // Aplicar rotación vertical solo a la cámara
            xRotation = Mathf.Clamp(-lookY, -80f, 56f);
            Quaternion verticalRotation = Quaternion.Euler(xRotation, 0f, 0f);
            virtualCamera.transform.localRotation = verticalRotation;
            Cabeza.transform.localRotation = verticalRotation;

            // Aplicar rotación horizontal al cuerpo
            playerBody.rotation = Quaternion.Euler(0f, lookX, 0f);
        }
    


}

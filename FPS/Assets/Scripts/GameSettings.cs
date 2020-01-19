using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static float rotSpeedX = 100;
    public static float rotSpeedY = 100;
    public static float rotDamp = 0.5f;
    public static float walkSpeed = 5;
    public static float runSpeed = 15;
    public static bool attack = false;
    public static KeyCode runKey = KeyCode.A;
    public static float shootVelocity = 0;
    public static int lineSegment = 10;
}

﻿using System.Collections;
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
    public static bool catchChicken = false;
    public static Vector3 p1StartPos = new Vector3(5f, 0.2f, -60f);
    public static Vector3 p2StartPos = new Vector3(0f, 0.2f, -60f);
    public static Quaternion p1StartRot = new Quaternion(0, 0, 0, 0);
    public static Quaternion p2StartRot = new Quaternion(0, 0, 0, 0);
}

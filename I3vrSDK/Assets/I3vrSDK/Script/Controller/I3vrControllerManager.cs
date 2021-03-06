﻿/*
 * Copyright (C) 2017 3ivr. All rights reserved.
 *
 * Author: Lucas(Wu Pengcheng)
 * Date  : 2017/07/19 14:25
 */

using UnityEngine;
using UnityEngine.Events;
using i3vr;
public enum ControllerType
{
    Single,
    LeftAndRight,
}

public enum DataSource
{
    Right,
    Left,
}

public class I3vrControllerManager : MonoBehaviour
{  
    private static I3vrController _rightHand, _leftHand;
    private static ControllerType _controllerNumb;

    public ControllerType SetControllerType = ControllerType.Single;
    public UnityEvent BothControllerEvent;

    public static ControllerType I3vrControllerNumb
    {
        get
        {
            return _controllerNumb;
        }
    }

    public static I3vrController I3vrRightController
    {
        get
        {
            return _rightHand;
        }
    }

    public static I3vrController I3vrLeftController
    {
        get
        {
            return _leftHand;
        }
    }

    void Awake()
    {
        _controllerNumb = SetControllerType;
    }
    
    private void Start()
    {
        _rightHand = GameObject.FindWithTag("RightController").GetComponent<I3vrController>();
        if (I3vrControllerNumb == ControllerType.LeftAndRight)
        {
            BothControllerEvent.Invoke();
            _leftHand = GameObject.FindWithTag("LeftController").GetComponent<I3vrController>();
        }
    }

    private void OnApplicationQuit()
    {
        if (I3vrControllerNumb == ControllerType.LeftAndRight)
        {
            AndroidDoubleServiceProvider.BleDestroy();
        }
        else AndroidServiceProvider.BleDestroy();
    }
}

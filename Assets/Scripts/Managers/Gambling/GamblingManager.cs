using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GamblingManager : MonoBehaviour
{
    public static GamblingManager instance;

    private void Awake()
    {
        instance = this;
    }
}

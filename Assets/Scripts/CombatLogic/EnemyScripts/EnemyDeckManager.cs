using System.Collections.Generic;
using UnityEngine;
using MapSystem;

public class EnemyDeckManager : MonoBehaviour
{
    [SerializeField] private List<ClassForDictionary<NodeType, List<ClassForDictionary<string, CardsSOTest>>>> decksForDifferentTypes;
}

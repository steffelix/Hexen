using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem.Views
{
    public class BackDropView : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData pointerEventData) => GameLoop.Instance.OnAbilityReleasedEmpty();
    }

}
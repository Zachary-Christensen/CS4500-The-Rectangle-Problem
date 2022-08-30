using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownScaleController : MonoBehaviour
{
    public Dropdown dropdownScale; // UI object from canvas

    public bool IsNotLastOption => dropdownScale.value != dropdownScale.options.Count - 1;
    public bool IsNotFirstOption => dropdownScale.value != 0;
    public bool Increment()
    {
        if (IsNotLastOption)
        {
            dropdownScale.value++;
            return true;
        }
        return false;
    }

    public bool Decrement()
    {
        if (IsNotFirstOption)
        {
            dropdownScale.value--;
            return true;
        }
        return false;
    }

    public void SetValue(int value)
    {
        dropdownScale.value = value;
    }
}

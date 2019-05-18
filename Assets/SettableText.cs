using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SettableText : MonoBehaviour {
    public string formatString = "{0:#0.0000}";

    public void SetNumericText(float number) {
        GetComponent<Text>().text = string.Format(new CultureInfo("en-US"), formatString, number);
    }
    public void SetNumericText(double number) {
        GetComponent<Text>().text = string.Format(new CultureInfo("en-US"), formatString, number);
    }
}

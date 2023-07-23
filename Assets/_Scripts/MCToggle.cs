using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class MCToggle : MonoBehaviour {
    public Toggle toggle;
    public TMP_Text toggle_Text;

    public Choice choice;
    Color[] stateColor = new Color[2] { new Color(0, 255, 0), Color.black };

    public void onPress() {
        if ( toggle.isOn ) {
            toggle_Text.color = stateColor[1];
        } else {
            toggle_Text.color = stateColor[0];
        }
    }

    public MCToggle initialize(Choice choi) {
        choice = choi;
        toggle_Text.text = choi.sentence;
        return this;
    }

    public bool doesInputEqualChoice() {
        return toggle.isOn == choice.isCorrect;
    }
}

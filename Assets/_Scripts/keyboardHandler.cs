using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class keyboardHandler : MonoBehaviour {
    [SerializeField] GameObject	prefab_Key;

    [SerializeField] GameObject board;
    [SerializeField] GameObject keysHolder;
    [SerializeField] TMP_Text inputField;
    [SerializeField] TMP_Text controlField;

    [SerializeField] GameObject[] keys;
    [SerializeField] string inputText;

    public KeyboardMode currentKeyMode = KeyboardMode.numbers;

    Dictionary<KeyboardMode, string[]> keyAlphabet = new Dictionary<KeyboardMode, string[]>(3) {
        {KeyboardMode.numbers,  new string[2+10]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "-", ","} },
        {KeyboardMode.hex,      new string[16]  {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"} },
        {KeyboardMode.bin,      new string[2]   {"0", "1"} }
    };

    void instantiateKeys(KeyboardMode mode) {
        string[] alphabet = keyAlphabet[mode];
        keys = new GameObject[alphabet.Length];

        for( int i=0; i<alphabet.Length; i++) {
            keys[i] = setupKey( alphabet[i] );
        }
    }

    void destantiateKeys() {
        foreach(GameObject key in keys)
            Destroy(key);
        keys = new GameObject[0];
    }

    GameObject setupKey(string name) {
        GameObject key = Instantiate(prefab_Key, keysHolder.transform);
        
        TMP_Text txt = key.GetComponentInChildren<TMP_Text>();
        txt.text = name;
        
        Button button = key.GetComponent<Button>();
        button.onClick.AddListener( () => keyPress(name) );

        return key;
    }

    void updateTexts() {
        inputField.text   = inputText;
        controlField.text = inputText;
    }

    public void open() {
        if ( board.activeInHierarchy ) return;
        board.SetActive(true);
        instantiateKeys(currentKeyMode);
    }

    public void close() {
        board.SetActive(false);
        destantiateKeys();
    }

    public void backSpace() {
        if ( inputText.Length <= 0 ) return;
        inputText = inputText.Remove(inputText.Length-1);
        updateTexts();
    }

    public void keyPress(string name) {
        inputText += name;
        updateTexts();
    }

    public void resetTexts(string txt="Enter your answer here") {
        inputField.text = txt;
        controlField.text = txt;
        inputText = "";
    }
}

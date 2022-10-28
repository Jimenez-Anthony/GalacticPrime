using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeybindSelector : MonoBehaviour
{
    public string key;
    private Button button;
    private bool selecting;

    void Start()
    {
        button = GetComponent<Button>();
        //string str = GetKey(key).ToString();
        transform.GetChild(0).GetComponent<TMP_Text>().text = StringFromKey(GetKey(key));
    }

    KeyCode GetKey(string value) {
        KeyCode key = (KeyCode)typeof(Keys).GetField(value).GetValue(null);
        return key;
    }

    string StringFromKey(KeyCode key) {
        string output;
        if (key == KeyCode.LeftShift) {
            output = "Shift";
        }
        else if (key == KeyCode.LeftControl) {
            output = "Control";
        }
        else {
            output = key.ToString();
        }
        return output;
    }

    // Update is called once per frame
    void Update()
    {
        if (selecting) {
            button.interactable = false;
            if (Input.anyKeyDown) {
                foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
                    if (Input.GetKeyDown(kcode)) {
                        ChangeKey(kcode);
                        selecting = false;
                    }
                }
            }
        }
        else {
            button.interactable = true;
        }
    }

    void ChangeKey(KeyCode newKey) {
        typeof(Keys).GetField(key).SetValue(null, newKey);
        transform.GetChild(0).GetComponent<TMP_Text>().text = StringFromKey(GetKey(key));
        PlayerPrefs.SetString(key, newKey.ToString());
    }

    public void ChangeKey() {
        selecting = true;
    }
}

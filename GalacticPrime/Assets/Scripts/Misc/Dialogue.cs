using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    public bool playOnStart = false;
    [TextArea(3, 6)]
    public string[] sentences;
}

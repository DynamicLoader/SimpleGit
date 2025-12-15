using UnityEngine;
using UnityEditor;

public class StatusPanel
{
    private string message = "";
    private Color messageColor = Color.white;

    public void SetStatus(string msg, Color col)
    {
        message = msg;
        messageColor = col;
    }

    public void Draw()
    {
        if (!string.IsNullOrEmpty(message))
        {
            var prevColor = GUI.contentColor;
            var prevFontSize = GUI.skin.label.fontSize;
            
            GUI.contentColor = messageColor;
            GUI.skin.label.fontSize = 14;
            
            EditorGUILayout.HelpBox(message, MessageType.None);
            
            GUI.contentColor = prevColor;
            GUI.skin.label.fontSize = prevFontSize;
        }
    }
}

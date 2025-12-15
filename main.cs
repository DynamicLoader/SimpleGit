using UnityEngine;
using UnityEditor;

public class SimpleGitWindow : EditorWindow
{
    private UserConfigPanel userConfigPanel;
    private StatusPanel statusPanel;
    private CommitHistoryPanel commitHistoryPanel;

    [MenuItem("Window/Simple Git")]
    public static void ShowWindow()
    {
        GetWindow<SimpleGitWindow>("Simple Git");
    }

    void OnEnable()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        statusPanel = new StatusPanel();
        userConfigPanel = new UserConfigPanel();
        userConfigPanel.OnStatusChanged += OnStatusChanged;
        userConfigPanel.Initialize();

        commitHistoryPanel = new CommitHistoryPanel();
        commitHistoryPanel.Initialize();
    }

    private void OnStatusChanged(string message, Color color)
    {
        statusPanel.SetStatus(message, color);
        Repaint();
    }

    void OnGUI()
    {
        DrawHeader();
        DrawPanels();
    }

    private void DrawHeader()
    {
        GUILayout.Label("Git User Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();
    }

    private void DrawPanels()
    {
        if (statusPanel != null)
        {
            statusPanel.Draw();
            EditorGUILayout.Space();
        }

        if (userConfigPanel != null)
        {
            userConfigPanel.Draw();
            EditorGUILayout.Space();
        }

        if (commitHistoryPanel != null)
        {
            commitHistoryPanel.Draw();
        }
    }
}

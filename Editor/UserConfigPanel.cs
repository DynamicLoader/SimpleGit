using System;
using UnityEngine;
using UnityEditor;

public class UserConfigPanel
{
    private string userName = "";
    private string userEmail = "";
    private bool foldout = true;

    public event Action<string, Color> OnStatusChanged;

    public UserConfigPanel()
    {
    }

    public void Initialize()
    {
        LoadFromGit();
    }

    public void Draw()
    {
        DrawHeader();
        DrawContent();
    }

    private void DrawHeader()
    {
        foldout = EditorGUILayout.Foldout(foldout, "User Configuration", true, EditorStyles.foldout);
    }

    private void DrawContent()
    {
        if (foldout)
        {
            EditorGUILayout.BeginVertical("box");
            
            userName = EditorGUILayout.TextField("User Name", userName);
            userEmail = EditorGUILayout.TextField("User Email", userEmail);
            
            DrawActionButtons();
            
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Refresh", GUILayout.Height(30)))
        {
            LoadFromGit();
        }
        
        if (GUILayout.Button("Save", GUILayout.Height(30)))
        {
            SaveToGit();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void LoadFromGit()
    {
        try
        {
            userName = GetGitConfig("user.name");
            userEmail = GetGitConfig("user.email");
            OnStatusChanged?.Invoke("Refreshed from git config", Color.green);
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke("Error refreshing: " + ex.Message, Color.red);
        }
    }

    private void SaveToGit()
    {
        try
        {
            bool ok1 = SetGitConfig("user.name", userName);
            bool ok2 = SetGitConfig("user.email", userEmail);
            if (ok1 && ok2)
            {
                OnStatusChanged?.Invoke("Saved git user.name and user.email", Color.green);
            }
            else
            {
                OnStatusChanged?.Invoke("Failed to save one or more values", Color.red);
            }
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke("Error saving: " + ex.Message, Color.red);
        }
    }

    private string GetGitConfig(string key)
    {
        var res = GitUtil.RunGit($"config --global --get {key}");
        if (res.ExitCode == 0) return res.Output.Trim();
        return "";
    }

    private bool SetGitConfig(string key, string value)
    {
        if (value == null) value = "";
        var res = GitUtil.RunGit($"config --global --replace-all {key} \"{value}\"");
        return res.ExitCode == 0;
    }
}

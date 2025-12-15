using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CommitHistoryPanel
{
    private List<CommitInfo> commits = new List<CommitInfo>();
    private bool foldout = true;
    private Vector2 scrollPosition = Vector2.zero;
    private const int MaxCommitsDisplay = 10;

    public class CommitInfo
    {
        public string Hash { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
    }

    public void Initialize()
    {
        LoadCommitHistory();
    }

    public void Draw()
    {
        DrawHeader();
        DrawContent();
    }

    private void DrawHeader()
    {
        foldout = EditorGUILayout.Foldout(foldout, $"Commit History ({commits.Count})", true, EditorStyles.foldout);
    }

    private void DrawContent()
    {
        if (!foldout)
            return;

        EditorGUILayout.BeginVertical("box");

        if (commits.Count == 0)
        {
            EditorGUILayout.HelpBox("No commits found or failed to load history", MessageType.Info);
        }
        else
        {
            DrawCommitList();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh History", GUILayout.Height(25)))
        {
            LoadCommitHistory();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawCommitList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        int displayCount = Mathf.Min(commits.Count, MaxCommitsDisplay);
        for (int i = 0; i < displayCount; i++)
        {
            DrawCommitItem(commits[i], i);
        }

        if (commits.Count > MaxCommitsDisplay)
        {
            EditorGUILayout.HelpBox($"Showing {MaxCommitsDisplay} of {commits.Count} commits", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawCommitItem(CommitInfo commit, int index)
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label($"[{index + 1}] {commit.Hash.Substring(0, 7)}", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Author", commit.Author, GUILayout.Height(18));
        EditorGUILayout.TextField("Date", commit.Date, GUILayout.Height(18));
        EditorGUILayout.TextArea(commit.Message, GUILayout.Height(40));

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    private void LoadCommitHistory()
    {
        commits.Clear();
        try
        {
            // Format: hash%n author%n date%n message%n---END---
            string format = "%H%n%an%n%ai%n%s%n---END---";
            var res = GitUtil.RunGit($"log --pretty=format:\"{format}\" -20");

            if (res.ExitCode == 0)
            {
                ParseCommits(res.Output);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading commit history: {ex.Message}");
        }
    }

    private void ParseCommits(string output)
    {
        string[] entries = output.Split(new[] { "---END---" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string entry in entries)
        {
            string[] lines = entry.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 4)
            {
                commits.Add(new CommitInfo
                {
                    Hash = lines[0].Trim(),
                    Author = lines[1].Trim(),
                    Date = lines[2].Trim(),
                    Message = lines[3].Trim()
                });
            }
        }
    }
}

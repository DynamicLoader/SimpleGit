using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CommitHistoryPanel
{
    private List<CommitInfo> commits = new List<CommitInfo>();
    private Dictionary<int, bool> commitFoldouts = new Dictionary<int, bool>();
    private bool foldout = true;
    private Vector2 scrollPosition = Vector2.zero;
    private const int MaxCommitsDisplay = 10;

    public class CommitInfo
    {
        public string Hash { get; set; }
        public string ShortHash => Hash.Length > 7 ? Hash.Substring(0, 7) : Hash;
        public string Author { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public string FullMessage { get; set; }
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
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

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
        if (!commitFoldouts.ContainsKey(index))
        {
            commitFoldouts[index] = false;
        }

        string foldoutTitle = $"{commit.ShortHash} - {commit.Message}";
        commitFoldouts[index] = EditorGUILayout.Foldout(commitFoldouts[index], foldoutTitle, true, EditorStyles.foldout);

        if (commitFoldouts[index])
        {
            EditorGUILayout.BeginVertical("box");

            // Hash with copy button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hash", commit.Hash, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                EditorGUIUtility.systemCopyBuffer = commit.Hash;
                LogUtil.Log($"Copied hash: {commit.Hash}");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Author", $"{commit.Author} <{commit.Email}>");
            EditorGUILayout.LabelField("Date", commit.Date);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Message", commit.FullMessage, GUILayout.ExpandWidth(true));
            // EditorGUILayout.SelectableLabel();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(2);
    }

    private void LoadCommitHistory()
    {
        commits.Clear();
        commitFoldouts.Clear();
        try
        {
            // Format: hash%n author%n email%n date%n subject%n body%n---END---
            string format = "%H%n%an%n%ae%n%ai%n%s%n%b%n---END---";
            var res = GitUtil.RunGit($"log --pretty=format:\"{format}\" -20");

            if (res.ExitCode == 0)
            {
                ParseCommits(res.Output);
            }
        }
        catch (Exception ex)
        {
            LogUtil.LogError($"Error loading commit history: {ex.Message}");
        }
    }

    private void ParseCommits(string output)
    {
        string[] entries = output.Split(new[] { "---END---" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string entry in entries)
        {
            string[] lines = entry.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 5)
            {
                string hash = lines[0].Trim();
                string author = lines[1].Trim();
                string email = lines[2].Trim();
                string date = lines[3].Trim();
                string message = lines[4].Trim();

                // Combine remaining lines as full message
                string fullMessage = string.Join("\n", lines, 4, lines.Length - 4).Trim();

                commits.Add(new CommitInfo
                {
                    Hash = hash,
                    Author = author,
                    Email = email,
                    Date = date,
                    Message = message,
                    FullMessage = fullMessage
                });
            }
        }
    }
}

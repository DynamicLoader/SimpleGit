using System;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;


public class LogUtil
{

    public static void Log(string message)
    {
        UnityEngine.Debug.Log($"[SimpleGit] {message}");
    }

    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError($"[SimpleGit] {message}");
    }
}

public class GitUtil
{
    public class GitResult
    {
        public int ExitCode;
        public string Output = "";
        public string Error = "";
    }

    public static GitResult RunGit(string args)
    {
        LogUtil.Log($"Running git {args}");
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var result = new GitResult();
        try
        {
            using (var p = new Process())
            {
                p.StartInfo = psi;
                p.Start();
                result.Output = p.StandardOutput.ReadToEnd();
                result.Error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                result.ExitCode = p.ExitCode;
            }
        }
        catch (Exception e)
        {
            result.ExitCode = -1;
            result.Error = e.Message;
        }

        return result;
    }
}

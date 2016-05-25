/**************************
 * 文件名:UZip.cs;
 * 文件描述:解压或压缩任;
 * 创建日期:2016/05/25;
 * Author:Create by ThisisGame;
 * Page:https://github.com/ThisisGame/UZip
 ***************************/

using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using System;
using System.Threading;

namespace ThisisGame.UZip
{
    public class UZip
    {

        private UZipTask task = null;

        private Thread thread = null;

        int processed = 0;//压缩完成的;
        int totalCount = 0; //需要压缩的文件数量;

        public void Compress(string sourceDirectory, string zipFileName,string password, Action<string, string, string, float> OnProgress, Action<string,string> OnComplete, Action<string,string, Exception> OnError)
        {
            //新建任务;
            task = new UZipTask();
            task.status = UZipTask.TaskStatus.None;
            task.srcPath = sourceDirectory;
            task.targetPath = zipFileName;
            task.OnProgress = OnProgress;
            task.OnComplete = OnComplete;
            task.OnError = OnError;

            //开启子线程
            thread = new Thread(() =>
            {
                try
                {
                    totalCount = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories).Length;
                    Debug.Log("totalCount="+ totalCount);


                    FastZipEvents events = new FastZipEvents();
                    events.ProcessFile = this.OnFileProgress;
                    events.ProcessDirectory = this.OnDirectoryProgress;
                    events.FileFailure = this.OnFileFailed;
                    events.DirectoryFailure = this.OnDirectoryFailed;
                    events.CompletedFile = this.OnFileCompleted;

                    FastZip zipHelper = new FastZip(events);
                    zipHelper.Password = password;
                    zipHelper.CreateEmptyDirectories = true;
                    zipHelper.CreateZip(zipFileName, sourceDirectory, true, "");

                    task.status = UZipTask.TaskStatus.Done;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Compress Thread Error: " + ex);
                    task.status = UZipTask.TaskStatus.Error;
                    task.exception = ex;
                }
            });
            thread.Start();
        }

        public void DeCompress(string zipFileName, string targetDirectory, string password, Action<string, string, string, float> OnProgress, Action<string, string> OnComplete, Action<string, string, Exception> OnError)
        {
            //新建任务;
            task = new UZipTask();
            task.status = UZipTask.TaskStatus.None;
            task.srcPath = zipFileName;
            task.targetPath = targetDirectory;
            task.OnProgress = OnProgress;
            task.OnComplete = OnComplete;
            task.OnError = OnError;

            //开启子线程
            thread = new Thread(() =>
            {
                try
                {
                    ZipFile fileInfo = new ZipFile(zipFileName);
                    totalCount = (int)fileInfo.Count;
                    Debug.Log("totalCount=" + totalCount);

                    FastZipEvents events = new FastZipEvents();
                    events.ProcessFile = this.OnFileProgress;
                    events.Progress = this.OnProgress;
                    events.ProcessDirectory = this.OnDirectoryProgress;
                    events.FileFailure = this.OnFileFailed;
                    events.DirectoryFailure = this.OnDirectoryFailed;
                    events.CompletedFile =this.OnFileCompleted ;

                    FastZip zipHelper = new FastZip(events);

                    zipHelper.Password = password;
                    zipHelper.ExtractZip(zipFileName, targetDirectory, "");
                    task.status = UZipTask.TaskStatus.Done;
                }
                catch (Exception ex)
                {
                    Debug.LogError("DeCompress Thread Error: " + ex);
                    task.status = UZipTask.TaskStatus.Error;
                    task.exception = ex;
                }
            });
            thread.Start();
        }

        public void Update()
        {
            if(task!=null)
            {
                switch(task.status)
                {
                    case UZipTask.TaskStatus.None:
                        {

                        }
                        break;
                    case UZipTask.TaskStatus.Working:
                        {
                            task.OnProgress(task.srcPath,task.targetPath,task.processfile,task.percent);
                        }
                        break;
                    case UZipTask.TaskStatus.Done:
                        {
                            task.OnComplete(task.srcPath,task.targetPath);
                            task.status = UZipTask.TaskStatus.None;
                        }
                        break;
                    case UZipTask.TaskStatus.Error:
                        {
                            task.OnError(task.srcPath, task.targetPath,task.exception);
                            task.status = UZipTask.TaskStatus.None;
                        }
                        break;
                }
            }
        }

        private void OnFileProgress(object sender, ScanEventArgs args)
        {
            //Debug.Log("OnFileProgress "+args.Name);
            //Debug.Log("OnFileProgress " + args.ContinueRunning);

            task.status = UZipTask.TaskStatus.Working;
            task.processfile = args.Name;
        }

        private void OnDirectoryProgress(object sender, ScanEventArgs args)
        {
            //Debug.Log("OnDirectoryProgress "+args.Name);
            //Debug.Log("OnDirectoryProgress " + args.ContinueRunning);

            task.status = UZipTask.TaskStatus.Working;
            task.processfile = args.Name;
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            //Debug.Log("OnProgress "+e.Name);
            //Debug.Log("OnProgress " + e.ContinueRunning);
            
            task.status = UZipTask.TaskStatus.Working;
            task.processfile = e.Name;
            task.percent = e.PercentComplete;
        }

        private void OnFileCompleted(object sender, ScanEventArgs e)
        {
            //Debug.Log("OnFileCompleted "+e.Name);
            //Debug.Log("OnFileCompleted " + e.ContinueRunning);

            processed++;

            task.status = UZipTask.TaskStatus.Working;
            task.processfile = e.Name;
            task.percent = (float)processed / totalCount;
        }

        private void OnFileFailed(object sender, ScanFailureEventArgs e)
        {
            //Debug.Log("OnFileFailed " + e.Name);
            //Debug.Log("OnFileFailed " + e.ContinueRunning);
            //Debug.Log("OnFileFailed " + e.Exception);

            task.status = UZipTask.TaskStatus.Error;
            task.processfile = e.Name;
            task.exception = e.Exception;
        }

        private void OnDirectoryFailed(object sender, ScanFailureEventArgs e)
        {
            //Debug.Log("OnDirectoryFailed " + e.Name);
            //Debug.Log("OnDirectoryFailed " + e.ContinueRunning);
            //Debug.Log("OnDirectoryFailed " + e.Exception);

            task.status = UZipTask.TaskStatus.Error;
            task.processfile = e.Name;
            task.exception = e.Exception;
        }
    }
}

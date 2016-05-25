/**************************
 * 文件名:UZipTask.cs;
 * 文件描述:解压或压缩任务;
 * 创建日期:2016/05/25;
 * Author:Create by ThisisGame;
 * Page:https://github.com/ThisisGame/UZip
 ***************************/


using System;

namespace ThisisGame.UZip
{
    public class UZipTask
    {
        public enum TaskStatus
        {
            None,
            Working,
            Done,
            Error
        }

        public string srcPath = string.Empty;
        public string targetPath = string.Empty;

        public TaskStatus status = TaskStatus.None;


        //进度回调 源目录 -- 目标目录 -- 当前处理文件目录 --  进度 
        public System.Action<string,string,string,float> OnProgress = null;

        //解压或者压缩完成的回调 源目录 -- 目标目录
        public System.Action<string,string> OnComplete = null;

        //解压或者压缩失败的回调 源目录 -- 目标目录 -- 异常
        public System.Action<string,string,Exception> OnError = null;

        //进度;
        public float percent = 0;

        //当前处理文件;
        public string processfile = string.Empty;

        //异常
        public System.Exception exception = null;
    }

}

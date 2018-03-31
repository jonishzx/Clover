using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;

namespace Yankee.Tech.Udf.IO
{

    /// <summary>
    /// 文件监视类，利用FileSystemWatcher来实现，并修复了FileSystemWatcher会重复触发事件的问题。
    /// </summary>
    public class UDFFileSystemWatcher
    {
        FileSystemWatcher _fsWatcher;
        InvokeMethod _executeChanged;
        TAction<FileSystemEventArgs> _onChangedCallbackMethod;

        /// <summary>
        /// 文件监视器构造函数
        /// </summary>
        /// <param name="path">要监控的目录路径</param>
        /// <param name="filter">要过滤的文件，可以是具体的文件名或搜索条件如：a.txt或*.txt</param>
        /// <param name="notifyFilters">NotifyFilters 枚举，需多个时用 | 分开</param>
        /// <param name="OnChangedCallbackMethod">当监测的文件发生改变时触发的回调函数，接收一个FileSystemEventArgs类型的参数</param>
        public UDFFileSystemWatcher(string path, string filter, NotifyFilters notifyFilters, TAction<FileSystemEventArgs> OnChangedCallbackMethod)
        {
            _fsWatcher = new FileSystemWatcher(path,filter);
            _fsWatcher.NotifyFilter = notifyFilters;
            _fsWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_Changed);

            _onChangedCallbackMethod = OnChangedCallbackMethod;

            _executeChanged = new InvokeMethod(FileSystemOnChangedCallbackMethod);
        }

        /// <summary> 
        /// 开始监控 
        /// </summary> 
        public void Start()
        {
            _fsWatcher.EnableRaisingEvents = true;
        }

        /// <summary> 
        /// 停止监控 
        /// </summary> 
        public void Stop()
        {
            _fsWatcher.EnableRaisingEvents = false;
        }


        void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _executeChanged.OnExecute(e.Name, e);
        }

        void FileSystemOnChangedCallbackMethod(object arg)
        {
            _onChangedCallbackMethod((FileSystemEventArgs)arg);
        }

        void fsWatcher_Renamed(object sender, FileSystemEventArgs e)
        {

        }
    }

    //public delegate void Completed(string key);

}

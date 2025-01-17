﻿using BetterGenshinImpact.Core.Config;
using BetterGenshinImpact.GameTask.Model;
using BetterGenshinImpact.Genshin.Settings;
using BetterGenshinImpact.Helpers;
using BetterGenshinImpact.Service;
using System;

namespace BetterGenshinImpact.GameTask
{
    /// <summary>
    /// 任务上下文
    /// </summary>
    public class TaskContext
    {
        private static TaskContext? _uniqueInstance;
        private static readonly object Locker = new();

        private TaskContext()
        {
        }

        public static TaskContext Instance()
        {
            if (_uniqueInstance == null)
            {
                lock (Locker)
                {
                    _uniqueInstance ??= new TaskContext();
                }
            }

            return _uniqueInstance;
        }

        public void Init(IntPtr hWnd)
        {
            GameHandle = hWnd;
            SystemInfo = new SystemInfo(hWnd);
            DpiScale = DpiHelper.ScaleY;
            //MaskWindowHandle = new WindowInteropHelper(MaskWindow.Instance()).Handle;
            IsInitialized = true;
        }

        public bool IsInitialized { get; set; }

        public IntPtr GameHandle { get; set; }

        //public IntPtr MaskWindowHandle { get; set; }

        public float DpiScale { get; set; }


        public SystemInfo SystemInfo { get; set; }


        public AllConfig Config
        {
            get
            {
                if (ConfigService.Config == null)
                {
                    throw new Exception("Config未初始化");
                }

                return ConfigService.Config;
            }
        }

        public SettingsContainer? GameSettings { get; set; }

        /// <summary>
        /// 关联启动原神的时间
        /// 注意 IsInitialized = false 时，这个值就会被设置
        /// </summary>
        public DateTime LinkedStartGenshinTime { get; set; } = DateTime.MinValue;
    }
}
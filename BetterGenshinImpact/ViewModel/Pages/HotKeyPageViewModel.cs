﻿using BetterGenshinImpact.Core.Config;
using BetterGenshinImpact.GameTask;
using BetterGenshinImpact.GameTask.Macro;
using BetterGenshinImpact.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using BetterGenshinImpact.Helpers.Extensions;
using Microsoft.Extensions.Logging;
using HotKeySettingModel = BetterGenshinImpact.Model.HotKeySettingModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using BetterGenshinImpact.GameTask.QucikBuy;
using BetterGenshinImpact.Model;

namespace BetterGenshinImpact.ViewModel.Pages;

public partial class HotKeyPageViewModel : ObservableObject
{
    private readonly ILogger<HotKeyPageViewModel> _logger;
    private readonly TaskSettingsPageViewModel _taskSettingsPageViewModel;
    public AllConfig Config { get; set; }

    [ObservableProperty] private ObservableCollection<HotKeySettingModel> _hotKeySettingModels = new();

    public HotKeyPageViewModel(IConfigService configService, ILogger<HotKeyPageViewModel> logger, TaskSettingsPageViewModel taskSettingsPageViewModel)
    {
        _logger = logger;
        _taskSettingsPageViewModel = taskSettingsPageViewModel;
        // 获取配置
        Config = configService.Get();

        // 构建快捷键配置列表
        BuildHotKeySettingModelList();

        foreach (var hotKeyConfig in HotKeySettingModels)
        {
            hotKeyConfig.RegisterHotKey();
            hotKeyConfig.PropertyChanged += (sender, e) =>
            {
                if (sender is HotKeySettingModel model)
                {
                    // 反射更新配置

                    // 更新快捷键
                    if (e.PropertyName == "HotKey")
                    {
                        Debug.WriteLine($"{model.FunctionName} 快捷键变更为 {model.HotKey}");
                        var pi = Config.HotKeyConfig.GetType().GetProperty(model.ConfigPropertyName, BindingFlags.Public | BindingFlags.Instance);
                        if (null != pi && pi.CanWrite)
                        {
                            var str = model.HotKey.ToString();
                            if (str == "< None >")
                            {
                                str = "";
                            }

                            pi.SetValue(Config.HotKeyConfig, str, null);
                        }
                    }

                    // 更新快捷键类型
                    if (e.PropertyName == "HotKeyType")
                    {
                        Debug.WriteLine($"{model.FunctionName} 快捷键类型变更为 {model.HotKeyType.ToChineseName()}");
                        model.HotKey = HotKey.None;
                        var pi = Config.HotKeyConfig.GetType().GetProperty(model.ConfigPropertyName + "Type", BindingFlags.Public | BindingFlags.Instance);
                        if (null != pi && pi.CanWrite)
                        {
                            pi.SetValue(Config.HotKeyConfig, model.HotKeyType.ToString(), null);
                        }
                    }

                    RemoveDuplicateHotKey(model);
                    model.UnRegisterHotKey();
                    model.RegisterHotKey();
                }
            };
        }
    }

    /// <summary>
    /// 移除重复的快捷键配置
    /// </summary>
    /// <param name="current"></param>
    private void RemoveDuplicateHotKey(HotKeySettingModel current)
    {
        if (current.HotKey.IsEmpty)
        {
            return;
        }

        foreach (var hotKeySettingModel in HotKeySettingModels)
        {
            if (hotKeySettingModel.HotKey.IsEmpty)
            {
                continue;
            }

            if (hotKeySettingModel.ConfigPropertyName != current.ConfigPropertyName && hotKeySettingModel.HotKey == current.HotKey)
            {
                hotKeySettingModel.HotKey = HotKey.None;
            }
        }
    }

    private void BuildHotKeySettingModelList()
    {
        var bgiEnabledHotKeySettingModel = new HotKeySettingModel(
            "启动停止 BetterGI",
            nameof(Config.HotKeyConfig.BgiEnabledHotkey),
            Config.HotKeyConfig.BgiEnabledHotkey,
            Config.HotKeyConfig.BgiEnabledHotkeyType,
            (_, _) => { WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, "SwitchTriggerStatus", "", "")); }
        );
        HotKeySettingModels.Add(bgiEnabledHotKeySettingModel);

        var takeScreenshotHotKeySettingModel = new HotKeySettingModel(
            "游戏截图（开发者）",
            nameof(Config.HotKeyConfig.TakeScreenshotHotkey),
            Config.HotKeyConfig.TakeScreenshotHotkey,
            Config.HotKeyConfig.TakeScreenshotHotkeyType,
            (_, _) => { TaskTriggerDispatcher.Instance().TakeScreenshot(); }
        );
        HotKeySettingModels.Add(takeScreenshotHotKeySettingModel);

        var autoPickEnabledHotKeySettingModel = new HotKeySettingModel(
            "自动拾取开关",
            nameof(Config.HotKeyConfig.AutoPickEnabledHotkey),
            Config.HotKeyConfig.AutoPickEnabledHotkey,
            Config.HotKeyConfig.AutoPickEnabledHotkeyType,
            (_, _) =>
            {
                TaskContext.Instance().Config.AutoPickConfig.Enabled = !TaskContext.Instance().Config.AutoPickConfig.Enabled;
                _logger.LogInformation("切换{Name}状态为[{Enabled}]", "自动拾取", ToChinese(TaskContext.Instance().Config.AutoPickConfig.Enabled));
            }
        );
        HotKeySettingModels.Add(autoPickEnabledHotKeySettingModel);

        var autoSkipEnabledHotKeySettingModel = new HotKeySettingModel(
            "自动剧情开关",
            nameof(Config.HotKeyConfig.AutoSkipEnabledHotkey),
            Config.HotKeyConfig.AutoSkipEnabledHotkey,
            Config.HotKeyConfig.AutoSkipEnabledHotkeyType,
            (_, _) =>
            {
                TaskContext.Instance().Config.AutoSkipConfig.Enabled = !TaskContext.Instance().Config.AutoSkipConfig.Enabled;
                _logger.LogInformation("切换{Name}状态为[{Enabled}]", "自动剧情", ToChinese(TaskContext.Instance().Config.AutoSkipConfig.Enabled));
            }
        );
        HotKeySettingModels.Add(autoSkipEnabledHotKeySettingModel);

        var autoFishingEnabledHotKeySettingModel = new HotKeySettingModel(
            "自动钓鱼开关",
            nameof(Config.HotKeyConfig.AutoFishingEnabledHotkey),
            Config.HotKeyConfig.AutoFishingEnabledHotkey,
            Config.HotKeyConfig.AutoFishingEnabledHotkeyType,
            (_, _) =>
            {
                TaskContext.Instance().Config.AutoFishingConfig.Enabled = !TaskContext.Instance().Config.AutoFishingConfig.Enabled;
                _logger.LogInformation("切换{Name}状态为[{Enabled}]", "自动钓鱼", ToChinese(TaskContext.Instance().Config.AutoFishingConfig.Enabled));
            }
        );
        HotKeySettingModels.Add(autoFishingEnabledHotKeySettingModel);

        var quickTeleportEnabledHotKeySettingModel = new HotKeySettingModel(
            "快速传送开关",
            nameof(Config.HotKeyConfig.QuickTeleportEnabledHotkey),
            Config.HotKeyConfig.QuickTeleportEnabledHotkey,
            Config.HotKeyConfig.QuickTeleportEnabledHotkeyType,
            (_, _) =>
            {
                TaskContext.Instance().Config.QuickTeleportConfig.Enabled = !TaskContext.Instance().Config.QuickTeleportConfig.Enabled;
                _logger.LogInformation("切换{Name}状态为[{Enabled}]", "快速传送", ToChinese(TaskContext.Instance().Config.QuickTeleportConfig.Enabled));
            }
        );
        HotKeySettingModels.Add(quickTeleportEnabledHotKeySettingModel);

        var quickTeleportTickHotKeySettingModel = new HotKeySettingModel(
            "手动触发快速传送触发快捷键（按住起效）",
            nameof(Config.HotKeyConfig.QuickTeleportTickHotkey),
            Config.HotKeyConfig.QuickTeleportTickHotkey,
            Config.HotKeyConfig.QuickTeleportTickHotkeyType,
            (_, _) => { Thread.Sleep(100); },
            true
        );
        HotKeySettingModels.Add(quickTeleportTickHotKeySettingModel);

        var turnAroundHotKeySettingModel = new HotKeySettingModel(
            "长按旋转视角 - 那维莱特转圈",
            nameof(Config.HotKeyConfig.TurnAroundHotkey),
            Config.HotKeyConfig.TurnAroundHotkey,
            Config.HotKeyConfig.TurnAroundHotkeyType,
            (_, _) => { TurnAroundMacro.Done(); },
            true
        );
        HotKeySettingModels.Add(turnAroundHotKeySettingModel);

        var enhanceArtifactHotKeySettingModel = new HotKeySettingModel(
            "按下快速强化圣遗物",
            nameof(Config.HotKeyConfig.EnhanceArtifactHotkey),
            Config.HotKeyConfig.EnhanceArtifactHotkey,
            Config.HotKeyConfig.EnhanceArtifactHotkeyType,
            (_, _) => { QuickEnhanceArtifactMacro.Done(); },
            true
        );
        HotKeySettingModels.Add(enhanceArtifactHotKeySettingModel);

        HotKeySettingModels.Add(new HotKeySettingModel(
            "按下快速购买商店物品",
            nameof(Config.HotKeyConfig.QuickBuyHotkey),
            Config.HotKeyConfig.QuickBuyHotkey,
            Config.HotKeyConfig.QuickBuyHotkeyType,
            (_, _) => { QuickBuyTask.Done(); },
            true
        ));


        HotKeySettingModels.Add(new HotKeySettingModel(
            "启动/停止自动七圣召唤",
            nameof(Config.HotKeyConfig.AutoGeniusInvokationHotkey),
            Config.HotKeyConfig.AutoGeniusInvokationHotkey,
            Config.HotKeyConfig.AutoGeniusInvokationHotkeyType,
            (_, _) => { _taskSettingsPageViewModel.OnSwitchAutoGeniusInvokation(); }
        ));

        HotKeySettingModels.Add(new HotKeySettingModel(
            "启动/停止自动伐木",
            nameof(Config.HotKeyConfig.AutoWoodHotkey),
            Config.HotKeyConfig.AutoWoodHotkey,
            Config.HotKeyConfig.AutoWoodHotkeyType,
            (_, _) => { _taskSettingsPageViewModel.OnSwitchAutoWood(); }
        ));

        HotKeySettingModels.Add(new HotKeySettingModel(
            "启动/停止自动战斗",
            nameof(Config.HotKeyConfig.AutoFightHotkey),
            Config.HotKeyConfig.AutoFightHotkey,
            Config.HotKeyConfig.AutoFightHotkeyType,
            (_, _) => { _taskSettingsPageViewModel.OnSwitchAutoFight(); }
        ));

        HotKeySettingModels.Add(new HotKeySettingModel(
            "启动/停止自动秘境",
            nameof(Config.HotKeyConfig.AutoDomainHotkey),
            Config.HotKeyConfig.AutoDomainHotkey,
            Config.HotKeyConfig.AutoDomainHotkeyType,
            (_, _) => { _taskSettingsPageViewModel.OnSwitchAutoDomain(); }
        ));
    }

    private string ToChinese(bool enabled)
    {
        return enabled.ToChinese();
    }
}
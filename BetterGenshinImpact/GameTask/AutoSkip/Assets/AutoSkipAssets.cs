﻿using System;
using System.Collections.Generic;
using BetterGenshinImpact.Core.Recognition;
using OpenCvSharp;

namespace BetterGenshinImpact.GameTask.AutoSkip.Assets;

public class AutoSkipAssets
{
    public RecognitionObject StopAutoButtonRo;
    public RecognitionObject PlayingTextRo;
    public RecognitionObject MenuRo;

    public Rect OptionRoi;
    public RecognitionObject OptionIconRo;
    public RecognitionObject DailyRewardIconRo;
    public RecognitionObject ExploreIconRo;
    public RecognitionObject ExclamationIconRo;

    public RecognitionObject PageCloseRo;

    public RecognitionObject CollectRo;
    public RecognitionObject ReRo;

    public RecognitionObject PrimogemRo;

    public RecognitionObject SubmitExclamationIconRo;
    public RecognitionObject SubmitGoodsRo;

    public Mat HangoutSelectedMat;
    public Mat HangoutUnselectedMat;

    public AutoSkipAssets()
    {
        var info = TaskContext.Instance().SystemInfo;
        StopAutoButtonRo = new RecognitionObject
        {
            Name = "StopAutoButton",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "stop_auto.png"),
            RegionOfInterest = new Rect(0, 0, info.CaptureAreaRect.Width / 5, info.CaptureAreaRect.Height / 8),
            DrawOnWindow = true
        }.InitTemplate();

        // 二值化的跳过剧情按钮
        //if (StopAutoButtonRo.TemplateImageGreyMat == null)
        //{
        //    throw new Exception("StopAutoButtonRo.TemplateImageGreyMat == null");
        //}
        //BinaryStopAutoButtonMat = StopAutoButtonRo.TemplateImageGreyMat.Clone();


        //Cv2.Threshold(BinaryStopAutoButtonMat, BinaryStopAutoButtonMat, 0, 255, ThresholdTypes.BinaryInv);

        PlayingTextRo = new RecognitionObject
        {
            Name = "PlayingText",
            RecognitionType = RecognitionTypes.Ocr,
            RegionOfInterest = new Rect((int)(100 * info.AssetScale), (int)(35 * info.AssetScale), (int)(85 * info.AssetScale), (int)(35 * info.AssetScale)),
            OneContainMatchText = new List<string>
            {
                "播", "番", "放", "中"
            },
            DrawOnWindow = true
        }.InitTemplate();


        OptionRoi = new Rect(info.CaptureAreaRect.Width / 2, 0, info.CaptureAreaRect.Width - info.CaptureAreaRect.Width / 2 - info.CaptureAreaRect.Width / 6, info.CaptureAreaRect.Height);
        OptionIconRo = new RecognitionObject
        {
            Name = "OptionIcon",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "icon_option.png"),
            RegionOfInterest = OptionRoi,
            DrawOnWindow = false
        }.InitTemplate();
        DailyRewardIconRo = new RecognitionObject
        {
            Name = "DailyRewardIcon",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "icon_daily_reward.png"),
            RegionOfInterest = OptionRoi,
            DrawOnWindow = false
        }.InitTemplate();
        ExploreIconRo = new RecognitionObject
        {
            Name = "ExploreIcon",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "icon_explore.png"),
            RegionOfInterest = OptionRoi,
            DrawOnWindow = false
        }.InitTemplate();
        // 更多对话要素
        ExclamationIconRo = new RecognitionObject
        {
            Name = "IconExclamation",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "icon_exclamation.png"),
            RegionOfInterest = OptionRoi,
            DrawOnWindow = false
        }.InitTemplate();

        // 其他
        MenuRo = new RecognitionObject
        {
            Name = "Menu",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "menu.png"),
            RegionOfInterest = new Rect(0, 0, info.CaptureAreaRect.Width / 4, info.CaptureAreaRect.Height / 4),
            DrawOnWindow = false
        }.InitTemplate();

        PageCloseRo = new RecognitionObject
        {
            Name = "PageClose",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "page_close.png"),
            RegionOfInterest = new Rect(info.CaptureAreaRect.Width - info.CaptureAreaRect.Width / 8, 0, info.CaptureAreaRect.Width / 8, info.CaptureAreaRect.Height / 8),
            DrawOnWindow = true
        }.InitTemplate();

        // 一键派遣
        CollectRo = new RecognitionObject
        {
            Name = "Collect",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "collect.png"),
            RegionOfInterest = new Rect(0, info.CaptureAreaRect.Height - info.CaptureAreaRect.Height / 3, info.CaptureAreaRect.Width / 4, info.CaptureAreaRect.Height / 3),
            DrawOnWindow = false
        }.InitTemplate();
        ReRo = new RecognitionObject
        {
            Name = "Re",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "re.png"),
            RegionOfInterest = new Rect(info.CaptureAreaRect.Width / 2, info.CaptureAreaRect.Height - info.CaptureAreaRect.Height / 4, info.CaptureAreaRect.Width / 4, info.CaptureAreaRect.Height / 4),
            DrawOnWindow = false
        }.InitTemplate();

        // 每日奖励
        PrimogemRo = new RecognitionObject
        {
            Name = "Primogem",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "primogem.png"),
            RegionOfInterest = new Rect(0, info.CaptureAreaRect.Height / 3, info.CaptureAreaRect.Width, info.CaptureAreaRect.Height / 3),
            DrawOnWindow = false
        }.InitTemplate();

        // 提交物品
        SubmitExclamationIconRo = new RecognitionObject
        {
            Name = "SubmitExclamationIconRo",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "submit_icon_exclamation.png"),
            RegionOfInterest = new Rect(0, 0, info.CaptureAreaRect.Width, info.CaptureAreaRect.Height / 4),
            DrawOnWindow = false
        }.InitTemplate();
        SubmitGoodsRo = new RecognitionObject
        {
            Name = "SubmitGoods",
            RecognitionType = RecognitionTypes.TemplateMatch,
            TemplateImageMat = GameTaskManager.LoadAssetImage("AutoSkip", "submit_goods.png"),
            RegionOfInterest = new Rect(0, 0, info.CaptureAreaRect.Width/2, info.CaptureAreaRect.Height / 3),
            DrawOnWindow = false
        }.InitTemplate();


        // 邀约
        HangoutSelectedMat = GameTaskManager.LoadAssetImage("AutoSkip", "hangout_selected.png", ImreadModes.Grayscale);
        HangoutUnselectedMat = GameTaskManager.LoadAssetImage("AutoSkip", "hangout_unselected.png", ImreadModes.Grayscale);
    }
}
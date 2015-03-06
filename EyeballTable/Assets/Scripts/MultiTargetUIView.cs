/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This module has been modified *slightly* so that connections to the server can be disabled.
 * * * * */

/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;

public class MultiTargetUIView : ISampleAppUIView {
    
    #region PUBLIC_PROPERTIES
    public CameraDevice.FocusMode FocusMode
    {
        get {
            return mFocusMode;
        }
        set {
            mFocusMode = value;
        }
    }
    #endregion PUBLIC_PROPERTIES
    
    #region PUBLIC_MEMBER_VARIABLES
    public event System.Action TappedToClose;
    public SampleAppUIBox mBox;
    public SampleAppUICheckButton mConnectLabel;
    public SampleAppUICheckButton mExtendedTracking;
    public SampleAppUICheckButton mCameraFlashSettings;
    public SampleAppUICheckButton mAutoFocusSetting;
    public SampleAppUILabel mCameraLabel;
    public SampleAppUIRadioButton mCameraFacing;
    public SampleAppUIButton mCloseButton;
    #endregion PUBLIC_MEMBER_VARIABLES
    
    #region PRIVATE_MEMBER_VARIABLES
    private CameraDevice.FocusMode mFocusMode;
    private SampleAppsUILayout mLayout;
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region PUBLIC_METHODS
    
    public void LoadView()
    {
        mLayout = new SampleAppsUILayout();
        mConnectLabel = mLayout.AddSimpleButton("(Re)connect");
        mLayout.AddGap(2);
        mExtendedTracking = mLayout.AddSlider("Extended Tracking", false);
        mLayout.AddGap(2);
        mAutoFocusSetting = mLayout.AddSlider("Autofocus", true);
        mLayout.AddGap(2);
        mCameraFlashSettings = mLayout.AddSlider("Flash", false);
        mLayout.AddGap(16);
        mCameraLabel = mLayout.AddGroupLabel("Camera");
        string[] options = { "Front", "Rear" };
        mCameraFacing = mLayout.AddToggleOptions(options, 1);
       
        Rect CloseButtonRect = new Rect(0, Screen.height - (100 * Screen.width) / 800.0f, Screen.width, (70.0f * Screen.width) / 800.0f);
        mCloseButton = mLayout.AddButton("Close", CloseButtonRect); 
    }
    
    public void UnLoadView()
    {
        mConnectLabel = null;
        mExtendedTracking = null;
        mCameraFlashSettings = null;
        mAutoFocusSetting = null;
        mCameraLabel = null;
        mCameraFacing = null;
    }
    
    public void UpdateUI(bool tf)
    {
        if(!tf)
        {
            return;
        }
        mLayout.Draw();
    }

    public void OnTappedToClose ()
    {
        if(this.TappedToClose != null)
        {
            this.TappedToClose();
        }
    }
    #endregion PUBLIC_METHODS
}


/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// UI Event Handler class that handles events generated by user-tap actions
/// over the UI Options Menu
/// </summary>
public class MultiTargetUIEventHandler : ISampleAppUIEventHandler { 
    
    #region PUBLIC_MEMBER_VARIABLES
    public override event System.Action CloseView;
    public override event System.Action GoToAboutPage;
    #endregion PUBLIC_MEMBER_VARIABLES
    
    #region PRIVATE_MEMBER_VARIABLES
    private MultiTargetUIView mView;
    private bool mCameraFacingFront;
    private static bool sExtendedTrackingIsEnabled;
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region PUBLIC_MEMBER_PROPERTIES
    public MultiTargetUIView View
    {
        get {
            if(mView == null){
                mView = new MultiTargetUIView();
                mView.LoadView();
            }
            return mView;
        }
    }

    /// <summary>
    /// Currently, there is no mechanism to query the SDK to know whether or not extended tracking is enabled/disabled.
    /// Therefore, it needs to be handled at the app layer.
    /// </value>
    public static bool ExtendedTrackingIsEnabled
    {
        get {
            return sExtendedTrackingIsEnabled;
        }
    }
    #endregion PUBLIC_MEMBER_PROPERTIES
    
    #region PUBLIC_METHODS
    public override void UpdateView (bool tf)
    {
        this.View.UpdateUI(tf);
    }
    
    public override  void Bind()
    {
        this.View.mExtendedTracking.TappedOn    += OnTappedToTurnOnTraking;
        this.View.mCameraFlashSettings.TappedOn += OnTappedToTurnOnFlash;
        this.View.mAutoFocusSetting.TappedOn    += OnTappedToTurnOnAutoFocus;
        this.View.mCameraFacing.TappedOnOption  += OnTappedToTurnCameraFacing;
        this.View.mCloseButton.TappedOn         += OnTappedOnCloseButton;
        this.View.mAboutLabel.TappedOn          += OnTappedOnAboutButton;
		this.View.mPlayCamera.TappedOn 			+= OnTappedToEnableVideo;
        sExtendedTrackingIsEnabled = false;
        EnableContinuousAutoFocus();
    }
    
    public override  void UnBind()
    { 
        this.View.mExtendedTracking.TappedOn    -= OnTappedToTurnOnTraking;
        this.View.mCameraFlashSettings.TappedOn -= OnTappedToTurnOnFlash;
        this.View.mAutoFocusSetting.TappedOn    -= OnTappedToTurnOnAutoFocus;
        this.View.mCameraFacing.TappedOnOption  -= OnTappedToTurnCameraFacing;
        this.View.mCloseButton.TappedOn         -= OnTappedOnCloseButton;
        this.View.mAboutLabel.TappedOn          -= OnTappedOnAboutButton;
		this.View.mPlayCamera.TappedOn 			-= OnTappedToEnableVideo;
    
        this.View.UnLoadView();
        mView = null;
    }
    
    public override  void TriggerAutoFocus()
    {
        StartCoroutine(TriggerAutoFocusAndEnableContinuousFocusIfSet());
    }
    
    public override  void SetToDefault(bool tf)
    {
        this.View.mCameraFlashSettings.Enable(tf);
    }
    #endregion PUBLIC_METHODS
    
    #region PRIVATE_METHODS
    
    /// <summary>
    /// Activating trigger autofocus mode unsets continuous focus mode (if was previously enabled from the UI Options Menu)
    /// So, we wait for a second and turn continuous focus back on (if options menu shows as enabled)
    /// </returns>
    private IEnumerator TriggerAutoFocusAndEnableContinuousFocusIfSet()
    {
        //triggers a single autofocus operation 
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO)) {
              this.View.FocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
        }
        
        yield return new WaitForSeconds(1.0f);
        
        //continuous focus mode is turned back on if it was previously enabled from the options menu
        if(this.View.mAutoFocusSetting.IsEnabled)
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO)) {
              this.View.FocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
            }
        }
        
        Debug.Log (this.View.FocusMode);
        
    }
    
    private void OnTappedOnAboutButton(bool tf)
    {
        if(this.GoToAboutPage != null)
        {
            this.GoToAboutPage();
        }
    }
    
    private void OnTappedToTurnOnTraking(bool tf)
    {
        if(!ExtendedTracking(tf)) 
        {
            this.View.mExtendedTracking.Enable(false);
        }
        else 
        {
            this.View.mExtendedTracking.Enable(tf);
        }
        OnTappedToClose();
    }
    
    private void OnTappedToTurnOnFlash(bool tf)
    {
        if(tf)
        {
            if(!CameraDevice.Instance.SetFlashTorchMode(true) || mCameraFacingFront)
            {
                this.View.mCameraFlashSettings.Enable(false);
            }
        }
        else 
        {
            CameraDevice.Instance.SetFlashTorchMode(false);
        }
        
        OnTappedToClose();
    }

	private void OnTappedToEnableVideo(bool tf)
	{
		QCARRenderer.Instance.DrawVideoBackground = tf;
		OnTappedToClose();
	}
    
    //We want autofocus to be enabled when the app starts
    private void EnableContinuousAutoFocus()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
        {
            this.View.FocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
            this.View.mAutoFocusSetting.Enable(true);
        }
    }
    
    private void OnTappedToTurnOnAutoFocus(bool tf)
    {
        if(tf)
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
            {
                this.View.FocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
            }
            else 
            {
                this.View.mAutoFocusSetting.Enable(false);
            }
        }
        else 
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL))
            {
                this.View.FocusMode = CameraDevice.FocusMode.FOCUS_MODE_NORMAL;
            }
        }
        
        OnTappedToClose();
    }
    
    private void OnTappedToTurnCameraFacing(int val)
    {
        if(val == 0)
        {
            //internally, flash is always turned off everytime it tries to switch to front camera
            //so updating the UI options to reflect that.
            this.View.mCameraFlashSettings.Enable(false);
            
            if(ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_FRONT)) {
                mCameraFacingFront = true;
            }
            else {
                ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_BACK);
                mCameraFacingFront = false;
                this.View.mCameraFacing.EnableIndex(1);
            }
        }
        else 
        {
            ChangeCameraDirection(CameraDevice.CameraDirection.CAMERA_BACK);
            mCameraFacingFront = false;
        }
        
        OnTappedToClose();
    }
    
    private void ResetCameraFacingToBack()
    {
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Init(CameraDevice.CameraDirection.CAMERA_BACK);
        CameraDevice.Instance.Start();
        mCameraFacingFront = false;
    }
    
    private bool ChangeCameraDirection(CameraDevice.CameraDirection direction)
    {
        bool directionSupported = false;
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        if(CameraDevice.Instance.Init(direction)) {
            directionSupported = true;
        }
        CameraDevice.Instance.Start();
        
        return directionSupported;
    }
    
    private void OnTappedToClose()
    {
        if(this.CloseView != null)
        {
            this.CloseView();
        }
    }
    
    private void OnTappedOnCloseButton()
    {
        OnTappedToClose();
    }
    
    /// <summary>
    /// This method turns extended tracking on or off for all currently available targets.
    /// Extended tracking allows to track targets when they are not in view.
    /// Returns true of extended tracking is supported; false otherwise
    /// </summary>
    private bool ExtendedTracking(bool tf)
    {
        // the StateManager gives access to all available TrackableBehavours
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        // We iterate over all TrackableBehaviours to start or stop extended tracking for the targets they represent.
        bool extendedTrackingStateChanged = true;
        foreach(var behaviour in stateManager.GetTrackableBehaviours())
        {
            var multiTgtBehaviour = behaviour as MultiTargetBehaviour;
            if(multiTgtBehaviour != null)
            {
                if(tf) {
                    //checks to see if extended tracking is supported, to handle the UI options appropriately
                    if(!multiTgtBehaviour.MultiTarget.StartExtendedTracking()){
                        extendedTrackingStateChanged = false;
                    }
                }
                else {
                    //checks to see if extended tracking is supported, to handle the UI options appropriately
                    if(!multiTgtBehaviour.MultiTarget.StopExtendedTracking()) {
                        extendedTrackingStateChanged = false;
                    }
                }

            }
        }

        if(!extendedTrackingStateChanged) {
            Debug.LogWarning("Extended Tracking Failed");
        }
        return extendedTrackingStateChanged;
    }
    #endregion PRIVATE_METHODS
}


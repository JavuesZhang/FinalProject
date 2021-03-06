using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.ArtificialBased;
using VRTK.GrabAttachMechanics;

public class BanshouSnapObject : SnapObjectBase
{
    public float maxAngle;
    public bool isAssembly;
    protected bool isRotating = false;
    protected Vector3 initialPosition;
    protected GameObject touchObject;
    protected Transform controlTransform;
    protected VRTK_InteractableObject controlInteract;
    protected VRTK_ArtificialRotator controlRotator;
    protected VRTK_RotateTransformGrabAttach controlGrabAttach;
    
    void Awake()
    {
        Global.Instance.banshouObejct = this;
    }

    void Update()
    {
        // if(isRotating && controlTransform != null && controlRotator != null) return;
        // if(controlInteract != null) controlInteract.isGrabbable = false;
        // GameObject touchObjectLeft = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractTouch>().GetTouchedObject();
        // GameObject touchObjectRight = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractTouch>().GetTouchedObject();
        
        // if(touchObjectLeft == null && touchObjectRight == null) return;
        // touchObject = touchObjectLeft == null ? touchObjectRight : touchObjectLeft;
        // if(!CommonUtil.GetStepController().IsInteractable(touchObject)) return;
    }

    void OnDisable()
    {
        if(Global.Instance != null && Global.Instance.banshouObejct == this)
        {
            Global.Instance.banshouObejct = null;
        }
    }

    public override void OnSnapped()
    {
        if(isRotating) return;
        Debug.Log("BanshouSnapped");
        isRotating = true;
        ManageGrabbableListeners(true);
    }

    public override void OnUnsnapped()
    {
        if(!isRotating) return;
        Debug.Log("BanshouUnsnapped");
        ManageGrabbableListeners(false);
        CheckAngle();
        isRotating = false;
    }

    private void RecordInitialPosition()
    {
        if(controlRotator == null || controlInteract == null) return;
        if(isAssembly)
        {
            controlRotator.angleLimits = new Limits2D(-maxAngle, 0f);
        }
        else
        {
            controlRotator.angleLimits = new Limits2D(0f, maxAngle);
        }
        controlRotator.stepValueRange = new Limits2D(0f, maxAngle);
    }
    public void SetParam(bool isValid)
    {
        if(!isValid)
        {
            if(controlInteract != null) controlInteract.isGrabbable = false;
            return;
        }
        touchObject = Global.Instance.rotatingObject;
        if(touchObject == null) return;
        controlTransform = touchObject.transform;
        initialPosition = CommonUtil.GetStepController().GetInitPosition(touchObject);

        controlRotator = touchObject.GetComponentInChildren<VRTK_ArtificialRotator>();
        controlGrabAttach = touchObject.GetComponentInChildren<VRTK_RotateTransformGrabAttach>();
        controlInteract = touchObject.GetComponentInChildren<VRTK_InteractableObject>();
        controlInteract.isGrabbable = true;
        
        Debug.Log("SetParamValid");
        RecordInitialPosition();
    }

    protected virtual void ManageGrabbableListeners(bool state)
    {
        if (controlGrabAttach != null)
        {
            if (state)
            {
                controlGrabAttach.AngleChanged += GrabMechanicAngleChanged;
                Debug.Log("GrabMechanicAngleChanged");
            }
            else
            {
                controlGrabAttach.AngleChanged -= GrabMechanicAngleChanged;
            }
        }
    }
    protected virtual void GrabMechanicAngleChanged(object sender, RotateTransformGrabAttachEventArgs e)
    {
        if (controlGrabAttach != null)
        {
            float currentValue = controlRotator.GetValue();
            controlTransform.localPosition = initialPosition - Vector3.up * (currentValue / 9000.0f);
            // Debug.Log(currentValue.ToString() + "---" + (currentValue / 18000.0f).ToString() + "===" + transform.localPosition.ToString());
        }
    }

    protected virtual void CheckAngle()
    {
        if(controlRotator == null) return;
        float currentValue = controlRotator.GetValue();
        float currentValueAbs = currentValue >= 0 ? currentValue : -currentValue;
        if(maxAngle - currentValueAbs <= 5f)
        {
            CommonUtil.NotifyStepController();
        }
    }
}

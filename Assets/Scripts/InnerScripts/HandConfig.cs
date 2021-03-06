
using UnityEngine;
using VRTK;

public class HandConfig : MonoBehaviour
{
    public VRTK_ControllerEvents leftController;
    public VRTK_ControllerEvents rightController;

    public GameObject leftHandAvatar;
    public GameObject rightHandAvatar;

    public GameObject realLeftAvatar;
    public GameObject realRightAvatar;

    protected virtual void Start()
    {
        ToggleVisibility();

        if (realLeftAvatar != null)
        {
            realLeftAvatar.SetActive(false);
        }
        if (realRightAvatar != null)
        {
            realRightAvatar.SetActive(false);
        }
    }

    protected virtual void ToggleVisibility()
    {
        ToggleAvatarVisibility();
        ToggleSDKVisibility();
        ToggleScriptAlias();
    }

    protected virtual void ToggleAvatarVisibility()
    {
        if (leftHandAvatar != null)
        {
            leftHandAvatar.SetActive(true);
        }
        if (rightHandAvatar != null)
        {
            rightHandAvatar.SetActive(true);
        }
    }

    protected virtual void ToggleSDKVisibility()
    {
        VRTK_SDKSetup sdkType = VRTK_SDKManager.GetLoadedSDKSetup();
        if (sdkType != null)
        {
            VRTK_ControllerReference leftController = VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerLeftHand(true));
            VRTK_ControllerReference rightController = VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerRightHand(true));

            switch (sdkType.name)
            {
                case "SteamVR":
                    ToggleControllerRenderer(leftController.actual, "Model");
                    ToggleControllerRenderer(rightController.actual, "Model");
                    break;
                case "Oculus":
                    ToggleControllerRenderer(leftController.model);
                    ToggleControllerRenderer(rightController.model);
                    break;
                case "WindowsMR":
                    ToggleControllerRenderer(leftController.model, "glTFController");
                    ToggleControllerRenderer(rightController.model, "glTFController");
                    break;
            }
        }
    }

    protected virtual void ToggleControllerRenderer(GameObject controller, string findPath = "")
    {
        if (controller != null)
        {
            if (findPath == "")
            {
                controller.SetActive(false);
            }
            else
            {
                Transform childModel = controller.transform.Find(findPath);
                if (childModel != null)
                {
                    childModel.gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual void ToggleScriptAlias()
    {
        GameObject scriptLeft = VRTK_DeviceFinder.GetControllerLeftHand(false);
        GameObject scriptRight = VRTK_DeviceFinder.GetControllerRightHand(false);
        CycleScriptAlias(scriptLeft, leftHandAvatar);
        CycleScriptAlias(scriptRight, rightHandAvatar);
    }

    protected virtual void CycleScriptAlias(GameObject controller, GameObject avatar)
    {
        if (controller != null)
        {
            VRTK_InteractTouch touch = controller.GetComponentInChildren<VRTK_InteractTouch>();
            VRTK_InteractGrab grab = controller.GetComponentInChildren<VRTK_InteractGrab>();

            touch.customColliderContainer = avatar.transform.Find("HandColliders").gameObject;
            grab.ForceControllerAttachPoint(avatar.transform.Find("GrabAttachPoint").GetComponent<Rigidbody>());

            touch.enabled = true;
            grab.enabled = true;
        }
    }
}
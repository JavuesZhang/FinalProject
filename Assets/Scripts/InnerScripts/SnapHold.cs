namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Highlighters;
    using VRTK.Controllables.ArtificialBased;
    // [ExecuteInEditMode]
    public class SnapHold : MonoBehaviour
    {
        [Tooltip("A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.")]
        public GameObject highlightObjectPrefab;
        [Tooltip("The colour to use when showing the snap zone is active. This is used as the highlight colour when no object is hovering but `Highlight Always Active` is true.")]
        public Color highlightColor = Color.clear;
        [Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
        public bool displayDropZoneInEditor = true;
        [Tooltip("If this is checked then grip should be held down to snap object to the zone.")]
        public bool isTool = true;
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
        public VRTK_PolicyList validObjectListPolicy;

        public VRTK_ControllerEvents leftController;
        public VRTK_ControllerEvents rightController;
        
        protected GameObject highlightContainer;
        protected GameObject highlightObject;
        protected GameObject highlightEditorObject = null;
        protected VRTK_InteractableObject currentSnappedObject = null;
        protected VRTK_InteractableObject currentInteractObject = null;
        protected bool willSnap = false;
        protected bool isSnapped = false;
        protected bool wasSnapped = false;
        protected bool isHighlighted = false;
        protected VRTK_BaseHighlighter objectHighlighter;
        protected Coroutine transitionInPlaceRoutine;
        protected Coroutine attemptTransitionAtEndOfFrameRoutine;

        protected Transform prevParent;
        protected Vector3 prevPosition;
        protected Quaternion prevRotation;

        protected const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";
        protected const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";
        protected const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

        protected virtual void Awake()
        {
            if(Application.isPlaying)
            {
                // ChooseDestroyType(transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)));
                // highlightEditorObject = null;
                if(GameObject.Find("LeftControllerScriptAlias") != null)
                {
                    leftController = GameObject.Find("LeftControllerScriptAlias").GetComponentInChildren<VRTK_ControllerEvents>();
                }
                if(GameObject.Find("RightControllerScriptAlias") != null)
                {
                    rightController = GameObject.Find("RightControllerScriptAlias").GetComponentInChildren<VRTK_ControllerEvents>();
                }
                GenerateHighlightObjects();
                if (highlightObject != null && objectHighlighter == null)
                {
                    InitializeHighlighter();
                }
            }
            
        }

        protected virtual void Update()
        {
            // CreateHighlightersInEditor();
        }
        
        protected virtual void ChooseDestroyType(Transform deleteTransform)
        {
            if (deleteTransform != null)
            {
                ChooseDestroyType(deleteTransform.gameObject);
            }
        }

        protected virtual void ChooseDestroyType(GameObject deleteObject)
        {
            Debug.Log("Destroy: "+deleteObject.name);
            if (VRTK_SharedMethods.IsEditTime())
            {
                if (deleteObject != null)
                {
                    DestroyImmediate(deleteObject);
                }
            }
            else
            {
                if (deleteObject != null)
                {
                    Destroy(deleteObject);
                }
            }
        }

        #region Highlight
        protected virtual void CreateHighlightersInEditor()
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                GenerateHighlightObjects();

                GenerateEditorHighlightObject();
                if (highlightEditorObject != null)
                {
                    highlightEditorObject.SetActive(displayDropZoneInEditor);
                }
            }
        }

        protected virtual void GenerateEditorHighlightObject()
        {
            if (highlightObject != null && highlightEditorObject == null && transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)) == null)
            {
                CopyObject(highlightObject, ref highlightEditorObject, HIGHLIGHT_EDITOR_OBJECT_NAME);
                Renderer[] renderers = highlightEditorObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material = Resources.Load("SnapDropZoneEditorObject") as Material;
                }
                highlightEditorObject.SetActive(true);
            }
        }

        private void GenerateHighlightObjects()
        {
            //If there is a given highlight prefab and no existing highlight object then create a new highlight object
            if (highlightObjectPrefab != null && highlightObject == null && transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME)) == null)
            {
                CopyObject(highlightObjectPrefab, ref highlightObject, HIGHLIGHT_OBJECT_NAME);
            }

            //if highlight object exists but not in the variable then force grab it
            Transform checkForChild = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
            if (checkForChild != null && highlightObject == null)
            {
                highlightObject = checkForChild.gameObject;
                
            }
            // VRTK_ArtificialRotator rotator = highlightObject.GetComponentInChildren<VRTK_ArtificialRotator>();
            // if(rotator != null)
            // {
            //     rotator.enabled = false; //不知道为何如果生效的话，会使得物体直接销毁
            // }

            DisableHighlightShadows();
            SetHighlightObjectActive(false);
        }
        
        protected virtual void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string givenName)
        {
            GenerateContainer();
            Vector3 saveScale = transform.localScale;
            transform.localScale = Vector3.one;

            clonedObject = Instantiate(objectBlueprint, highlightContainer.transform) as GameObject;
            clonedObject.name = givenName;

            //default position of new highlight object
            clonedObject.transform.localPosition = Vector3.zero;
            clonedObject.transform.localRotation = Quaternion.identity;

            transform.localScale = saveScale;
            Debug.Log("CopyObject");
        }

        protected virtual void GenerateContainer()
        {
            if (highlightContainer == null || transform.Find(HIGHLIGHT_CONTAINER_NAME) == null)
            {
                highlightContainer = new GameObject(HIGHLIGHT_CONTAINER_NAME);
                highlightContainer.transform.SetParent(transform);
                highlightContainer.transform.localPosition = Vector3.zero;
                highlightContainer.transform.localRotation = Quaternion.identity;
                highlightContainer.transform.localScale = Vector3.one;
            }
        }

        protected virtual void DisableHighlightShadows()
        {
            if (highlightObject != null)
            {
                Renderer[] foundRenderers = highlightObject.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < foundRenderers.Length; i++)
                {
                    foundRenderers[i].receiveShadows = false;
                    foundRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }

        protected virtual void SetHighlightObjectActive(bool state)
        {
            Debug.Log("SetActive?");
            if (highlightObject != null)
            {
                highlightObject.SetActive(state);
                Debug.Log("SetActive:"+state);
                isHighlighted = state;
            }
        }

        protected virtual void InitializeHighlighter()
        {
            VRTK_BaseHighlighter existingHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
            //If no highlighter is found on the GameObject then create the default one
            if (existingHighlighter == null)
            {
                highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }
            else
            {
                VRTK_SharedMethods.CloneComponent(existingHighlighter, highlightObject);
            }

            //Initialise highlighter and set highlight colour
            objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
            objectHighlighter.unhighlightOnDisable = false;
            objectHighlighter.Initialise(highlightColor);
            objectHighlighter.Highlight(highlightColor);

            //if the object highlighter is using a cloned object then disable the created highlight object's renderers
            if (objectHighlighter.UsesClonedObject())
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!VRTK_PlayerObject.IsPlayerObject(renderers[i].gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
                    {
                        renderers[i].enabled = false;
                    }
                }
            }

        }

        protected virtual string ObjectPath(string name)
        {
            return HIGHLIGHT_CONTAINER_NAME + "/" + name;
        }
        #endregion
    
        private void OnTriggerEnter(Collider collider)
        {
            // Debug.Log(collider.gameObject.name);
            if(collider.gameObject.CompareTag("Tool"))
            {
                // Debug.Log("inner: "+collider.gameObject.name);
                currentInteractObject = collider.GetComponentInParent<VRTK_InteractableObject>();
                CheckCanSnapHold(currentInteractObject);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.CompareTag("Tool"))
            {
                currentInteractObject = collider.GetComponentInParent<VRTK_InteractableObject>();
                CheckCanUnSnapHold(currentInteractObject);
            }
        }

        protected virtual void CheckCanSnapHold(VRTK_InteractableObject interactableObjectCheck)
        {
            if (ValidSnapObject(interactableObjectCheck))
            {
                Debug.Log("ValidSnapObject");
                Debug.Log(isSnapped);
                AddHoldEvent();
                if (!isSnapped)
                {
                    SetHighlightObjectActive(true);
                    willSnap = true;
                }
            }
        }
        protected virtual void CheckCanUnSnapHold(VRTK_InteractableObject interactableObjectCheck)
        {
            if (isSnapped && currentSnappedObject == interactableObjectCheck)
            {
                RecoverPreviousState();            
                isSnapped = false;
                wasSnapped = true;
                currentSnappedObject = null;
                RemoveHoldEvent();
            }
            SetHighlightObjectActive(false);
            willSnap = false;

        }

        protected virtual bool ValidSnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            return (interactableObjectCheck != null && !VRTK_PolicyList.Check(interactableObjectCheck.gameObject, validObjectListPolicy));
        }

        protected virtual void AddHoldEvent()
        {
            if(isTool)
            {
                if(leftController != null)
                {
                    leftController.GripPressed += HoldObject;
                    leftController.GripReleased += UnholdObject;
                }
                if(rightController != null)
                {
                    rightController.GripPressed += HoldObject;
                    rightController.GripReleased += UnholdObject;
                }
            }
            else
            {
                if(leftController != null)
                {
                    leftController.GripClicked += HoldObject;
                }
                if(rightController != null)
                {
                    rightController.GripClicked += HoldObject;
                }
            }
            Debug.Log("AddHoldEvent");
        }

        protected virtual void RemoveHoldEvent()
        {
            if(isTool)
            {
                if(leftController != null)
                {
                    leftController.GripPressed -= HoldObject;
                    leftController.GripReleased -= UnholdObject;
                }
                if(rightController != null)
                {
                    rightController.GripPressed -= HoldObject;
                    rightController.GripReleased -= UnholdObject;
                }
            }
            else
            {
                if(leftController != null)
                {
                    leftController.GripClicked -= HoldObject;
                }
                if(rightController != null)
                {
                    rightController.GripClicked -= HoldObject;
                }
            }
            Debug.Log("RemoveHoldEvent");
        }

        protected virtual void HoldObject(object sender, ControllerInteractionEventArgs e)
        {
            if (attemptTransitionAtEndOfFrameRoutine != null)
            {
                StopCoroutine(attemptTransitionAtEndOfFrameRoutine);
            }
            attemptTransitionAtEndOfFrameRoutine = StartCoroutine(AttemptForceSnapAtEndOfFrame(currentInteractObject));
            // AttemptForceSnap(currentInteractObject);
        }

        protected virtual void UnholdObject(object sender, ControllerInteractionEventArgs e)
        {
            Debug.Log("Ungrab");
            CheckCanUnSnapHold(currentSnappedObject);
            CheckCanSnapHold(currentSnappedObject);
            SnapObjectBase snapObject = currentInteractObject.gameObject.GetComponent<SnapObjectBase>();
            if(snapObject != null)
            {
                snapObject.OnUnsnapped();
            }
        }

        protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(VRTK_InteractableObject currentInteractObject)
        {
            yield return new WaitForEndOfFrame();
            SaveCurrentState();
            AttemptForceSnap(currentInteractObject);
            // Debug.Log("AttemptForceSnap"+currentInteractObject.gameObject.name);
            if(!isTool){
                RemoveHoldEvent();
            }
            SnapObjectBase snapObject = currentInteractObject.gameObject.GetComponent<SnapObjectBase>();
            if(snapObject != null)
            {
                snapObject.OnSnapped();
            }
        }

        protected virtual void SaveCurrentState()
        {
            if(currentInteractObject != null)
            {
                prevParent = currentInteractObject.transform.parent;
                prevPosition = currentInteractObject.transform.localPosition;
                prevRotation = currentInteractObject.transform.localRotation;
            }
        }

        protected virtual void RecoverPreviousState()
        {
            Transform curTransform = currentSnappedObject.transform;
            curTransform.SetParent(prevParent);
            curTransform.localPosition = prevPosition;
            curTransform.localRotation = prevRotation;
        }

        protected virtual void AttemptForceSnap(VRTK_InteractableObject objectToSnap)
        {
            //force snap settings on
            willSnap = true;
            //Force touch one of the object's colliders on this trigger collider
            SnapObjectToZone(objectToSnap);
            // transform.GetComponentInChildren<Collider>().enabled = false;
            // Collider[] colliders = currentSnappedObject.transform.GetComponentsInChildren<Collider>();
            // foreach (Collider collider in colliders)
            // {
            //     collider.enabled = true;
            // }
        }

        protected virtual void SnapObjectToZone(VRTK_InteractableObject objectToSnap)
        {
            if (!isSnapped && ValidSnapObject(objectToSnap))
            {
                SnapObject(objectToSnap);
            }
        }

        protected virtual void SnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ValidSnapObject(interactableObjectCheck))
            {
                //Only snap it to the drop zone if it's not already in a drop zone
                if (!interactableObjectCheck.IsInSnapDropZone())
                {
                    if (highlightObject != null)
                    {
                        //Turn off the drop zone highlighter
                        SetHighlightObjectActive(false);
                    }

                    // Vector3 newLocalScale = GetNewLocalScale(interactableObjectCheck);
                    if (transitionInPlaceRoutine != null)
                    {
                        StopCoroutine(transitionInPlaceRoutine);
                    }

                    isSnapped = true;
                    currentSnappedObject = interactableObjectCheck;

                    if (gameObject.activeInHierarchy)
                    {
                        UpdateTransformDimensions(interactableObjectCheck, highlightContainer);
                    }

                }
            }

            //Force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && interactableObjectCheck != null && interactableObjectCheck.IsGrabbed() ? false : isSnapped);
            willSnap = !isSnapped;
            wasSnapped = false;
        }

        protected virtual void UpdateTransformDimensions(VRTK_InteractableObject ioCheck, GameObject endSettings)
        {
            Transform ioTransform = ioCheck.transform;
            if (ioTransform != null && endSettings != null)
            {
                //Force all to the last setting in case anything has moved during the transition
                ioTransform.position = endSettings.transform.position;
                ioTransform.rotation = endSettings.transform.rotation;
                ioTransform.SetParent(transform);
            }

            // SetDropSnapType(ioCheck);
            
        }

    }

}

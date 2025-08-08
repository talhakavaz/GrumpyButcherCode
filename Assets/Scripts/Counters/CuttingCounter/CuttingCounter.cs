using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of counter, capable of holding, and processing the item when alt interacted with
/// </summary>
public class CuttingCounter : ClearCounter, IProgressTracked
{
    public static event EventHandler OnAnyCut;

    public static new void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<CutProgressUpdatedArg> OnCut;
    public event EventHandler<IProgressTracked.ProgressChangedArg> OnProgressChanged;
    public class CutProgressUpdatedArg : EventArgs, IProgressTracked.ProgressChangedArg
    {
        public bool active;

        public bool IsBarActive()
        {
            return active;
        }

        public bool IsWarning()
        {
            return false;
        }
    }

    public float GetNormalizedProgress()
    {
        Debug.Assert(Cutting);
        return CuttingProgressNormalized;
    }

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;

    private CuttingRecipeSO activeCuttingRecipe;
    private int cuttingProgress;

    private bool Cutting => activeCuttingRecipe != null;
    private bool DoneCutting => cuttingProgress >= activeCuttingRecipe.cutCount;
    private float CuttingProgressNormalized => (float)cuttingProgress / activeCuttingRecipe.cutCount;

    private void Start()
    {
        // each time the counter changes item, clear the cutting progress
        OnCounterItemChange += (_, _) => ResetCutting();

        // chain the onCut event with the onProgressChanged event and onAnyCut
        OnCut += (obj, arg) => OnProgressChanged.Invoke(obj, arg);
        OnCut += (obj, _) => OnAnyCut?.Invoke(obj, EventArgs.Empty);
    }

    protected override void InteractAltAction(Player player)
    {
        if (Cutting)
        {
            Cut();
        }
        else if (HoldingObject)
        {
            // destroy the current object, and spawns the cutted one
            CuttingRecipeSO recipe = GetCuttingRecipe(currentKitchenObject.GetKitchenObjectSO());
            if (recipe)
            {
                InitalizeCutting(recipe);
                Cut();
            }
            else
            {
                Debug.Log(currentKitchenObject.GetKitchenObjectSO().objectName + " cannot be cut");
            }
        }
    }

    protected override bool FilterAllowedObject(KitchenObject kitchenObject)
    {
        return kitchenObject.GetKitchenObjectSO().objectName != "Plate";
    }

    #region Recipe Actions

    private CuttingRecipeSO GetCuttingRecipe(KitchenObjectSO from)
    {
        foreach (CuttingRecipeSO recipe in cuttingRecipes)
        {
            if (recipe.from == from)
            {
                return recipe;
            }
        }
        return null;
    }

    #endregion

    #region Cutting Actions

    private void InitalizeCutting(CuttingRecipeSO recipe)
    {
        if (Cutting)
        {
            Debug.LogError("Cannot have multiple active cutting recipe");
        }
        else
        {
            activeCuttingRecipe = recipe;
            SetProgress(0);
        }
    }

    private void ResetCutting()
    {
        activeCuttingRecipe = null;
        SetProgress(0);
    }

    private void Cut()
    {
        if (Cutting)
        {
            SetProgress(cuttingProgress + 1);

            // Checks for done cutting
            if (DoneCutting)
            {
                currentKitchenObject.DestroySelf();
                KitchenObject.Spawn(activeCuttingRecipe.to, this);
                ResetCutting();
            }
        }
        else
        {
            Debug.LogError("Cannot cut when with no active cutting recipe");
        }
    }

    private void SetProgress(int newProgress)
    {
        cuttingProgress = newProgress;
        OnCut?.Invoke(this, new CutProgressUpdatedArg
        {
            active = cuttingProgress != 0
        });
    }

    #endregion
}



//using System;
//using System.Collections;
//using System.Collections.Generic;
//using DynamicMeshCutter;
//using UnityEngine;

///// <summary>
///// A type of counter, capable of holding, and processing the item when alt interacted with
///// </summary>
//public class CuttingCounter : ClearCounter, IProgressTracked
//{
//    public static event EventHandler OnAnyCut;

//    public static new void ResetStaticData()
//    {
//        OnAnyCut = null;
//    }

//    public event EventHandler<CutProgressUpdatedArg> OnCut;
//    public event EventHandler<IProgressTracked.ProgressChangedArg> OnProgressChanged;
//    public class CutProgressUpdatedArg : EventArgs, IProgressTracked.ProgressChangedArg
//    {
//        public bool active;

//        public bool IsBarActive()
//        {
//            return active;
//        }

//        public bool IsWarning()
//        {
//            return false;
//        }
//    }

//    public float GetNormalizedProgress()
//    {
//        return IsCuttingComplete() ? 1.0f : 0.0f;
//    }

//    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;

//    private CuttingRecipeSO activeCuttingRecipe;
//    private bool cuttingInProgress;

//    private GameObject cutterObject; // Reference to the CutterBehaviour object in the scene

//    private void Start()
//    {
//        OnCounterItemChange += (_, _) => ResetCutting();

//        OnCut += (obj, arg) => OnProgressChanged.Invoke(obj, arg);
//        OnCut += (obj, _) => OnAnyCut?.Invoke(obj, EventArgs.Empty);

//        cutterObject = GameObject.FindObjectOfType<MouseBehaviour>()?.gameObject;
//        if (cutterObject == null)
//        {
//            Debug.LogError("No CutterBehaviour (e.g., MouseBehaviour) found in the scene!");
//        }
//    }

//    protected override void InteractAltAction(Player player)
//    {
//        if (IsCuttingComplete())
//        {
//            // Cutting is complete, convert cut meshes to KitchenObjects and allow pickup
//            GameObject masterParent = GameObject.Find("SlicedMeshesMasterParent");
//            if (masterParent != null)
//            {
//                foreach (Transform child in masterParent.transform)
//                {
//                    KitchenObject kitchenObject = child.gameObject.GetComponent<KitchenObject>();
//                    if (kitchenObject == null)
//                    {
//                        // Add KitchenObject component if not present
//                        KitchenObjectSO kitchenObjectSO = activeCuttingRecipe.to; // Use the resulting KitchenObjectSO
//                        kitchenObject = child.gameObject.AddComponent<KitchenObject>();
//                        kitchenObject.SetKitchenObjectSO(kitchenObjectSO);
//                    }
//                    // Allow player to pick up the object
//                    if (player != null && !player.HoldingObject)
//                    {
//                        SetCurrentKitchenObject(null); // Clear counter
//                        if (kitchenObject != null)
//                        {
//                            kitchenObject.SetKitchenObjectHolder(player); // Transfer to player
//                            Destroy(child.gameObject, 0); // Destroy the child after pickup
//                        }
//                    }
//                }
//                // Clear the master parent after all objects are picked up
//                if (masterParent.transform.childCount == 0)
//                {
//                    Destroy(masterParent);
//                }
//                ResetCutting();
//            }
//        }
//        else if (HoldingObject)
//        {
//            CuttingRecipeSO recipe = GetCuttingRecipe(currentKitchenObject.GetKitchenObjectSO());
//            if (recipe != null)
//            {
//                InitializeCutting(recipe);
//                StartMeshCutting();
//            }
//            else
//            {
//                Debug.Log(currentKitchenObject.GetKitchenObjectSO().objectName + " cannot be cut");
//            }
//        }
//    }

//    protected override bool FilterAllowedObject(KitchenObject kitchenObject)
//    {
//        return kitchenObject.GetKitchenObjectSO().objectName != "Plate";
//    }

//    #region Recipe Actions

//    private CuttingRecipeSO GetCuttingRecipe(KitchenObjectSO from)
//    {
//        foreach (CuttingRecipeSO recipe in cuttingRecipes)
//        {
//            if (recipe.from == from)
//            {
//                return recipe;
//            }
//        }
//        return null;
//    }

//    #endregion

//    #region Cutting Actions

//    private void InitializeCutting(CuttingRecipeSO recipe)
//    {
//        if (activeCuttingRecipe != null)
//        {
//            Debug.LogError("Cannot have multiple active cutting recipes");
//        }
//        else
//        {
//            activeCuttingRecipe = recipe;
//            cuttingInProgress = true;
//            SetProgress(true);
//        }
//    }

//    private void ResetCutting()
//    {
//        activeCuttingRecipe = null;
//        cuttingInProgress = false;
//        SetProgress(false);
//    }

//    private void StartMeshCutting()
//    {
//        if (cutterObject != null && HoldingObject)
//        {
//            MouseBehaviour cutter = cutterObject.GetComponent<MouseBehaviour>();
//            if (cutter != null)
//            {
//                MeshTarget meshTarget = currentKitchenObject.gameObject.GetComponent<MeshTarget>();
//                if (meshTarget != null)
//                {
//                    cutter.Cut(meshTarget, Vector3.zero, Vector3.forward); // Adjust position and normal as needed
//                }
//                else
//                {
//                    Debug.LogError("MeshTarget component not found on the kitchen object!");
//                }
//            }
//        }
//    }

//    private bool IsCuttingComplete()
//    {
//        GameObject masterParent = GameObject.Find("SlicedMeshesMasterParent");
//        return masterParent != null && masterParent.transform.childCount > 0;
//    }

//    private void SetProgress(bool isActive)
//    {
//        OnCut?.Invoke(this, new CutProgressUpdatedArg { active = isActive });
//    }

//    #endregion
//}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jiggle : MVRScript
{
    private Transform _cua;
    private DynamicBone _dynamicBone;
    private JSONStorableStringChooser _rootJSON;
    private CustomUnityAssetLoader _loader;
    private JSONStorableFloat _updateRateJSON;
    private JSONStorableFloat _dampingJSON;
    private JSONStorableFloat _elasticityJSON;
    private JSONStorableFloat _inertJSON;
    private JSONStorableFloat _radiusJSON;
    private JSONStorableFloat _endLengthJSON;

    public JSONStorableFloat _stiffnessJSON { get; private set; }

    public override void Init()
    {
        try
        {
            var containingAtom = GetAtom();
            _loader = containingAtom.GetComponentInChildren<CustomUnityAssetLoader>();
            if (_loader == null) throw new InvalidOperationException($"Atom does not have a {nameof(CustomUnityAssetLoader)}");

            _rootJSON = new JSONStorableStringChooser("Root", new List<string>(), "", "Root", (string _) => Apply());
            RegisterStringChooser(_rootJSON);
            var rootPopup = CreateFilterablePopup(_rootJSON, false);
            rootPopup.popup.onOpenPopupHandlers += SyncRoot;

            _updateRateJSON = new JSONStorableFloat("Update rate", 60f, (float _) => Apply(), 1f, 120f, false);
            RegisterFloat(_updateRateJSON);
            CreateSlider(_updateRateJSON, false);

            _dampingJSON = new JSONStorableFloat("Damping", 0.1f, (float _) => Apply(), 0f, 1f, true);
            RegisterFloat(_dampingJSON);
            CreateSlider(_dampingJSON, false);

            _elasticityJSON = new JSONStorableFloat("Elasticity", 0.1f, (float _) => Apply(), 0f, 1f, true);
            RegisterFloat(_elasticityJSON);
            CreateSlider(_elasticityJSON, false);

            _stiffnessJSON = new JSONStorableFloat("Stiffness", 0.1f, (float _) => Apply(), 0f, 1f, true);
            RegisterFloat(_stiffnessJSON);
            CreateSlider(_stiffnessJSON, false);

            _inertJSON = new JSONStorableFloat("Inert", 0f, (float _) => Apply(), 0f, 1f, true);
            RegisterFloat(_inertJSON);
            CreateSlider(_inertJSON, false);

            _radiusJSON = new JSONStorableFloat("Radius", 0f, (float _) => Apply(), 0f, 1f, false);
            RegisterFloat(_radiusJSON);
            CreateSlider(_radiusJSON, false);

            _endLengthJSON = new JSONStorableFloat("End length", 0f, (float _) => Apply(), 0f, 1f, false);
            RegisterFloat(_endLengthJSON);
            CreateSlider(_endLengthJSON, false);

            SyncRoot();

            StartCoroutine(InitDeferred());
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(Init)}: {e}");
        }
    }

    public IEnumerator InitDeferred()
    {
        if (string.IsNullOrEmpty(_rootJSON.val)) yield break;
        if (string.IsNullOrEmpty(containingAtom.GetStorableByID("asset").GetStringChooserParamValue("assetName"))) yield break;

        while (_loader.transform.childCount == 0)
            yield return 0;

        Apply();
    }

    private void Apply()
    {
        if (_loader.transform.childCount == 0) return;
        _cua = _loader.transform.GetChild(0);
        if (_dynamicBone != null) { Destroy(_dynamicBone); _dynamicBone = null; }
        var root = _cua.Find(_rootJSON.val.TrimStart());
        if (root == null) return;
        _dynamicBone = _cua.gameObject.AddComponent<DynamicBone>();
        _dynamicBone.m_Root = root;
        _dynamicBone.m_UpdateRate = _updateRateJSON.val;
        _dynamicBone.m_Damping = _dampingJSON.val;
        _dynamicBone.m_Elasticity = _elasticityJSON.val;
        _dynamicBone.m_Stiffness = _stiffnessJSON.val;
        _dynamicBone.m_Radius = _radiusJSON.val;
        _dynamicBone.m_Inert = _inertJSON.val;
        _dynamicBone.m_EndLength = _endLengthJSON.val;
    }

    private void SyncRoot()
    {
        if (_loader.transform.childCount == 0) {
            _rootJSON.choices = new List<string>();
            return;
        }
        var cua = _loader.transform.GetChild(0);
        var potentialRoots = new List<string>();
        GetChildren(0, cua, potentialRoots);
        _rootJSON.choices = potentialRoots;
    }

    private void GetChildren(int indent, Transform parent, List<string> list)
    {
        foreach (Transform child in parent)
        {
            if (!string.IsNullOrEmpty(child.name))
            {
                var name = new string(' ', indent) + child.name;
                if (!list.Contains(child.name))
                    list.Add(name);
            }
            GetChildren(indent + 2, child, list);
        }
    }

    public void OnEnable()
    {
        try
        {
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnEnable)}: {e}");
        }
    }

    public void OnDisable()
    {
        try
        {
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnDisable)}: {e}");
        }
    }

    public void OnDestroy()
    {
        try
        {
            if (_dynamicBone != null) { Destroy(_dynamicBone); _dynamicBone = null; }
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(OnDestroy)}: {e}");
        }
    }

    private Atom GetAtom()
    {
        // Note: Yeah, that's horrible, but containingAtom is null
        var container = gameObject?.transform?.parent?.parent?.parent?.parent?.parent?.gameObject;
        if (container == null) throw new NullReferenceException($"Could not find the parent gameObject while looking for the containing atom.");
        var atom = container.GetComponent<Atom>();
        if (atom == null) throw new NullReferenceException($"Could not find the parent atom in {container.name}.");
        if (atom.type != "CustomUnityAsset") throw new InvalidOperationException("Can only be applied on CustomUnityAsset.");
        return atom;
    }
}

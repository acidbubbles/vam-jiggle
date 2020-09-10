using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jiggle : MVRScript
{
    private Transform _cua;
    private DynamicBone _dynamicBone;
    private JSONStorableStringChooser _rootJSON;

    public override void Init()
    {
        try
        {
            var containingAtom = GetAtom();
            var loader = containingAtom.GetComponentInChildren<CustomUnityAssetLoader>();
            if (loader == null) throw new InvalidOperationException($"Atom does not have a {nameof(CustomUnityAssetLoader)}");
            if (loader.transform.childCount == 0) throw new InvalidOperationException($"No unity asset has been loaded");
            _cua = loader.transform.GetChild(0);

            _rootJSON = new JSONStorableStringChooser("Root", new List<string>(), "", "Root", SelectRoot);
            RegisterStringChooser(_rootJSON);
            var rootPopup = CreateFilterablePopup(_rootJSON, false);
            rootPopup.popup.onOpenPopupHandlers += SyncRoot;

            SyncRoot();
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(Init)}: {e}");
        }
    }

    private void SelectRoot(string val)
    {
        if (_dynamicBone != null) { Destroy(_dynamicBone); _dynamicBone = null; }
        var root = _cua.Find(val.TrimStart());
        if (root == null) return;
        _dynamicBone = _cua.gameObject.AddComponent<DynamicBone>();
        _dynamicBone.m_Root = root;
    }

    private void SyncRoot()
    {
        if (_cua == null || _rootJSON == null) return;
        var potentialRoots = new List<string>();
        GetChildren(0, _cua, potentialRoots);
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

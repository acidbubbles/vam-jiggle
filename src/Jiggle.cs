using System;
using UnityEngine;

public class Jiggle : MVRScript
{
    private Transform _cua;
    private DynamicBone _dynamicBone;

    public override void Init()
    {
        try
        {
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Jiggle)}.{nameof(Init)}: {e}");
        }
    }

    public void OnEnable()
    {
        try
        {
            var containingAtom = GetAtom();
            var loader = containingAtom.GetComponentInChildren<CustomUnityAssetLoader>();
            if (loader == null) throw new InvalidOperationException($"Atom does not have a {nameof(CustomUnityAssetLoader)}");
            if (loader.transform.childCount == 0) throw new InvalidOperationException($"No unity asset has been loaded");
            _cua = loader.transform.GetChild(0);
            _dynamicBone = _cua.gameObject.AddComponent<DynamicBone>();
            _dynamicBone.m_Root = _cua.GetChild(1);
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
            if (_dynamicBone != null) Destroy(_dynamicBone);
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

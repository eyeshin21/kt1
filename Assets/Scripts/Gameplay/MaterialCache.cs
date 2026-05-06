using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialCache
{
    static Dictionary<ColorEnum, Material> shooterActiveSelectableMats;
    public static Material GetShooterActiveSelectableMat(ColorEnum color)
    {
        if (shooterActiveSelectableMats == null)
        {
            shooterActiveSelectableMats = new Dictionary<ColorEnum, Material>();
        }

        if (!shooterActiveSelectableMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Shooters/ActiveSelectable/{color}");
            shooterActiveSelectableMats[color] = mat;
        }

        return mat;
    }
    
    static Dictionary<ColorEnum, Material> shooterActiveMats;
    public static Material GetShooterActiveMat(ColorEnum color)
    {
        if (shooterActiveMats == null)
        {
            shooterActiveMats = new Dictionary<ColorEnum, Material>();
        }

        if (!shooterActiveMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Shooters/Active/{color}");
            shooterActiveMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<ColorEnum, Material> shooterInactiveMats;
    public static Material GetShooterInactiveMat(ColorEnum color)
    {
        if (shooterInactiveMats == null)
        {
            shooterInactiveMats = new Dictionary<ColorEnum, Material>();
        }

        if (!shooterInactiveMats.TryGetValue(color, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Shooters/Inactive/{color}");
            shooterInactiveMats[color] = mat;
        }

        return mat;
    }

    static Dictionary<int, Material> lockMats;
    public static Material GetLockMat(int lockCode)
    {
        if (lockMats == null)
        {
            lockMats = new Dictionary<int, Material>();
        }

        if (!lockMats.TryGetValue(lockCode, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Locks/{lockCode}");
            lockMats[lockCode] = mat;
        }

        return mat;
    }

    static Dictionary<int, Material> keyMats;
    public static Material GetKeyMat(int lockCode)
    {
        if (keyMats == null)
        {
            keyMats = new Dictionary<int, Material>();
        }

        if (!keyMats.TryGetValue(lockCode, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Keys/{lockCode}");
            keyMats[lockCode] = mat;
        }

        return mat;
    }

    static Dictionary<eTypeLevel, Material> cornerMats;
    public static Material GetCornerMat(eTypeLevel eTypeLevel)
    {
        if (cornerMats == null)
        {
            cornerMats = new Dictionary<eTypeLevel, Material>();
        }

        if (!cornerMats.TryGetValue(eTypeLevel, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Corners/{eTypeLevel}");
            cornerMats[eTypeLevel] = mat;
        }

        return mat;
    }

    static Dictionary<int, Material> woodMats;
    public static Material GetWoodMat(int id)
    {
        if (woodMats == null)
        {
            woodMats = new Dictionary<int, Material>();
        }

        if (!woodMats.TryGetValue(id, out var mat))
        {
            mat = Resources.Load<Material>($"Materials/Wood/{id}");
            woodMats[id] = mat;
        }

        return mat;
    }
}

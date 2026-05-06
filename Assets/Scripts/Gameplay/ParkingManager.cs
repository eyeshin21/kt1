using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class ParkingManager : Singleton<ParkingManager>
{
    public List<ParkingSlot> slots = new List<ParkingSlot>();
    public List<ParkingSlot> slotsToClear = new List<ParkingSlot>();
    public List<ShooterController> parkedShooters = new List<ShooterController>();

    public void Init()
    {
        //int level = UserConfig.Instance.CurLevel;
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init();
            //if (i == 5)
            //{
            //    slots[i].Init(level < 8);
            //}
            //else if (i == 6)
            //{
            //    slots[i].Init(level < 24);
            //}
            //else
            //{
            //    slots[i].Init(false);
            //}
        }
    }

    public void CheckShoot()
    {
        //if (parkedShooters.Count > 0)
        //{
        //    for (int i = 0; i < parkedShooters.Count; i++)
        //    {
        //        if (!parkedShooters[i].isShooting)
        //        {
        //            parkedShooters[i].CheckShoot();
        //        }
        //    }
        //}
    }

    public void CheckWarning()
    {
        if (TotalFreeParkingSlot() == 1)
        {
            ParkingSlot parkingSlot = GetFreeSlot();
            if (parkingSlot != null)
            {
                parkingSlot.Warning();
            }
        }
    }

    public void RotateShooter(out float rotateTime)
    {
        float delay = 0;
        for (int i = 0; i < slotsToClear.Count; i++)
        {
            ShooterController shooter = slotsToClear[i].parkedShooter;

            if (shooter != null)
            {
                shooter.transform.GetChild(0).DOPunchPosition(Vector3.back * 1f, 0.25f, 1).SetEase(Ease.Linear).SetDelay(delay);
                shooter.transform.GetChild(0).DOLocalRotate(Vector3.back * 360f, 0.25f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetDelay(delay);
                delay += 0.125f;
            }
        }

        rotateTime = delay + 0.25f;
    }

    public int TotalUnlockedParkingSlot()
    {
        int count = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].isLocked)
            {
                count++;
            }
        }

        return count;
    }

    public int TotalFreeParkingSlot()
    {
        int count = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].isLocked)
            {
                if (slots[i].parkedShooter == null || (slots[i].parkedShooter != null && !parkedShooters.Contains(slots[i].parkedShooter)))
                {
                    count++;
                }
            }
        }

        return count;
    }

    public ParkingSlot GetFreeSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].isLocked && slots[i].parkedShooter == null)
            {
                return slots[i];
            }
        }

        return null;
    }

    public bool CanAddSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isLocked)
            {
                return true;
            }
        }

        return false;
    }

    public void AddSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isLocked)
            {
                //slots[i].Init(false);
                ParkingSlot slot = slots[i];
                EventManager.EmitEventData(EventVariables.AddSlot, slot);
                return;
            }
        }
    }

    public bool Is2SlotAvailable(out ParkingSlot slot1, out ParkingSlot slot2)
    {
        ParkingSlot[] freeSlots = new ParkingSlot[2];

        int count = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].isLocked && slots[i].parkedShooter == null)
            {
                freeSlots[count] = slots[i];
                count++;
            }

            if (count == 2)
            {
                break;
            }
        }

        if (count == 2)
        {
            slot1 = freeSlots[0];
            slot2 = freeSlots[1];
            return true;
        }
        else
        {
            slot1 = null;
            slot2 = null;
            return false;
        }
    }

    public bool Magnet()
    {
        if (parkedShooters.Count > 0)
        {
            for (int i = 0; i < parkedShooters.Count; i++)
            {
                if (parkedShooters[i].realCapacity > 0)
                {
                    List<ScrewController> screws = GameplayController.Instance.levelController.screws;
                    List<ScrewController> screwsToFill = new List<ScrewController>();
                    for (int j = 0; j < screws.Count; j++)
                    {
                        if (screws[j].color == parkedShooters[i].color)
                        {
                            screwsToFill.Add(screws[j]);

                            if (screwsToFill.Count == parkedShooters[i].realCapacity)
                            {
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < screwsToFill.Count; j++)
                    {
                        parkedShooters[i].Shoot(screwsToFill[j]);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public bool Undo()
    {
        if (parkedShooters.Count > 0)
        {
            for (int i = parkedShooters.Count - 1; i >= 0; i--)
            {
                if (parkedShooters[i].slot != null && parkedShooters[i] != null && parkedShooters[i].CanUndo())
                {
                    parkedShooters[i].Undo();
                    return true;
                }
            }
        }

        return false;
    }

    public bool Clear()
    {
        bool canClear = false;

        int count = 0;
        for (int i = 0; i < slotsToClear.Count; i++)
        {
            if (!slotsToClear[i].isLocked && slotsToClear[i].parkedShooter != null && parkedShooters.Contains(slotsToClear[i].parkedShooter) && slotsToClear[i].extraParkedShooter == null)
            {
                ShooterController parkedShooter = slotsToClear[i].parkedShooter;
                if (!parkedShooter.isShooting && parkedShooter.realCapacity > 0)
                {
                    parkedShooter.ClearFromSlot();
                    canClear = true;
                    count++;
                }
            }

            if (count == 3)
            {
                break;
            }
        }

        if (count < 3)
        {
            for (int i = 0; i < slotsToClear.Count; i++)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].parkedShooter != null && parkedShooters.Contains(slotsToClear[i].parkedShooter))
                {
                    ShooterController parkedShooter = slotsToClear[i].parkedShooter;
                    if (!parkedShooter.isShooting && parkedShooter.realCapacity > 0)
                    {
                        ParkingSlot extraSlot = FindFreeExtraSlot(i);
                        if (extraSlot != null)
                        {
                            parkedShooter.ClearToSlot(extraSlot);
                            canClear = true;
                            count++;
                        }
                    }
                }

                if (count == 3)
                {
                    break;
                }
            }
        }

        return canClear;
    }

    private ParkingSlot FindFreeExtraSlot(int index)
    {
        if (index <= 3)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].extraParkedShooter == null)
                {
                    return slotsToClear[i];
                }
            }

            for (int i = index + 1; i < slotsToClear.Count; i++)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].extraParkedShooter == null)
                {
                    return slotsToClear[i];
                }
            }
        }
        else
        {
            for (int i = index + 1; i < slotsToClear.Count; i++)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].extraParkedShooter == null)
                {
                    return slotsToClear[i];
                }
            }

            for (int i = index - 1; i >= 0; i--)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].extraParkedShooter == null)
                {
                    return slotsToClear[i];
                }
            }
        }

        return null;
    }

    public bool CanClear()
    {
        if (IsAnyExtraSlotAvailable())
        {
            for (int i = 0; i < slotsToClear.Count; i++)
            {
                if (!slotsToClear[i].isLocked && slotsToClear[i].parkedShooter != null && parkedShooters.Contains(slotsToClear[i].parkedShooter))
                {
                    ShooterController parkedShooter = slotsToClear[i].parkedShooter;
                    if (!parkedShooter.isShooting && parkedShooter.realCapacity > 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsAnyExtraSlotAvailable()
    {
        for (int i = 0; i < slotsToClear.Count; i++)
        {
            if (!slotsToClear[i].isLocked && slotsToClear[i].extraParkedShooter == null)
            {
                return true;
            }
        }

        return false;
    }

    public void Recycle()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Recycle();
        }

        for (int i = 0; i < parkedShooters.Count; i++)
        {
            parkedShooters[i].Recycle();
        }
        parkedShooters.Clear();
    }
}

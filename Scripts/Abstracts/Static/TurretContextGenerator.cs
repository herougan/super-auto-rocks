using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class TurretContextGenerator
{
    public static Dictionary<TurretContextAction.Type, string> tcaToString = new Dictionary<TurretContextAction.Type, string>() {
        [TurretContextAction.Type.DestroyRubble] = "Destroy",
        [TurretContextAction.Type.Sell] = "Sell",
        [TurretContextAction.Type.Combine] = "Combine",
        [TurretContextAction.Type.Lock] = "Lock",
        [TurretContextAction.Type.Unlock] = "Unlock",
        [TurretContextAction.Type.Freeze] = "Freeze",
        [TurretContextAction.Type.Unfreeze] = "Unfreeze",
    };
    public static List<TurretContextAction> GenerateContextActions(Turret turret) {
        List<TurretContextAction> contextActions = new List<TurretContextAction>();

        if (Battler.IsRubble(turret))
            contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.DestroyRubble});
        else if (turret.player.type == Player.Type.Shop) {
            if (!turret.frozen)
                contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Freeze});
            else
                contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Unfreeze});
        } else {
            contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Combine});
            contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Sell});
            if (turret.locked) 
                contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Unlock});
            else 
                contextActions.Add(new TurretContextAction() {type = TurretContextAction.Type.Lock});
        }

        return contextActions;
    }
}

public class TurretContextAction {
    public int value;
    public Type type;
    public TurretName name;

    public enum Type {
        DestroyRubble,
        Sell,
        Combine,
        Lock,
        Unlock,
        Freeze,
        Unfreeze,
    }
}
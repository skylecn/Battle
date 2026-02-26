#if UNITY_EDITOR
using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CityBattle.Logic;
using Collision2d;

namespace Game.CityBattle.Entity
{

public partial class CharacterEntity
{
    public Debugger debugger;

    public void StartDebugger()
    {
        debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
        var characterObj = BattleWorld.Instance.GetCharacter(Id);
        debugger.BehaviorTree = characterObj.behaviourTree.Root;
        var physicsBody = gameObject.AddComponent<CharacterPhysicsBody>();
        physicsBody.InternalBody = characterObj.physicsBody;

        Gizmos.color = characterObj.camp == BattleCamp.Friend ? Color.green : Color.red;
    }

    public void StopDebugger()
    {
        if (debugger != null)
        {
            Object.Destroy(debugger);
        }
        }
}
public class CharacterPhysicsBody : MonoBehaviour
{
    public PhysicsBody InternalBody;


        private void OnDrawGizmos()
        {
            if (InternalBody == null) return;
            // bool hit = InternalBody.CurrentCollisions.Count > 0;
            // Gizmos.color = hit ? Color.red : Color.green;
            if (InternalBody.ShapeData is Circle c){
                Gizmos.DrawWireSphere(new Vector3(c.Center.x, 0, c.Center.y), c.Radius);
            }
            else if (InternalBody.ShapeData is AABB a)
            {
                Gizmos.DrawWireCube(new Vector3(a.Center.x, 0, a.Center.y), a.Size);
            }
            else if (InternalBody.ShapeData is OBB o)
            {
                Gizmos.DrawWireCube(new Vector3(o.Center.x, 0, o.Center.y), o.Size);
            }
        }
}

}


#endif

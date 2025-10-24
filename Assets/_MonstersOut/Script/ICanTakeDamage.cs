using UnityEngine;
namespace RGame
{
    public interface ICanTakeDamage
    {
        //The take damage event
        void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null);
    }
}
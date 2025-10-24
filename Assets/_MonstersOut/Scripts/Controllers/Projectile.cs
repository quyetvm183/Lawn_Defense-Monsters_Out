using UnityEngine;
using System.Collections;
namespace RGame
{
    public abstract class Projectile : MonoBehaviour
    {
        //set the speed for the projectile
        public float Speed = 3;
        //Set the layer of the target
        public LayerMask LayerCollision;

        public GameObject Owner { get; private set; }
        public Vector2 Direction { get; private set; }
        public Vector2 InitialVelocity { get; private set; }
        public bool CanGoBackOwner { get; private set; }
        public float NewDamage { get; private set; }

        [HideInInspector]
        public bool Explosion;

        //protected float Damage;
        protected Vector2 force;
        protected WeaponEffect weaponEffect;

        // Use this for initialization
        public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity, bool isExplosion = false, bool canGoBackToOwner = false, float _newDamage = 0, WeaponEffect _weaponEffect = null)
        {
            //make the object facing to the direction
            transform.right = direction;
            //set the owner for the object
            Owner = owner;
            Direction = direction;
            //the start velocity for the projectile
            InitialVelocity = initialVelocity;
            CanGoBackOwner = canGoBackToOwner && isExplosion;
            NewDamage = _newDamage;
            weaponEffect = _weaponEffect;

            Explosion = isExplosion;
            //trigger the finish Init event
            OnInitialized();
        }
        //Init the new parameters for the projectile object
        public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity, bool isExplosion = false, float Mindamage = 0, float MaxDamage = 0, float critPercent = 0, Vector2 _force = default(Vector2), WeaponEffect _weaponEffect = null)
        {
            //set the owner for the object
            Owner = owner;
            //set the new move direction
            Direction = direction;
            //the start velocity for the projectile
            InitialVelocity = initialVelocity;
            force = _force;
            //save the new damage
            NewDamage = Random.Range(Mindamage, MaxDamage);
            Explosion = isExplosion;
            weaponEffect = _weaponEffect;
            //trigger the finish Init event
            OnInitialized();
        }

        public virtual void OnInitialized()
        {
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            //check if the trigger object hasn't the layer in the layer list
            if ((LayerCollision.value & (1 << other.gameObject.layer)) == 0)
            {
                OnNotCollideWith(other);
                return;
            }
            //if the projectile contact with the owner, trigger the Collide with Owner action
            var isOwner = Owner == other.gameObject;
            if (isOwner)
            {
                OnCollideOwner();
                return;
            }

            //If contact with the target with the layer in the layer list, try to get the damage script
            var takeDamage = (ICanTakeDamage)other.gameObject.GetComponent(typeof(ICanTakeDamage));
            //If the target has the damage script, deal the damage to it
            if (takeDamage != null)
            {

                if (other.gameObject.GetComponent(typeof(Projectile)) != null)
                {
                    var otherProjectile = (Projectile)other.gameObject.GetComponent(typeof(Projectile));
                    if (Owner == otherProjectile.Owner)
                        return;
                    //If the target is the projectile too, set contact with the other
                    OnCollideOther(other);
                }
                else
                    //Deal damage to the target
                    OnCollideTakeDamage(other, takeDamage);
                return;
            }
            else
            {
                //Check the body part
                var takeBodyDamage = (ICanTakeDamageBodyPart)other.gameObject.GetComponent(typeof(ICanTakeDamageBodyPart));
                if (takeBodyDamage != null)
                {
                    //Deal the damage to the body part
                    OnCollideTakeDamageBodyPart(other, takeBodyDamage);
                }
                else
                    //if the target don't have the bodypart, set contact with the Other
                    OnCollideOther(other);
            }
        }

        protected virtual void OnNotCollideWith(Collider2D other)
        {
        }

        protected virtual void OnCollideOwner() { }

        protected virtual void OnCollideTakeDamage(Collider2D other, ICanTakeDamage takedamage) { }

        protected virtual void OnCollideTakeDamageBodyPart(Collider2D other, ICanTakeDamageBodyPart takedamage) { }

        protected virtual void OnCollideOther(Collider2D other) { }
    }
}
using UnityEngine;

namespace Model
{
    public interface IDamageableController{
        IDamageable Damageable {get;}
        GameObject GameObject {get;}
    }
}
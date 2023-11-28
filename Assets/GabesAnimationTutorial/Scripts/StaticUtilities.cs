using UnityEngine;

public static class StaticUtilities
{
    // 1) Animations
    public static readonly int XSpeedAnimId = Animator.StringToHash("xSpeed");
    public static readonly int YSpeedAnimId = Animator.StringToHash("ySpeed");
    public static readonly int IdleAnimId = Animator.StringToHash("IdleState");
    public static readonly int AttackAnimId = Animator.StringToHash("Attack");
    public static readonly int TurnAnimId = Animator.StringToHash("Turn");
    public static readonly int IsTurningAnimId = Animator.StringToHash("IsTurning");

    // 2) Layers
    public static readonly int GroundLayer = 1 << LayerMask.NameToLayer("Ground");
    public static readonly int PlayerLayer = 1 << LayerMask.NameToLayer("Player");
    public static readonly int EnemyLayer =  1  << LayerMask.NameToLayer("Enemy");
    
    public static readonly int MoveLayerMask = PlayerLayer | GroundLayer | EnemyLayer;
    public static readonly int AttackLayerMask = GroundLayer | EnemyLayer;

    // 3) Shaders
    public static readonly int Color = Shader.PropertyToID("_Color");
}

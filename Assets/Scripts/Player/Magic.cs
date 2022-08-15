using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Magic")]
public class Magic : Item
{
    [Header("Basic Magic Info")]
    [SerializeField] private MagicType _magicType;
    [SerializeField] private int _knowledgePoints = 10;
    [SerializeField] private int _pageAmount = 1;
    [SerializeField, ReadOnly] private bool _canAttack;
    [SerializeField, ReadOnly] private bool _canDefend;
    [SerializeField, ReadOnly] private bool _canInstruct;

    [Header("Magic Type Specifics")]
    // Attack specific
    [SerializeField] private bool _canDamage = false;
    [ShowIf("_canDamage")] [SerializeField] private int _damage = 1;
    [SerializeField] private bool _hasKnockback = false;

    // Defense specific
    [SerializeField] private bool _canHeal = false;
    [ShowIf("_canHeal")] [SerializeField] private int _heal = 1;
    [SerializeField] private bool _canMitigate = false;
    [ShowIf("_canMitigate")] [SerializeField] private int _mitigation = 1;

    [Header("Etc")]
    [SerializeField] private bool _isElemental = false;
    [ShowIf("_isElemental")] [SerializeField] private bool _elementFire = false;
    [ShowIf("_isElemental")] [SerializeField] private bool _elementElectricity = false;
    [SerializeField] private bool _isAOE = false;
    [ShowIf("_isAOE")] [SerializeField] private float _aoeSize = 5;
    [ShowIf("_isAOE")] [SerializeField] private float _aoePotency = 1;
    [SerializeField] private bool _isTimed = false;
    [ShowIf("_isTimed")] [SerializeField] private float _duration = 50;


    // Public Variable Storage
    public string MagicName => ItemName;
    public MagicType MagicType => _magicType;
    public int KnowledgePoints => _knowledgePoints;
    public int PageAmount => _pageAmount;

    /*public bool CanAttack => _canAttack;
    public bool CanDefend => _canDefend;
    public bool CanInstruct => _canInstruct;
    */
    public bool CanDamage => _canDamage;
    public int Damage => _canDamage ? _damage : 0;
    public bool Knockback => _hasKnockback;
    public bool CanHeal => _canHeal;
    public int Heal => _canHeal ? _heal : 0;
    public bool CanMitigate => _canMitigate;
    public int Mitigation => _canMitigate ? _mitigation : 0;

    public bool IsElemental => _isElemental;
    public bool ElementFire => _elementFire;
    public bool ElementElectricity => _elementElectricity;
    public bool IsAOE => _isAOE;
    public float AOESize => _isAOE ? _aoeSize : 0;
    public float AOEPotency => _isAOE ? _aoePotency : 0;
    public bool IsTimed => _isTimed;
    public float Duration => _isTimed ? _duration : 0;

    protected override void OnValidate()
    {
        base.OnValidate();
        _canAttack = (IsAttack);
        _canDefend = (IsDefense);
        _canInstruct = (IsInstruction);
    }

    public bool IsAttack => _magicType is MagicType.Attack /*MagicType.Flame or MagicType.Zap or MagicType.Woosh*/;
    public bool IsDefense => _magicType is MagicType.Defense /*MagicType.Heal or MagicType.Block or MagicType.Dodge or MagicType.Speed*/;
    public bool IsInstruction => _magicType is MagicType.Instruction /*MagicType.Flourish or MagicType.Fix*/;

    public void CastSpell()
    {
        // TODO: Cast
        Debug.Log($"Casting {MagicName}");
    }
}
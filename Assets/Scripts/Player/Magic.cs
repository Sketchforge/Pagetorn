using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Magic")]
public class Magic : ScriptableObject
{
    [Header("Basic Magic Info")]
    [SerializeField, ReadOnly] private string _magicName;
    [SerializeField] private MagicType _type;
    [SerializeField, TextArea] private string _description = "";
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _knowledgePoints = 10;
    [SerializeField] private int _pageAmount = 1;
    [SerializeField, ReadOnly] private bool _canAttack;
    [SerializeField, ReadOnly] private bool _canDefend;
    [SerializeField, ReadOnly] private bool _canInstruct;

    [Header("Magic Type Specifics")]
    // Attack specific
    [ShowIf("_canAttack")] [SerializeField] private bool _canDamage = false;
    [ShowIf("_canDamage")] [SerializeField] private int _damage = 1;
    [ShowIf("_canAttack")] [SerializeField] private bool _hasKnockback = false;

    // Defense specific
    [ShowIf("_canDefend")] [SerializeField] private bool _canHeal = false;
    [ShowIf("_canHeal")] [SerializeField] private int _heal = 1;
    [ShowIf("_canDefend")] [SerializeField] private bool _canMitigate = false;
    [ShowIf("_canMitigate")] [SerializeField] private int _mitigation = 1;

    [Header("Etc")]
    [SerializeField] private bool _isAOE = false;
    [ShowIf("_isAOE")] [SerializeField] private float _aoeSize = 5;
    [ShowIf("_isAOE")] [SerializeField] private float _aoePotency = 1;
    [SerializeField] private bool _isTimed = false;
    [ShowIf("_isTimed")] [SerializeField] private float _duration = 50;


    // Public Variable Storage
    public string MagicName => _magicName;
    public MagicType Type => _type;
    public int KnowledgePoints => _knowledgePoints;
    public Sprite Sprite => _sprite;
    public int PageAmount => _pageAmount;

    public bool CanAttack => _canAttack;
    public bool CanDefend => _canDefend;
    public bool CanInstruct => _canInstruct;
    public bool CanDamage => _canAttack ? _canDamage : false;
    public int Damage => _canDamage ? _damage : 0;
    public bool Knockback => _canAttack ? _hasKnockback : false;
    public bool CanHeal => _canDefend ? _canHeal : false;
    public int Heal => _canHeal ? _heal : 0;
    public bool CanMitigate => _canDefend ? _canMitigate : false;
    public int Mitigation => _canMitigate ? _mitigation : 0;

    public bool IsAOE => _isAOE;
    public float AOESize => _isAOE ? _aoeSize : 0;
    public float AOEPotency => _isAOE ? _aoePotency : 0;
    public bool IsTimed => _isTimed;
    public float Duration => _isTimed ? _duration : 0;

    private void OnValidate()
    {
        _magicName = name.Replace("SC", "SoftCover").Replace("HC", "HardCover").Replace(" ", "");
        _canAttack = (IsAttack);
        _canDefend = (IsDefense);
        _canInstruct = (IsInstruction);
    }

    public bool IsAttack => _type is MagicType.FireAttack or MagicType.LightningAttack /*MagicType.Flame or MagicType.Zap or MagicType.Woosh*/;
    public bool IsDefense => _type is MagicType.Defense /*MagicType.Heal or MagicType.Block or MagicType.Dodge or MagicType.Speed*/;
    public bool IsInstruction => _type is MagicType.Instruction /*MagicType.Flourish or MagicType.Fix*/;

}
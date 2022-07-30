using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Magic")]
public class Magic : ScriptableObject
{
    [Header("Basic Magic Info")]
    [SerializeField, ReadOnly] private string _magicName;
    [SerializeField] private MagicType _type;
    [SerializeField, TextArea] private string _description = "";
    [SerializeField] private Sprite _sprite;
    [SerializeField, ReadOnly] private bool _canAttack;
    [SerializeField, ReadOnly] private bool _canDefend;

    [Header("Magic Type Specifics")]
    [ShowIf("_canAttack")] [SerializeField] private bool _canDamage = false;    // attack specific
    [ShowIf("_canDamage")] [SerializeField] private int _damage = 1;
    [ShowIf("_canDamage")] [SerializeField] private bool _hasKnockback = false;

    [ShowIf("_canDefend")] [SerializeField] private bool _canHeal = false;      // defense specific
    [ShowIf("_canHeal")] [SerializeField] private int _heal = 1;
    [ShowIf("_canDefend")] [SerializeField] private bool _canMitigate = false;
    [ShowIf("_canMitigate")] [SerializeField] private int _mitigation = 1;

    [Header("Etc")]
    [SerializeField] private int _pageAmount = 1;
    [SerializeField] private bool _isAOE = false;
    [ShowIf("_isAOE")] [SerializeField] private float _aoeSize = 5;
    [ShowIf("_isAOE")] [SerializeField] private float _aoePotency = 1;
    [SerializeField] private bool _isTimed = false;
    [ShowIf("_isTimed")] [SerializeField] private float _time = 50;


    public string MagicName => _magicName;
    public MagicType Type => _type;
    public Sprite Sprite => _sprite;
    public int PageAmount => _pageAmount;
    public bool CanAttack => _canAttack;
    public bool CanDefend => _canDefend;

    private void OnValidate()
    {
        //_magicName = name.Replace("SC", "SoftCover").Replace("HC", "HardCover").Replace(" ", "");
        _canAttack = (IsAttack);
        _canDefend = (IsDefense);
    }

    public bool IsAttack => _type is MagicType.Attack /*MagicType.Flame or MagicType.Zap or MagicType.Woosh*/;
    public bool IsDefense => _type is MagicType.Defense /*MagicType.Heal or MagicType.Block or MagicType.Dodge or MagicType.Speed*/;
    public bool IsInstruction => _type is MagicType.Instruction /*MagicType.Flourish or MagicType.Fix*/;

}
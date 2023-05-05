using CoffeyUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Item/Magic")]
public class Magic : Item
{
    [Header("Basic Magic Info")]
    [SerializeField] private MagicType _magicType;
    [SerializeField] public int _knowledgePoints = 10;
    [SerializeField] public int _pageAmount = 1;
    [SerializeField, ReadOnly] private bool _canAttack;
    [SerializeField, ReadOnly] private bool _canDefend;
    [SerializeField, ReadOnly] private bool _canInstruct;

    [Header("Magic Type Specifics")]
    // Attack specific
    [SerializeField] private bool _canDamage = false;
    [ShowIf("_canDamage")] [SerializeField] private int _damage = 1;
    [SerializeField] private bool _isProjectile = false;
    [ShowIf("_isProjectile")] [SerializeField] private float _projectileSpeed = 0;
    //[ShowIf("_canDamage")] [SerializeField] private float _projectileLifetime = 10f;
    [SerializeField] private bool _hasKnockback = false;

    // Defense specific
    [SerializeField] private bool _canHeal = false;
    [ShowIf("_canHeal")] [SerializeField] private int _heal = 1;
    [SerializeField] private bool _canMitigate = false;
    [ShowIf("_canMitigate")] [SerializeField] private int _mitigation = 1;

    [Header("Etc")]
    //[SerializeField] private Animation _animation;
    [SerializeField] private bool _isElemental = false;
    [ShowIf("_isElemental")] [SerializeField] private bool _elementFire = false;
    [ShowIf("_isElemental")] [SerializeField] private bool _elementElectricity = false;
    [SerializeField] private float _activeRadius = 1;
    [SerializeField] private bool _canTrack = false;
    [SerializeField] private bool _canChain = false;
    [ShowIf("_canChain")] [SerializeField] private float _chainRange = 5;
    [SerializeField] private bool _isAOE = false;
    [ShowIf("_isAOE")] [SerializeField] private float _aoeSize = 5;
    [ShowIf("_isAOE")] [SerializeField] private float _aoePotency = 1;
    [SerializeField] private bool _isTimed = false;
    [ShowIf("_isTimed")] [SerializeField] private float _duration = 50;
    private float _timer = 1;
    private bool _isCurrentlyTimed = false;

    [SerializeField] private GameObject _spellItem = null;
    public GameObject SpellItem => _spellItem;


    // Public Variable Storage
    public string MagicName => ItemName;
    public MagicType MagicType => _magicType;
    public int KnowledgePoints => _knowledgePoints;
    public int PageAmount => _pageAmount;
    private int _pageAmountEdit;

    /*public bool CanAttack => _canAttack;
    public bool CanDefend => _canDefend;
    public bool CanInstruct => _canInstruct;
    */
    public bool CanDamage => _canDamage;
    public int Damage => _canDamage ? _damage : 0;
    public float ActiveRadius => _activeRadius;
    public bool IsProjectile => _isProjectile;
    public float ProjectileSpeed => _isProjectile ? _projectileSpeed : 0;
    //public float ProjectileLifetime => _canDamage ? _projectileLifetime : 0;
    public bool Knockback => _hasKnockback;
    public bool CanHeal => _canHeal;
    public int Heal => _canHeal ? _heal : 0;
    public bool CanMitigate => _canMitigate;
    public int Mitigation => _canMitigate ? _mitigation : 0;

    public bool IsElemental => _isElemental;
    public bool ElementFire => _elementFire;
    public bool ElementElectricity => _elementElectricity;
    public bool CanTrack => _canTrack;
    public bool CanChain => _canChain;
    public float ChainRange => _canChain ? _chainRange : 0;
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
        _pageAmountEdit = PageAmount;
        _timer = Duration;
    }

    public bool IsAttack => _magicType is MagicType.Attack /*MagicType.Flame or MagicType.Zap or MagicType.Woosh*/;
    public bool IsDefense => _magicType is MagicType.Defense /*MagicType.Heal or MagicType.Block or MagicType.Dodge or MagicType.Speed*/;
    public bool IsInstruction => _magicType is MagicType.Instruction /*MagicType.Flourish or MagicType.Fix*/;

    public void CastSpell(InventoryItemSlot slot)
    {
        // TODO: Cast
        Debug.Log($"Casting {MagicName}");
        
        // BUG: Only 1 timed spell can be cast at a time
        if (_isCurrentlyTimed) {
            Debug.Log("Hey you are already using a timed spell");
            Debug.Log("Time left on spell: " + (_timer));
            return;
        }

        if (PlayerManager.Instance.Survival.GetStat(SurvivalStatEnum.MagicPoints) >= KnowledgePoints && slot.ItemHealth >= 0) // find if player has enough points to cast
        {
            GameObject spellObject = Instantiate(_spellItem, PlayerManager.Actions.LookDirection.position, PlayerManager.Actions.LookDirection.rotation);
            SpellManager spell = spellObject.GetComponent<SpellManager>();

            spell.OnCast(this);

            //_animation.Play();

            //if (Knockback) ;
            //if (CanTrack) GameObject.transform.position; // target an object near cursor (possible raycast/cone? similar to crafting block) to track or affect primarily
            //if (IsAOE) create sphere for aoe, possibly do more/less damage away from center using aoePotency?

            slot.DamageItem(1);
        }
        else Debug.Log($"Whoops can't cast lol loser (not enough KP to cast)");

        if (CanMitigate && !IsTimed)
            Debug.LogError($"Mitigation has to be timed to be used!");
    }
}
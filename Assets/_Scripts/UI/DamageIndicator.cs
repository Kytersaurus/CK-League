using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private TMP_Text _damageValue;
    [SerializeField] private float _initialYVelocity = 7f;
    [SerializeField] private float _inititalXVelocityRange = 3f;
    [SerializeField] private float _lifeTime = 0.8f;
    [SerializeField] private Color _regularColour;
    [SerializeField] private Color _blockedColour;
    [SerializeField] private Color _healColour;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _damageValue = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _rigidbody.linearVelocity = new Vector2(Random.Range(-_inititalXVelocityRange, _inititalXVelocityRange), _initialYVelocity);
        Destroy(gameObject, _lifeTime);
    }
    public void SetupMessage(string message, bool blocked, bool heal)
    {
        _damageValue.SetText(message);
        Color colour = _regularColour;
        if (blocked)
        {
            colour = _blockedColour;
        }
        if (heal)
        {
            colour = _healColour;   
        }
        _damageValue.color = colour;
    }
}

using UnityEngine;

public class CapsuleScript : MonoBehaviour
{
    private Vector3 _startPos;

    [SerializeField] private ItemSO item;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPos = transform.position;
        spriteRenderer.sprite = item.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10)
        {
            transform.position = _startPos;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    public ItemSO GetItem()
    {
        return item;
    }

    public void SetItem(ItemSO itemSo)
    {
        this.item = itemSo;
        spriteRenderer.sprite = itemSo.sprite;
    }
    
}

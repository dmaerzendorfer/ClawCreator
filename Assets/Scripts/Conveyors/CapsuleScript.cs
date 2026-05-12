using UnityEngine;

public class CapsuleScript : MonoBehaviour
{
    private Vector3 _startPos;

    [SerializeField] private ItemSO item;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer capsuleSpriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPos = transform.position;
        spriteRenderer.sprite = item.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -100)
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
        if (item.name.StartsWith("Eyes"))
        {
            capsuleSpriteRenderer.color = Color.white;
        } else if (item.name.StartsWith("Headwear"))
        {
            capsuleSpriteRenderer.color = Color.red;
        }  else if (item.name.StartsWith("Nose")) 
        {
            capsuleSpriteRenderer.color = Color.green;
        } else if (item.name.StartsWith("Mouth")) 
        {
            capsuleSpriteRenderer.color = Color.blue;
        }  else if (item.name.StartsWith("Outfit")) 
        {
            capsuleSpriteRenderer.color = Color.yellow;
        }
    }

    public void resetPosition()
    {
        transform.position = _startPos;
        transform.localScale = Vector3.one;
    }
}
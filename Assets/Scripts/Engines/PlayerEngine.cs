using System.Collections;
using UnityEngine;

public class PlayerEngine : Singelton<PlayerEngine>
{
    public bool GodMode;
    public Sprite[] sprites;

    private Vector2 velocity = Vector2.zero;
    private Vector2 gravity = new Vector2(0, -8f);
    private Vector2 tapVelocity = new Vector2(0, 2.5f);
    private readonly float maxSpeed = 8f;
    private readonly float forwardSpeed = 2.5f;

    private bool isDead;

    private float spriteSwapCounter = 0.1f;

    private SpriteRenderer spriteRenderer;

    public Vector2 Position 
    {
        get 
        {
            return transform.position;
        }
        private set 
        {
            transform.position = value;
        }
    }

    public bool IsFalling 
    {
        get 
        {
            return velocity.y < 0;
        }
    }  

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() 
    {
        CameraEngine.Instance.ChangeTarget(transform);

        StartCoroutine(IFly());       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionGameObject = collision.gameObject;

        switch (collisionGameObject.layer)
        {
            //Collectable
            case 8:

                collision.gameObject.SetActive(false);

                break;

            //Obstacle
            case 9:

                if (isDead == false && GodMode == false)
                    Die();

                break;

            default:

                break;
        }      
    }

    private void Die()
    {
        isDead = true;
        spriteRenderer.sprite = sprites[2];

        GameManager.Instance.ChangeGameState(GAME_STATE.END);
    }

    private IEnumerator IFly()
    {
        var angle = 0f;

        while (isDead == false)
        {
            var foo = Position.x;

            velocity.x = forwardSpeed;
            velocity += gravity * Time.deltaTime;

            if (InputManager.Instance.Tap)
            {
                if (IsFalling)
                {
                    velocity.y = 0;
                }

                velocity += tapVelocity;
            }

            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
            Position += velocity * Time.deltaTime;
           
            angle = 0f;

            if (IsFalling)
            {
                angle = Mathf.Lerp(0, -60, -velocity.y / maxSpeed);
                spriteRenderer.sprite = sprites[0];
            }
            else
            {
                spriteRenderer.sprite = sprites[1];
                spriteSwapCounter -= Time.deltaTime;

                if (spriteSwapCounter < 0)
                {
                    spriteRenderer.sprite = sprites[0];
                    spriteSwapCounter = 0.1f;
                }
            }

            transform.rotation = Quaternion.Euler(0, 0, angle);

            AreWeInPlayArea();

            GameMaster.Instance.NeedIncreaseScore();

            yield return null;
        }     
    }

    private void AreWeInPlayArea()
    {
        if (Position.y > 6.6f)
        {
            velocity = new Vector2(velocity.x, 0f);
            Position = new Vector2(Position.x, 6.6f);
        }

        if (Position.y < -3.35f)
        {
            Position = new Vector2(Position.x, -3.35f);
            transform.rotation = Quaternion.Euler(Vector2.zero);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Mech : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public ContactFilter2D filter = new ContactFilter2D();
    RaycastHit2D[] hits = new RaycastHit2D[10];

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool onCeiling;
    public int wallSide;

    [Space]

    [Header("Collision")]
    public BoxCollider2D m_Collider;
    [SerializeField] public float distance = 0.25f;
/*
    [SerializeField] public float bottomHeight = 0.25f;
    [SerializeField] public float topHeight = 0.25f;
    [SerializeField] public float leftWidth = 0.25f;
    [SerializeField] public float rightWidth = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset, topOffset;
*/

    private Color debugCollisionColor = Color.red;
    private Vector2 bottomSize,leftSize, rightSize, topSize;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<BoxCollider2D>();
        bottomSize = new Vector2(m_Collider.size.x, distance);
        leftSize = new Vector2(distance, m_Collider.size.y);
        rightSize = new Vector2(distance, m_Collider.size.y);
        topSize = new Vector2(m_Collider.size.x, distance);

        
    }

    // Update is called once per frame
    void Update()
    {  
        int numHitsUp = m_Collider.Cast(Vector2.up, filter, hits, distance);
        int numHitsDown = m_Collider.Cast(Vector2.down, filter, hits, distance);
        int numHitsLeft = m_Collider.Cast(Vector2.left, filter, hits, distance);
        int numHitsRight = m_Collider.Cast(Vector2.right, filter, hits, distance);

        if(numHitsUp > 0)
        {
            onCeiling = true;
        }else
        {
            onCeiling = false;
        }
        if(numHitsDown > 0)
        {
            onGround = true;
        }else
        {
            onGround = false;
        }
        if(numHitsLeft > 0)
        {
            onLeftWall = true;
            onWall = true;
        }else
        {
            onLeftWall = false;
            
        }
        if(numHitsRight > 0)
        {
            onRightWall = true;
            onWall = true;
        }else
        {
            onRightWall = false;
            
        }
        if(numHitsRight == 0 && numHitsLeft == 0)
        {
            onWall = false;
        }
        /*
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, (Vector2)bottomSize, groundLayer);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, (Vector2)rightSize, groundLayer) 
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, (Vector2)leftSize, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, (Vector2)rightSize, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, (Vector2)leftSize, groundLayer);
        onCeiling = Physics2D.OverlapBox((Vector2)transform.position + topOffset, (Vector2)topSize, groundLayer);
        */

        wallSide = onRightWall ? -1 : 1; 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset, topOffset };

        Gizmos.DrawWireCube((Vector2)transform.position  + Vector2.up * distance, (Vector2)bottomSize);
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.down * distance, (Vector2)rightSize);
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.left * distance, (Vector2)leftSize);
        Gizmos.DrawWireCube((Vector2)transform.position  + Vector2.right * distance, (Vector2)topSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocityManager : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Vector2 velocity;


    // Start is called before the first frame update
    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

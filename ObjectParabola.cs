using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParabola : MonoBehaviour
{   
    [SerializeField]
    public float width = 4f, 
        speed,
        height = 6f;

    [HideInInspector]
    private bool jumped;

    private float parabolaTime;
    private Vector2 startPos; 

    private void Start()
    {
        startPos = this.transform.position; 
    }

    private void Update()
    {
        parabolaTime += Time.deltaTime * speed / 10;
        transform.position = Parabola(startPos, height, parabolaTime);
        if( parabolaTime > 0.5f && jumped)
        {
            jumped = false;
        }
    }
    private void OnEnable()
    {
        parabolaTime = 0f;
        startPos = this.transform.position;
    }

    public Vector2 Parabola(Vector2 start, float height, float time)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x; //funkcja kwadratowa 
        return new Vector2(start.x + time * width, start.y + f(time));
    }

    public Vector3 GetEndPosition()
    {
        Vector2 endpos = Parabola(startPos,height, 0.99f);
        return new Vector3 (endpos.x, endpos.y, 0f);
    }
    
    void OnCollisionEnter2D (Collision2D other)
	{
        jumped = true;
        parabolaTime = 0f;
		startPos = this.transform.position;
	}
}

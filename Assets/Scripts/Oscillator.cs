using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] //prevents script from being attached to a game object twice
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    Vector3 startingPos;

    [Range(0,1)] [SerializeField] float movementFactor; //0 for not moves, 1 for fully moved
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon) { return; } //prevents NaN

        float cycles = Time.time / period; //grows continuously from zero

        const float tau = Mathf.PI * 2; //Pi x 2, or Tau
        float rawSineWave = Mathf.Sin(tau * cycles); //creates sinewave

        movementFactor = rawSineWave / 2f; // movement factor 
        Vector3 offset = movementFactor * movementVector; //offset
        transform.position = startingPos + offset; //moves the object by startingposition plus the offset
    }
}

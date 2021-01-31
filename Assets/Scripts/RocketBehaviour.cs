using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketBehaviour : MonoBehaviour
{
    [SerializeField] float boostThrust = 100f; //Speed of thrusting
    [SerializeField] float rcsThrust = 250f; //Speed of turning
    [SerializeField] float levelLoadTime = 1f;

    [SerializeField] AudioClip mainEngine; //Engine sound
    [SerializeField] AudioClip successClip; //Win sound
    [SerializeField] AudioClip deathSound; //Death sound

    [SerializeField] ParticleSystem engineParticles; //Jet trail
    [SerializeField] ParticleSystem successParticles; //Win particles
    [SerializeField] ParticleSystem deathParticles; //Death explosion

    Rigidbody rigidBody; //3D Rigidbody for physics
    AudioSource audioSource; //Audiosource

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); // getting component
        audioSource = GetComponent<AudioSource>(); // getting component
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTransitioning) //if player is alive, execute
        {
        ThrustInput();
        RotateInput();
        }
        
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    
    private void ReloadScene() //loads scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter(Collision collision) //executes when colliding with an object
    {
        if(isTransitioning || collisionsDisabled) { return; } //ignores collision when dead

        switch (collision.gameObject.tag) //Cases for each type of object
        {
            case "Friendly":          
                break;

            case "Finish":
                SuccessSequence();
                break;

            default:
                DeathSequence();
                break;
                
        }
    }
    private void DeathSequence() //occurs when colliding with a deadly object
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("ReloadScene", levelLoadTime);
    }

    private void SuccessSequence() //occurs when getting to the end
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(successClip);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadTime);
    }

    void ThrustInput() //activates when space is pressed
    { 
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            engineParticles.Stop();
        }
    }

    private void ApplyThrust() //applies thrust movement
    {
        rigidBody.AddRelativeForce(Vector3.up * boostThrust * (Time.deltaTime * 100));
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        if (!engineParticles.isPlaying)
        {
            engineParticles.Play();
        }
        
    }
    void RotateInput() //activates when A or D is pressed
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsThrust * Time.deltaTime);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
using UnityEngine;

public class SecretMist : MonoBehaviour
{
    public float MistChance = 0.5f;
    public GameObject mist;
    private ParticleSystem ps;
    private float timer = 0f;
    private float currentRate = 2f;
    public float startRate = 2f;

    private bool paused = false;

    void Start() {
        float rand = UnityEngine.Random.Range(0, 100);
        if (rand <= MistChance) {
            currentRate = startRate;
            ps = mist.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            emission.rateOverTime = currentRate;
        }
        else {
            Destroy(mist);
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused && !paused) { 
            paused = true;
            ps.Pause();
            return; 
        }
        else if(!PlayerUI.paused && paused) {
            paused = false;
            ps.Play();
        }
        else if(PlayerUI.paused && paused) {
            return;
        }

        timer += Time.deltaTime;
        if(timer >10f){
            if(ps != null){
                currentRate += 1f;
                var emission = ps.emission;
                emission.rateOverTime = currentRate;
                timer = 0f;
            }
        }
    }
}

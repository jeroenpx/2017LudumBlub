using System.Collections;
using UnityEngine;

public class ExplodingLetterParticleControl : MonoBehaviour {

    private ParticleSystem _particleSys;
    [SerializeField]
    private int nrParticlesForTitle = 1000;
    [SerializeField]
    private float secondsToHoldTitle = 1;
    [SerializeField]
    private Sprite title;

	// Use this for initialization
	void Start () {
        StartCoroutine(ExplodeTitle());
    }

    private void CreateInitialParticles()
    {
        _particleSys = GetComponent<ParticleSystem>();
        _particleSys.Emit(nrParticlesForTitle);
        ParticleSystem.Particle[] parts = new ParticleSystem.Particle[nrParticlesForTitle];
        _particleSys.GetParticles(parts);
        float grayscale;
        
        for (int p=0; p<nrParticlesForTitle; ++p)
        {
			parts [p].remainingLifetime = parts [p].startLifetime;
            do
            {
                Vector3 newPos = Vector3.zero;
                newPos.x = Random.Range(0, title.bounds.extents.x *2);
                newPos.y = Random.Range(0, title.bounds.extents.y *2);
                grayscale = title.texture.GetPixel((int)(newPos.x * title.pixelsPerUnit), (int)(newPos.y * title.pixelsPerUnit)).grayscale;
                parts[p].position = transform.TransformPoint(newPos);
            }
            while (grayscale < .5f);
        }

        _particleSys.SetParticles(parts, nrParticlesForTitle);
        _particleSys.Pause();
    }

    private IEnumerator ExplodeTitle()
    {
		CreateInitialParticles();
		yield return new WaitForSeconds(secondsToHoldTitle);
        _particleSys.Play();
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class RescueCreature : MonoBehaviour
{
    public GameObject beforeSprite;
    public GameObject afterSprite;
    public VisualEffect rescueParticles;
    public AudioSource sfxRescue;

    public void PlayRescueEffect()
    {
        // Fade out sprite A
        var spriteA = beforeSprite.GetComponent<SpriteRenderer>();
        spriteA.DOFade(0, 1f).OnComplete(() =>
        {
            beforeSprite.SetActive(false);

            // Play particle + sound
            if (rescueParticles != null) rescueParticles.Play();
            if (sfxRescue != null) sfxRescue.Play();

            // Activate sprite B and fade in
            afterSprite.SetActive(true);
            var spriteB = afterSprite.GetComponent<SpriteRenderer>();
            spriteB.color = new Color(1, 1, 1, 0); // start transparent
            spriteB.DOFade(1, 1f);

            // Optional: scale bounce effect
            afterSprite.transform.localScale = Vector3.zero;
            afterSprite.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        });
    }
}

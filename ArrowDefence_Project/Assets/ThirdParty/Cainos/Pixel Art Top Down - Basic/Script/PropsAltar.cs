using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//when something get into the alta, make the runes glow
namespace Cainos.PixelArtTopDown_Basic
{

    public class PropsAltar : MonoBehaviour
    {
        public List<SpriteRenderer> runes;
        public float lerpSpeed;

        private Color curColor;
        private Color targetColor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            targetColor = new Color(1, 1, 1, 1);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            targetColor = new Color(1, 1, 1, 0);
        }

        private void Start()
        {
            StartCoroutine(this.ChangeColor());
        }

        private void Update()
        {
            curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

            foreach (var r in runes)
            {
                r.color = curColor;
            }
        }

        private IEnumerator ChangeColor()
        {
            float timer = 0f;

            while(true)
            {
                yield return null;

                timer += Time.deltaTime;

                if(timer <= 2f)
                {
                    targetColor = new Color(1, 1, 1, 0);
                }
                else if (timer >= 2f && timer <= 4f)
                {
                    targetColor = new Color(1, 1, 1, 1);
                }
                else
                {
                    timer = 0f;
                }
            }
        }
    }
}

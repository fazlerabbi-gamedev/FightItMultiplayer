using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class TextMeshProTextReveal : MonoBehaviour
    {
        private TextMeshPro _mTextMeshPro;

        // Sets the reveal speed in seconds.
        public float RevealSpeed = 0.01f;

        // The current page of the text, needs to be changed when you display the next page.
        public int CurrentPage = 0;

        // Lets other scripts know when to allow the next page to load.
        public bool IsWriting;

        void Awake()
        {
            IsWriting = true;
            _mTextMeshPro = GetComponent<TextMeshPro>();
        }

        IEnumerator Start()
        {
            // Force and update of the mesh to get valid information.
            _mTextMeshPro.ForceMeshUpdate();

            var totalVisibleCharacters = _mTextMeshPro.textInfo.characterCount; // Get # of Visible Character in text object
            var counter = 0;
            var visibleCount = 0;

            while (true)
            {
                visibleCount = counter % (totalVisibleCharacters + 1);

                _mTextMeshPro.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

                if (_mTextMeshPro.textInfo.pageInfo[CurrentPage].lastCharacterIndex >= visibleCount)
                {
                    IsWriting = true;
                    counter += 1;
                }
                else
                {
                    IsWriting = false;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}

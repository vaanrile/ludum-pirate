using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace BBX.Dialogue.GUI
{
    public class TextMeshAnimator : MonoBehaviour
    {
        public TMP_Text TMProText;

        private int _visibleCount;
        private int _frameCount;

        private const float WaveAmount = 0.2f;
        private const float WaveSpeed = 0.2f;
        private const float WaveSeparation = 20;
        private const float DefaultShakeAmount = 3f;

        private readonly Dictionary<int, Vector3[]> _defaultShakeVertices = new Dictionary<int, Vector3[]>();

        public float textParseSpeed =6;

        public string[] DialogueArray;
        private int _currentDialogueCount;
        public float nextDialogueDelay;

        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 1. Parse text and find characters with tags
        /// 2. Hide all the text
        /// 3. Start displaying text
        /// </summary>

        public void InitRead()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.DOFade(1, 0.5f).From(0);
            StartTextRead();
        }

        private void StartTextRead()
        {
            if (_currentDialogueCount > (DialogueArray.Length - 1))
            {
                _canvasGroup.DOFade(0, 0.5f);
                GameManager.instance.GameStart();
                return;
            }
            Parse();
            TMProText.alpha = 0;
            StartCoroutine(RunText());
        }

        
        /// <summary>
        /// Parse the custom tags into links
        /// </summary>
        private void Parse()
        {
            _defaultShakeVertices.Clear();
            TMProText.textInfo.ClearAllMeshInfo();
            var inputText = DialogueArray[_currentDialogueCount];

            const string re1 = "(<)";
            const string re2 = "(shake|wave)";
            const string re3 = "(=)";
            const string re4 = "([+-]?\\d*\\.\\d+)(?![-+0-9\\.])";
            const string re5 = "(>)";
            const string re6 = "(</)";

            inputText = Regex.Replace(inputText, re1 + re2 + re3 + re4 + re5, "<link=$2|$4>");
            inputText = Regex.Replace(inputText, re1 + re2 + re5, "<link=$2>");
            inputText = Regex.Replace(inputText, re6 + re2 + re5, "</link>");

            TMProText.text = inputText;
            TMProText.ForceMeshUpdate();
        }

        
        /// <summary>
        /// Run the text, this increases the visible characters and maxes their alpha one by one
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunText()
        {
            var textInfo = TMProText.textInfo;
            var totalVisibleCharacters = textInfo.characterCount;
            _visibleCount = 0;
            yield return new WaitForSeconds(1);
            // Wait a frame because parent layout elements do something to the first character
            yield return new WaitForEndOfFrame();

            while (_visibleCount < totalVisibleCharacters)
            {
                if (_visibleCount > totalVisibleCharacters)
                {
                    yield return new WaitForSeconds(1);
                    yield break;
                }

                Alpha(textInfo, _visibleCount, 255);
                _visibleCount += 1;

                yield return new WaitForSeconds(textParseSpeed);
            }
            yield return new WaitForSeconds(nextDialogueDelay);
            _currentDialogueCount++;
            StartTextRead();
        }

        
        /// <summary>
        /// Runs once a frame
        /// </summary>
        private void Update()
        {
            var textInfo = TMProText.textInfo;

            foreach (var link in TMProText.textInfo.linkInfo)
            {
                Animate(textInfo, link);
            }

            _frameCount++;
        }

        
        /// <summary>
        /// This sets the alpha of a character, it's used for the ticker effect
        /// </summary>
        /// <param name="textInfo"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void Alpha(TMP_TextInfo textInfo, int index, byte value)
        {
            if (!textInfo.characterInfo[index].isVisible) return;

            // Get the index of the material used by the current character.
            var materialIndex = textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex used by this text element.
            var vertexIndex = textInfo.characterInfo[index].vertexIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            var newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            for (byte corner = 0; corner < 4; corner++)
            {
                newVertexColors[vertexIndex + corner].a = value;
            }

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            TMProText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        
        /// <summary>
        /// Animate a group of characters
        /// </summary>
        /// <param name="textInfo"></param>
        /// <param name="link"></param>
        private void Animate(TMP_TextInfo textInfo, TMP_LinkInfo link)
        {
            var type = link.GetLinkID();
            var start = link.linkTextfirstCharacterIndex;
            var end = link.linkTextfirstCharacterIndex + link.linkTextLength;

            for (var i = start; i < end; i++)
            {
                if (i >= _visibleCount) break;

                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                var vertexIndex = textInfo.characterInfo[i].vertexIndex;

                var destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                if (type == "wave")
                {
                    Wave(destinationVertices, vertexIndex);
                }
                else if (type.Contains("shake"))
                {
                    Shake(destinationVertices, link, vertexIndex);
                }

                textInfo.meshInfo[materialIndex].mesh.vertices = textInfo.meshInfo[materialIndex].vertices;
                TMProText.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
            }
        }

        
        /// <summary>
        /// Animate a group of characters in a wave
        /// </summary>
        /// <param name="destinationVertices"></param>
        /// <param name="vertexIndex"></param>
        private void Wave(IList<Vector3> destinationVertices, int vertexIndex)
        {
            for (byte corner = 0; corner < 4; corner++)
            {
                var offset = WaveVector(WaveAmount, _frameCount * WaveSpeed + destinationVertices[vertexIndex + corner].x / WaveSeparation);
                destinationVertices[vertexIndex + corner] += offset;
            }
        }

        
        /// <summary>
        /// Shake a group of characters
        /// First save a copy of their original positions then add a random offset
        /// </summary>
        /// <param name="destinationVertices"></param>
        /// <param name="link"></param>
        /// <param name="vertexIndex"></param>
        private void Shake(IList<Vector3> destinationVertices, TMP_LinkInfo link, int vertexIndex)
        {
            if (!_defaultShakeVertices.ContainsKey(vertexIndex))
            {
                _defaultShakeVertices.Add(vertexIndex, destinationVertices.ToArray());
            }

            var offset = ShakeVector(ShakeAmount(link.GetLinkID()));
            for (byte corner = 0; corner < 4; corner++)
            {
                destinationVertices[vertexIndex + corner] = _defaultShakeVertices[vertexIndex][vertexIndex + corner] + offset;
            }
        }

        
        /// <summary>
        /// Parse a shake amount from the tag if it exists return default if not
        /// </summary>
        /// <param name="shake"></param>
        /// <returns></returns>
        private static float ShakeAmount(string shake)
        {
            if (shake == "shake") return DefaultShakeAmount;

            var regex = new Regex("(shake\\|)([+-]?\\d*\\.\\d+)(?![-+0-9\\.])", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(shake);

            if (!match.Success) return DefaultShakeAmount;

            var intString = match.Groups[2].ToString();
            var hasInt = float.TryParse(intString, out var number);

            return !hasInt ? DefaultShakeAmount : number;
        }

        
        /// <summary>
        /// Calculate the wave movement position
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static Vector3 WaveVector(float amount, float time)
        {
            return new Vector3(0, Mathf.Sin(time) * amount);
        }

        
        /// <summary>
        /// Calculate the shake movement position
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static Vector3 ShakeVector(float amount)
        {
            return new Vector3(Random.Range(-amount, amount), Random.Range(-amount, amount));
        }
    }
}
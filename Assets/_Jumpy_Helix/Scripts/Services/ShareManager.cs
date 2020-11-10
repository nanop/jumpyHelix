using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace OnefallGames
{
    public class ShareManager : MonoBehaviour
    {
        [Header("Native Sharing Config")]
        [SerializeField] private string shareText = "Can you beat my score!!!";
        [SerializeField] private string shareSubject = "Share With";
        [SerializeField] private string appUrl = "https://play.google.com/store/apps/details?id=com.CBGames.BlockySnake";


        [Header("Twitter Sharing Config")]
        [SerializeField] private string titterAddress = "http://twitter.com/intent/tweet";
        [SerializeField] private string textToDisplay = "Hey Guys! Check out my score: ";
        [SerializeField] private string tweetLanguage = "en";

        [Header("Facebook Sharing Config")]
        [SerializeField] private string fbAppID = "1013093142200006";
        [SerializeField] private string caption = "Check out My New Score: ";
        [Tooltip("The URL of a picture attached to this post.The Size must be atleat 200px by 200px.If you dont want to share picture, leave this field empty.")]
        [SerializeField] private string pictureUrl = "http://i-cdn.phonearena.com/images/article/85835-thumb/Google-Pixel-3-codenamed-Bison-to-be-powered-by-Andromeda-OS.jpg";
        [SerializeField] private string description = "Enjoy Fun, free games! Challenge yourself or share with friends. Fun and easy to use games.";

        public string AppUrl { get { return appUrl; } }
        public string ScreenshotFilePath { get { return Path.Combine(Application.temporaryCachePath, "Screenshot.png"); } }


        /// <summary>
        /// Create the screenshot
        /// </summary>
        public void CreateScreenshot()
        {
            StartCoroutine(CRTakeScreenshot());
        }
        private IEnumerator CRTakeScreenshot()
        {
            Texture2D screenshot2D = null;
            yield return new WaitForEndOfFrame();
            screenshot2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot2D.Apply();
            File.WriteAllBytes(ScreenshotFilePath, screenshot2D.EncodeToPNG());
            Destroy(screenshot2D);
        }


        /// <summary>
        /// Share screenshot with text
        /// </summary>
        public void NativeShare()
        {
            new NativeShare().AddFile(ScreenshotFilePath).SetSubject(shareSubject).SetText(shareText + " " + AppUrl).Share();
        }


        /// <summary>
        /// Share on titter page
        /// </summary>
        public void TwitterShare()
        {
            Application.OpenURL(titterAddress + "?text=" + UnityWebRequest.EscapeURL(textToDisplay) + "&amp;lang=" + UnityWebRequest.EscapeURL(tweetLanguage));
        }


        /// <summary>
        /// Share on facbook page
        /// </summary>
        public void FacebookShare()
        {
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                Application.OpenURL("https://www.facebook.com/dialog/feed?" + "app_id=" + fbAppID + "&link=" + appUrl + "&picture=" + pictureUrl
                             + "&caption=" + caption + "&description=" + description);
            }
            else
            {
                Application.OpenURL("https://www.facebook.com/dialog/feed?" + "app_id=" + fbAppID + "&link=" + appUrl + "&caption=" + caption + "&description=" + description);
            }
        }
    }

}
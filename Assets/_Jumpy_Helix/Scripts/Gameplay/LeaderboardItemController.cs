using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItemController : MonoBehaviour
{
    [SerializeField] private Text usernameTxt = null;
    [SerializeField] private Text levelTxt = null;


    public void OnSetup(int indexRank, PlayerLeaderboardData data)
    {
        transform.localScale = Vector3.one;
        usernameTxt.text = indexRank.ToString() + "." + " " + data.Name;
        levelTxt.text = "Level: " + data.HighestLevel.ToString();

        if (indexRank == 1)
        {
            usernameTxt.color = Color.red;
            levelTxt.color = Color.red;
        }
        else if (indexRank == 2)
        {
            usernameTxt.color = Color.yellow;
            levelTxt.color = Color.yellow;
        }
        else if (indexRank == 3)
        {
            usernameTxt.color = Color.blue;
            levelTxt.color = Color.blue;
        }
        else if (indexRank == 4)
        {
            usernameTxt.color = Color.green;
            levelTxt.color = Color.green;
        }
        else if (indexRank == 5)
        {
            usernameTxt.color = Color.magenta;
            levelTxt.color = Color.magenta;
        }
    }
}

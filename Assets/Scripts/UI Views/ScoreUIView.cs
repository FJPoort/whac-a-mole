using TMPro;
using UnityEngine;

public class ScoreUIView : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _scoreText;

	public void UpdateScore(int score)
	{
		_scoreText.text = $"Score: {score}";
	}
}
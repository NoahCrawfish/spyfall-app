using UnityEngine;

public class ManagePlayerScoring : MonoBehaviour
{
    private ManageGame manageGame;

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();    
    }

    public enum RoundEndings {
        spyFoundOrGuessedWrong,
        spyGuessedRight,
        innocentVoted,
        timeRanOut
    }

    public void SetPreviousScores() {
        foreach (var player in manageGame.Players) {
            player.PreviousScore = player.Score;
        }
    }

    public void ResetScoresToPrevious() {
        foreach (var player in manageGame.Players) {
            player.Score = player.PreviousScore;
        }
    }

    public void UpdateScores(RoundEndings roundEnding, Player firstToVote = null) {
        switch (roundEnding) {
            case RoundEndings.spyFoundOrGuessedWrong:
                CiviliansWon(firstToVote);
                break;
            case RoundEndings.spyGuessedRight:
                SpyWon(false);
                break;
            case RoundEndings.innocentVoted:
                SpyWon(false);
                break;
            case RoundEndings.timeRanOut:
                SpyWon(true);
                break;
        }
    }

    private void CiviliansWon(Player firstToVote) {
        foreach (var player in manageGame.Players) {
            if (player == firstToVote) {
                player.Score += manageGame.MaxPoints[(int)ManageGame.PlayerTypes.civilian];
            } else if (player.Role != "Spy") {
                player.Score += manageGame.MaxPoints[(int)ManageGame.PlayerTypes.civilian] * 0.5f;
            }
        }
    }

    private void SpyWon(bool halfPoints) {
        foreach (var player in manageGame.Players) {
            if (player.Role == "Spy") {
                float points = manageGame.MaxPoints[(int)ManageGame.PlayerTypes.spy];
                player.Score += halfPoints ? points * 0.5f : points;
            }
        }
    }
}

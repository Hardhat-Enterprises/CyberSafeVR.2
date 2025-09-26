using UnityEngine;

namespace Smishing03
{
    public enum ExpectedType { Phishing, Safe }

    public class SocketJudgeSimple : MonoBehaviour
    {
        public ExpectedType expected;

        public bool IsCorrect(EmailCardTag card)
        {
            if (!card) return false;
            return (card.isPhishing && expected == ExpectedType.Phishing)
                || (!card.isPhishing && expected == ExpectedType.Safe);
        }
    }
}

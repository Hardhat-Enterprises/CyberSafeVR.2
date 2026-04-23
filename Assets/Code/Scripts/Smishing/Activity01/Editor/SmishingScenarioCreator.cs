#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Smishing01.Editor
{
    /// <summary>
    /// Creates the 6 SmishingScenarioData assets with rich Australian-context
    /// content. Called automatically from SmishingSceneBuilder.
    /// </summary>
    public static class SmishingScenarioCreator
    {
        public const string ScenarioDataFolder =
            "Assets/Code/Scripts/Smishing/Activity01/Runtime/ScenarioData";

        public static SmishingScenarioData[] CreateAll()
        {
            Directory.CreateDirectory(ScenarioDataFolder);

            var list = new SmishingScenarioData[6];
            list[0] = Create("01_BankingFraud", new SmishingScenarioData
            {
                scenarioTitle = "Bank Account Alert",
                category      = ScenarioCategory.BankingFraud,
                senderName    = "+61 4XX XXX XXX",
                messageBody   = "URGENT: Your ANZ account has been compromised. Click the link IMMEDIATELY to secure your account: http://secure-anz-banking.ru/verify",
                embeddedUrl   = "http://secure-anz-banking.ru/verify",
                introNarration= "A text just buzzed in on your phone. Read it carefully before deciding what to do.",
                correctAction = PlayerAction.ReportMessage,
                correctFeedback   = "Great work! This was a phishing attempt. The .ru domain, urgency language, and generic sender number are classic red flags.",
                incorrectFeedback = "That was a smishing attempt. Banks never ask you to verify accounts via SMS links, and they don't use foreign .ru domains.",
                redFlags = new[] {
                    "Urgency language (URGENT, IMMEDIATELY)",
                    "Suspicious .ru domain",
                    "Generic sender number (not a verified bank shortcode)",
                    "Unsolicited request to click a link"
                },
                hintText = "Look closely at the URL — does that domain really belong to an Australian bank?",
                suspiciousSubstring = "secure-anz-banking.ru",
                difficultyLevel = 1
            });

            list[1] = Create("02_ParcelDelivery", new SmishingScenarioData
            {
                scenarioTitle = "Parcel Delivery",
                category      = ScenarioCategory.PackageDelivery,
                senderName    = "AusPost Delivery",
                messageBody   = "Your parcel could not be delivered. A $3.20 redelivery fee is required. Pay now or your parcel will be returned: https://ausposst-redelivery.com/pay",
                embeddedUrl   = "https://ausposst-redelivery.com/pay",
                introNarration= "Another text. You weren't expecting any parcels — was something ordered recently?",
                correctAction = PlayerAction.ReportMessage,
                correctFeedback = "Sharp eye! 'ausposst' is a typo of 'auspost' and the .com TLD is wrong — Australia Post uses auspost.com.au.",
                incorrectFeedback = "This was a typosquatting attack. Real Australia Post domain is auspost.com.au, and they never request payment via SMS link.",
                redFlags = new[] {
                    "Misspelled domain (ausposst)",
                    "Wrong TLD (.com vs .com.au)",
                    "Requests payment via SMS",
                    "Unexpected / unsolicited message"
                },
                hintText = "Spell the sender's brand name out loud. Does the URL match exactly?",
                suspiciousSubstring = "ausposst",
                difficultyLevel = 2
            });

            list[2] = Create("03_ATORefund", new SmishingScenarioData
            {
                scenarioTitle = "ATO Tax Refund",
                category      = ScenarioCategory.GovernmentImpersonation,
                senderName    = "ATO-Refund",
                messageBody   = "Good news! You are eligible for a $842.50 tax refund. Claim it within 24 hours at: https://ato-refund-claim.info/submit",
                embeddedUrl   = "https://ato-refund-claim.info/submit",
                introNarration= "The Australian Tax Office appears to have messaged you about a refund.",
                correctAction = PlayerAction.ReportMessage,
                correctFeedback = "Correct. The real ATO NEVER notifies of refunds by SMS, and the .info domain is a major giveaway.",
                incorrectFeedback = "This was a scam. The ATO communicates refunds only through your myGov inbox — never by SMS link.",
                redFlags = new[] {
                    ".info domain (ATO uses .gov.au)",
                    "Time-limited pressure (24 hours)",
                    "ATO does not notify refunds via SMS",
                    "Suspicious subdomain structure"
                },
                hintText = "What top-level domain would a real Australian government service use?",
                suspiciousSubstring = ".info",
                difficultyLevel = 2
            });

            list[3] = Create("04_TelcoSuspension", new SmishingScenarioData
            {
                scenarioTitle = "Telstra Account",
                category      = ScenarioCategory.TelcoScam,
                senderName    = "Telstra",
                messageBody   = "Your account will be suspended today due to unpaid charges. Verify your details now: https://bit.ly/tlstra-verify-3819",
                embeddedUrl   = "https://bit.ly/tlstra-verify-3819",
                introNarration= "A message from your telco — or is it?",
                correctAction = PlayerAction.ReportMessage,
                correctFeedback = "Nicely spotted. Bit.ly shortlinks hide the real destination — legitimate telcos never use them for account actions.",
                incorrectFeedback = "This was a phishing attempt. Telstra would use a telstra.com.au domain directly, never a bit.ly shortlink.",
                redFlags = new[] {
                    "URL shortener hides real destination",
                    "Suspension threat creates panic",
                    "Misspelled brand (tlstra)",
                    "Requests account verification via SMS"
                },
                hintText = "Why would a legitimate company hide the URL destination with a shortlink?",
                suspiciousSubstring = "bit.ly/tlstra-verify",
                difficultyLevel = 1
            });

            list[4] = Create("05_PrizeScam", new SmishingScenarioData
            {
                scenarioTitle = "Prize Winner",
                category      = ScenarioCategory.PrizeScam,
                senderName    = "Coles Rewards",
                messageBody   = "Congratulations! You've won a $500 Coles gift card. Claim before it expires: https://coles-winners.xyz/claim/8821",
                embeddedUrl   = "https://coles-winners.xyz/claim/8821",
                introNarration= "An exciting win, apparently. Is this too good to be true?",
                correctAction = PlayerAction.ReportMessage,
                correctFeedback = "You got it! The .xyz TLD and 'too good to be true' framing are classic prize-scam markers.",
                incorrectFeedback = "Classic prize scam. Coles would use coles.com.au; .xyz domains are frequently abused for fraud.",
                redFlags = new[] {
                    ".xyz TLD is commonly abused",
                    "Too-good-to-be-true prize",
                    "Artificial expiry deadline",
                    "Unsolicited win (you didn't enter)"
                },
                hintText = "Did you actually enter a competition run by this company?",
                suspiciousSubstring = ".xyz",
                difficultyLevel = 1
            });

            list[5] = Create("06_GPAppointment", new SmishingScenarioData
            {
                scenarioTitle = "Appointment Reminder",
                category      = ScenarioCategory.Legitimate,
                senderName    = "MyMedicare",
                messageBody   = "Reminder: You have a GP appointment tomorrow at 10:00 AM at Deakin Health Clinic, Waurn Ponds. Call 5227 8500 to reschedule. Do not reply to this SMS.",
                embeddedUrl   = "",
                introNarration= "You booked a GP appointment yesterday. Decide whether to act on this reminder or flag it.",
                correctAction = PlayerAction.IgnoreMessage,
                correctFeedback = "Well done! This message is LEGITIMATE. No link, local phone number, and it references an appointment you actually made. Recognising safe messages matters too.",
                incorrectFeedback = "This message was actually legitimate — no link, a verifiable local number, and it refers to an appointment you made. Healthy caution is good, but over-reporting wastes effort.",
                redFlags = new[] {
                    "(No red flags — this is a legitimate message)"
                },
                hintText = "Are there actually any links or payment requests in this message?",
                suspiciousSubstring = "",
                difficultyLevel = 3
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return list;
        }

        private static SmishingScenarioData Create(string assetName, SmishingScenarioData template)
        {
            string path = $"{ScenarioDataFolder}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<SmishingScenarioData>(path);
            if (existing != null)
            {
                CopyFields(template, existing);
                EditorUtility.SetDirty(existing);
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<SmishingScenarioData>();
            CopyFields(template, asset);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void CopyFields(SmishingScenarioData from, SmishingScenarioData to)
        {
            to.scenarioTitle       = from.scenarioTitle;
            to.category            = from.category;
            to.senderName          = from.senderName;
            to.messageBody         = from.messageBody;
            to.embeddedUrl         = from.embeddedUrl;
            to.introNarration      = from.introNarration;
            to.correctAction       = from.correctAction;
            to.correctFeedback     = from.correctFeedback;
            to.incorrectFeedback   = from.incorrectFeedback;
            to.redFlags            = from.redFlags;
            to.hintText            = from.hintText;
            to.suspiciousSubstring = from.suspiciousSubstring;
            to.difficultyLevel     = from.difficultyLevel;
        }
    }
}
#endif

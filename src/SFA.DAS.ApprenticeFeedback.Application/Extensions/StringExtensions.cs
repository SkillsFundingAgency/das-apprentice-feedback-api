using System.Text.RegularExpressions;

namespace SFA.DAS.ApprenticeFeedback.Application.Extensions
{
    public static class StringExtensions
    {
        // Only these characters are valid to appear in the answers in the exit survey.
        // Guarding against HTML or JS injection attack
        private static readonly Regex regexLegalExitSurveyAnswerCharacters = new Regex("[^a-zA-Z '-]");

        public static string RemoveIllegalCharacters(this string str)
        {
            if (null == str) return str;
            return regexLegalExitSurveyAnswerCharacters.Replace(str, string.Empty);
        }
    }
}

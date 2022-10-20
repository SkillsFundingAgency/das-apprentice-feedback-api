using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Extensions;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        public class RemoveIllegalCharacters
        {
            [TestCase(null, ExpectedResult = null)]
            [TestCase("Everything is fine here", ExpectedResult = "Everything is fine here")]
            [TestCase("Don't remove the apostrophe", ExpectedResult = "Don't remove the apostrophe")]
            [TestCase("Hyphens for example end-point are ok", ExpectedResult = "Hyphens for example end-point are ok")]
            [TestCase("<a href=\"http://imadodgylink.com\">Click this dodgy link</a>", ExpectedResult = "a hrefhttpimadodgylinkcomClick this dodgy linka")]
            [TestCase("<button onclick='alert(\"You shouldn't have clicked that!\");'>Dodgy button</button>", ExpectedResult = "button onclick'alertYou shouldn't have clicked that'Dodgy buttonbutton")]
            [TestCase("<script>Here be nasties</script>", ExpectedResult = "scriptHere be nastiesscript")]
            public string OnlyIllegalCharactersAreRemoved(string str)
            {
                return str.RemoveIllegalCharacters();
            }
        }
    }
}

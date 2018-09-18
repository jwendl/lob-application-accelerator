using OpenQA.Selenium;

namespace LobAccelerator.TestingUtils.Auth
{
    static class SeleniumExtensions
    {
        public static void SendTextToTextBox(this IWebDriver driver, string elementName, string text)
        {
            IWebElement emailElement = driver.FindElement(By.Name(elementName));
            emailElement.SendKeys(text);
        }

        public static void ClickOnButton(this IWebDriver driver, string elementName)
        {
            IWebElement secondButtonElement = driver.FindElement(By.ClassName(elementName));
            secondButtonElement.Click();
        }
    }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace BusinessAsUsual.Tests.E2E
{
    /// <summary>
    /// End-to-end test for the provisioning flow using Selenium.
    /// Simulates user interaction with the Razor Page and verifies modal feedback.
    /// </summary>
    public class ProvisioningFlowTests
    {
        /// <summary>
        /// Test form fill and submission success
        /// </summary>
        [Fact]
        public void ProvisioningFlow_ShowsSuccessModal()
        {
            using var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost:5000/Admin/ProvisionCompany");

            driver.FindElement(By.Id("CompanyName")).SendKeys("TestCo");
            driver.FindElement(By.Id("AdminEmail")).SendKeys("admin@test.com");
            driver.FindElement(By.Id("BillingPlan")).SendKeys("Standard");
            driver.FindElement(By.Id("ModuleInventory")).Click();
            driver.FindElement(By.Id("SubmitButton")).Click();

            var modal = driver.FindElement(By.Id("statusModal"));
            Assert.True(modal.Displayed);
        }
    }
}
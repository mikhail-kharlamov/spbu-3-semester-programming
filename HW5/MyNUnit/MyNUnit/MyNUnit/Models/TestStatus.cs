namespace MyNUnit.Models;

/// <summary>
/// Represents the overall status of a single test.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// The test completed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// The test itself did not pass (assertion failed or unexpected exception in test body).
    /// </summary>
    Failed,

    /// <summary>
    /// The test was skipped and not executed.
    /// </summary>
    Ignored,

    /// <summary>
    /// The test could not be executed because infrastructure failed
    /// (Before/After/BeforeClass/AfterClass) or the test method definition is invalid.
    /// </summary>
    Errored,
}

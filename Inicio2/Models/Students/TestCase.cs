namespace Inicio2.Models.Estudiantes
{
    
    /// Represents a test case for a programming problem.
    
    public class TestCase
    {
        public int Id { get; set; }

        
        /// Input for the test case.
        
        public string Input { get; set; } = string.Empty;

        
        /// Expected output for the test case.
        
        public string ExpectedOutput { get; set; } = string.Empty;
    }
}
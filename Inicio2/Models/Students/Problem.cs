namespace Inicio2.Models.Estudiantes
{
    
    /// Represents a programming problem with its metadata and test cases.
    
    public class Problem
    {
        public int Id { get; set; }

    
        /// Problem code (unique identifier).
    
        public string Code { get; set; } = string.Empty;

        
        /// Problem title.
        
        public string Title { get; set; } = string.Empty;

        
        /// Problem description.
        
        public string Description { get; set; } = string.Empty;

        
        /// Difficulty level (e.g., Easy, Medium, Hard).
        
        public string Difficulty { get; set; } = string.Empty;

        
        /// Topics related to the problem.
        
        public string Topics { get; set; } = string.Empty;

        
        /// Example input for the problem.
        
        public string SampleInput { get; set; } = string.Empty;

        
        /// Example output for the problem.
        
        public string SampleOutput { get; set; } = string.Empty;

        
        /// Additional cases or notes.
        
        public string AdditionalCases { get; set; } = string.Empty;

        
        /// List of test cases for the problem.
        
        public List<TestCase> TestCases { get; set; } = new();
    }
}


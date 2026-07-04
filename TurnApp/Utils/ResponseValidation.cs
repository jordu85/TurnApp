namespace TurnApp.Utils
{
    public class ResponseValidation
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ResponseValidation(Dictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}

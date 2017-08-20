namespace Isidore.MagicMirror.Infrastructure.Validation
{
    public class ValidationResult
    {
        public object Entity { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
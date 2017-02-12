namespace Isidore.MagicMirror.ImageProcessing.FaceRecognition
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual object GetHashCode(){
            return Id.GetHashCode() & Name.GetHashCode();
        }
    }
}
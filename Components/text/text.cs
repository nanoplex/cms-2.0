using MongoDB.Bson;

namespace cms.Components
{
    public class text : Component
    {
        public string Text { get; set; }
        public int Size { get; set; }

        public override string Frontend
        {
            get
            {
                return "<p data-id='" + _id + "' style='font-size:" + Size + "px'>" + Text + "</p>";
            }
        }
    }
}
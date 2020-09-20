using System.ComponentModel.DataAnnotations;
namespace scoreapp.Data
{
    public class ScoreModel
    {
        [Key]
        public string UserName { get; set; }
        public int Points { get; set; }
    }
    public class ScoreModelContainer
    {
        public ScoreModel[] Scores { get; set; }
    }
}

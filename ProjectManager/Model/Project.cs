using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int ResponsibilityId { get; set; }
        public User? Responsibility { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Description { get; set; }
        public ObservableCollection<Assignment> Assignments { get; set; } = [];
        [NotMapped]
        public ObservableCollection<Assignment> MyAssignments { get; set; } = [];
        public int Progress
        {
            get
            {
                if (Assignments == null || Assignments.Count == 0)
                    return 0;

                int weightedSum = 0;
                int totalWeight = 0;

                foreach (var ass in Assignments)
                {
                    int weight = PriorityToInt(ass.Priority.ToString());
                    weightedSum += ass.ProgressPercent * weight;
                    totalWeight += weight;
                }

                return totalWeight > 0 ? weightedSum / totalWeight : 0;
            }
        }
        private int PriorityToInt(string priority)
        {
            switch (priority)
            {
                case "Hoch":
                    return 3;
                case "Mittel":
                    return 2;
                case "Niedrig":
                    return 1;
                default:
                    return 0;
            }
        }
        public override string ToString()
        {
            return Title;
        }
    }
}

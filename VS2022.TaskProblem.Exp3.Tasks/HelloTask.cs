using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Exp3.Tasks
{
    public class HelloTask : InfoTask
    {
        protected override string Hello => "Exp3";
        protected override string Title => "Experiment 3 - Output";
        protected override string FilePrefix => "exp3";
    }
}

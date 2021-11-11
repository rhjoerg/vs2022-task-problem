using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Exp5.Tasks
{
    public class HelloTask : InfoTask
    {
        protected override string Hello => "Exp5";
        protected override string Title => "Experiment 5 - Output";
        protected override string FilePrefix => "exp5";
    }
}

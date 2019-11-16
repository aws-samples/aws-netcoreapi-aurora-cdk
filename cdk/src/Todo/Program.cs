using Amazon.CDK;

namespace Todo
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null);
            new TodoInfraStack(app, "TodoInfraStack", new StackProps());
            app.Synth();
        }
    }
}

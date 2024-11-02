using System.Text.RegularExpressions;

var input_files = Directory.GetFiles("tests").Where(f => f.EndsWith(".in")).Order();
var output_files = Directory.GetFiles("tests").Where(f => f.EndsWith(".out")).Order();

foreach (var (in_file, out_file) in input_files.Zip(output_files))
{
    var solver = new DynamicProgrammingSolver(in_file);
    solver.solve();

    var steps = solver.getSolution();
    var expected_steps = Parsing.readFile(out_file).ExtractLongs().First();
    if (steps == expected_steps)
    {
        Console.WriteLine($"Success on {in_file}, got {steps}");
    }
    else
    {
        Console.WriteLine($"Failed on {in_file}, got: {steps}, expected {expected_steps}");
    }
}

public class DynamicProgrammingSolver
{
    public DynamicProgrammingSolver(string file_name)
    {
        string[] fileLines = Parsing.readFileLines(file_name);

        var info = fileLines[0].ExtractLongs().ToArray();
        var num_locations = info[0];
        var num_edges = info[1];
        necessary_food = info[2];

        var food_info = fileLines[1].ExtractLongs();
        graph_nodes = food_info.Select(f => new Node(f)).ToList();

        for (int edge_num = 0; edge_num < num_edges; edge_num++)
        {
            var edge = fileLines[edge_num + 2].ExtractLongs().ToArray();
            int i = (int)edge[0] - 1;  // convert to zero index
            int j = (int)edge[1] - 1;  // convert to zero index
            graph_nodes[i].neighbors.Add(j);
        }

        var goal_index = graph_nodes.Count - 1;
        solverState[goal_index] = new();
        solverState[goal_index][ZERO_STEPS] = graph_nodes[goal_index].food;
    }

    public void solve(int index = 0)
    {
        if (!isSolved(index))
        {
            solverState[index] = new();
            foreach (var neighbor in graph_nodes[index].neighbors)
            {
                solve(neighbor);
                foreach (var (steps, food) in solverState[neighbor])
                {
                    long current_best_food = solverState[index].GetValueOrDefault(steps + 1);
                    solverState[index][steps + 1] = Math.Max(current_best_food, food + graph_nodes[index].food);
                }
            }
        }
    }

    public long getSolution()
    {
        long solution = long.MaxValue;
        foreach( var (steps, food) in solverState[0])
        {
            if(food >= necessary_food && steps < solution)
            {
                solution = steps;
            }
        }
        return solution;
    }

    private bool isSolved(int index) => solverState.ContainsKey(index);

    private const int ZERO_STEPS = 0;
    private List<Node> graph_nodes = new();
    private long necessary_food = 0;
    private Dictionary<int, Dictionary<long, long>> solverState = new();
}

public class Node
{
    public long food = 0;
    public List<int> neighbors = new();

    public Node(long food)
    {
        this.food = food;
    }
}

public static class Parsing
{
    public static string readFile(string file_name) => File.ReadAllText(file_name);

    public static string[] SplitLines(this string str) => str.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

    public static string[] readFileLines(string file_name) => File.ReadAllText(file_name).SplitLines();

    public static IEnumerable<long> ExtractLongs(this string str) => Regex.Matches(str, @"\d+").Select(_ => long.Parse(_.Value));
}


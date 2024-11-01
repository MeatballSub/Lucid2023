using System.Text;
using System.Text.RegularExpressions;

var graph_nodes = parseInput("test_input/01.in");

foreach(var node in graph_nodes)
{
    Console.WriteLine(node);
}

List<Node> parseInput(string file_name)
{
    string[] fileLines = Parsing.readFileLines(file_name);

    var info = fileLines[0].ExtractLongs().ToArray();
    var num_locations = info[0];
    var num_edges = info[1];
    var necessary_food = info[2];

    var food_info = fileLines[1].ExtractLongs();
    var graph_nodes = food_info.Select(f => new Node(f)).ToList();

    for (int edge_num = 0; edge_num < num_edges; edge_num++)
    {
        var edge = fileLines[edge_num + 2].ExtractLongs().ToArray();
        int i = (int)edge[0] - 1;  // convert to zero index
        int j = (int)edge[1] - 1;  // convert to zero index
        graph_nodes[i].neighbors.Add(j);
    }

    return graph_nodes;
}

public class Node
{
    public long food = 0;
    public List<int> neighbors = new();

    public Node(long food)
    {
        this.food = food;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Food = " + food);
        foreach(var neighbor in neighbors)
        {
            sb.AppendLine("  " + (neighbor + 1));  // convert to one index
        }
        return sb.ToString();
    }
}

public static class Parsing
{
    public static string readFile(string file_name) => File.ReadAllText(file_name);

    public static string[] SplitLines(this string str) => str.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

    public static string[] readFileLines(string file_name) => File.ReadAllText(file_name).SplitLines();

    public static IEnumerable<long> ExtractLongs(this string str) => Regex.Matches(str, @"\d+").Select(_ => long.Parse(_.Value));
    public static IEnumerable<long> ExtractSignedLongs(this string str) => Regex.Matches(str, @"-?\d+").Select(_ => long.Parse(_.Value));
    public static IEnumerable<IEnumerable<long>> ExtractLongs(this IEnumerable<string> str) => str.Select(s => Regex.Matches(s, @"\d+").Select(_ => long.Parse(_.Value)));
    public static IEnumerable<IEnumerable<long>> ExtractSignedLongs(this IEnumerable<string> str) => str.Select(s => Regex.Matches(s, @"-?\d+").Select(_ => long.Parse(_.Value)));

    public static string get(this Match m, string named_capture) => m.Groups[named_capture].Value;
}


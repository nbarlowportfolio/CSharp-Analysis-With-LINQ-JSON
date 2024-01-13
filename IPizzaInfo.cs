
namespace mercuryworks.jobscreening{
public interface IPizzaInfo
{
    string? Name { get; set; }
    string? Department { get; set; }
    List<string>? Toppings { get; set; }
}
}
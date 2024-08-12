namespace StagePermutations;

public class RegisterPermutationAttribute : SearchableAttribute
{
    public RegisterPermutationAttribute(string targetSceneName, string configSection, string name)
    {
        this.targetSceneName = targetSceneName;
        this.configSection = configSection;
        this.name = name;
    }

    public string targetSceneName;
    public string configSection;
    public string name;
    public string description;
}
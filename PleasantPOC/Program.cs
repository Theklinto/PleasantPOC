using System.Text.Json;
using System.Text.RegularExpressions;
using PleasantPOC;
using PleasantPOC.Models;

static string GetPasswordFromConsole()
{
    var pass = string.Empty;
    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && pass.Length > 0)
        {
            Console.Write("\b \b");
            pass = pass[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            pass += keyInfo.KeyChar;
        }
    } while (key != ConsoleKey.Enter);
    Console.WriteLine();

    return pass;
}

PleasantClient client = new();

#region Login details
{
    Console.WriteLine("Enter login information for Pleasant");
    Console.Write("Username: ");
    string username = Console.ReadLine() ?? string.Empty;
    Console.Write("Password: ");
    string password = GetPasswordFromConsole();
    Console.Write("2FA token: ");
    string token = Console.ReadLine() ?? string.Empty;
    LoginModel loginModel = new()
    {
        Password = password,
        Username = username,
        TwoFactorToken = token
    };
    try
    {
        await client.LoginAsync(loginModel);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("The login was successful");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to login: {ex.Message}");
        return;
    }
}
#endregion

CredentialGroup? selectedEnvironment = null;
#region Get enviroment
{
    Console.WriteLine("Fetching available environments...");
    CredentialGroup? credentialGroup;
    try
    {
        credentialGroup = await client.GetEnvironmentsCredentialGroupAsync();
        if (credentialGroup is null)
            return;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to fetch environments: {ex.Message}");
        return;
    }

    List<(int Index, CredentialGroup Environment)> environments;
    {
        int index = 1;
        environments = credentialGroup.Children
            .Select(x => (Index: index++, Environment: x))
            .OrderBy(x => x.Index)
            .ToList();
    }
    if (environments.Any() is false)
    {
        Console.WriteLine("No available environments was found");
        return;
    }

    while (selectedEnvironment is null)
    {

        Console.WriteLine();
        Console.WriteLine("Select which environment should be configured");
        foreach ((int index, CredentialGroup environment) in environments)
        {
            Console.WriteLine($"\t{index}. {environment.Name}");
        }

        Console.Write("Select environment(number): ");
        string selectedIndexString = Console.ReadLine() ?? string.Empty;
        _ = int.TryParse(selectedIndexString, out int selectedIndex);
        selectedEnvironment = environments.FirstOrDefault(x => x.Index == selectedIndex).Environment;
        if (selectedEnvironment is null)
        {
            Console.WriteLine("The selected environment was not found");
        }
    }

    Console.WriteLine();
    Console.Write($"The following environment was selected: ");
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine(selectedEnvironment.Name);
    Console.ResetColor();
    Console.WriteLine();
}
#endregion

Console.WriteLine($"Fetching credentials in {selectedEnvironment.Name}");
List<Credential> credentials = await client.GetEnvironmentCredentialsAsync(selectedEnvironment.Id);
Console.WriteLine();
foreach (Credential credential in credentials)
{
    Console.WriteLine($"Handeling credential: {credential.Name}");
    bool templateFound = credential.CustomUserFields.TryGetValue(Utils.TemplateFilePathKey, out string? templateFileRelativePath);
    bool filePlacementDefined = credential.CustomUserFields.TryGetValue(Utils.FilePathKey, out string? filePlacementRelativePath);

    if (templateFound is false || filePlacementDefined is false
        || string.IsNullOrWhiteSpace(templateFileRelativePath)
        || string.IsNullOrWhiteSpace(filePlacementRelativePath))
    {
        continue;
    }

    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), templateFileRelativePath);
    string templateFileAsString = File.ReadAllText(templatePath);

    //Values on the entry
    Dictionary<string, string> keyValuePairs = credential.CustomUserFields
        .Where(x => Utils.IsValidKeyRegex().Match(x.Key).Success)
        .ToDictionary(x => x.Key, x => x.Value);

    //Find references
    List<Guid> references = credential.CustomUserFields
        .Where(x => x.Key.StartsWith("Reference") && Guid.TryParse(x.Value, out _))
        .Select(x => new Guid(x.Value))
        .ToList();

    //Get KeyValuePairs from references
    Console.WriteLine($"\tFound {references.Count} references");
    foreach (Guid referenceCredentialId in references)
    {
        Credential referenceCredential = await client.GetCredentialsAsync(referenceCredentialId);
        Console.WriteLine($"\t\tFetching values from {referenceCredential.Name}");
        foreach ((string key, string value) in referenceCredential.CustomUserFields)
        {
            if (Utils.IsValidKeyRegex().Match(key).Success is false)
                continue;

            if (keyValuePairs.ContainsKey(key))
                keyValuePairs[key] = value;
            else
                keyValuePairs.Add(key, value);
        }
    }

    foreach ((string key, string value) in keyValuePairs)
    {
        Console.WriteLine($"\tReplacing key: {key}");
        templateFileAsString = templateFileAsString.Replace(key, value);
    }

    string filePlacementPath = Path.Combine(Directory.GetCurrentDirectory(), filePlacementRelativePath);
    await File.WriteAllTextAsync(filePlacementPath, templateFileAsString);
}
Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Program done!");
Console.ResetColor();
Console.Write("Press any key to exit...");
Console.Read();
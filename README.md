# PleasantPOC
Developed as a proof of concept for generation configuration files through Pleasant Password Server.

## How to setup PleasantPOC
### How to build
From within Visual Studio, the PleasantPOC project should be published using the following properties:
- Deployment-mode: Self-contained
- Produce single file
- Trim unused code
     - Not necessarily needed, but reduces size of executable

After there shold only be the executable and a config.json file. Those needs to be in the same folder.

### How to configure
There is very little configuration for the program itself, most of it is done in Pleasant.

configuration explained
```
"PleasantServer": {
    "Url": "", //Should be the url to the Pleasant Password Server
    "Port": "", //Not necessarily needed, but default values are: 443 (https) and 80 (http)
    "EnvironmentsCredentialGroupId": "" //Should be the credentialgroup/folder id for the folder containing the environments
  }
```
The section about Pleasant should explain how to optain the required credentialgroup/folder id. The configuration are are meant to include only non sensitive data, as it makes it easier to include in version control. So the team members quickly can setup the envrionment needed.

## How to configure Pleasant
### Setup root environment folder
Nothing fancy in this. A folder should be created somewhere. Placement doesn't matter. This will be the root folder for creating environment specific variables.

The id of this folder, should be the one configured in config.json as the EnvironmentsCredentialGroupId. It can be optained in the web client by rightclicking the folder and selecting "Copy Link to Folder". At the end of the url, there should be an query parameter ?itemId={guid}. The Guid should be used.

### Setup environment folders
Inside the root environment folder, there should be created a folder for each envrionment. The folder naming doesn't matter, but will be the name used in the program when selected the environment. All credentials/entries inside this folder, will be taken into account when searching for configuration entries. But if the required values are not present, nothing wil happen. So it's perfectly fine to place entries inside, not used by the program.

### Setup configuration entries
An entry/credential can be setup to replace values inside a template file, and create is as a configuration file.
The naming of the entry is not relevant, but will be printed in the console when going through.

For the entry to be considered a valid configuration entry, the TemplateFile and FilePlacement fields should be set. But no Key-Value pairs are required.

#### Available custom fields
- TemplateFile
     - The key should be: TemplateFile
     - The value is the path to the templatefile, including filename and extension.
     - e.g. /api/config/appsettings.json.template
- FilePlacement
     - They key should be: FilePlacement
     - The value is the path where the file should be created, including filename and extension.
     - e.g. /api/config/appsettings.json
- Key-Value pair - Format: {#*#}
     - They key for this field, should be encapsulated in hashtags(#). It will match keys with same format in template files. 
     - The star(*) can be replaced with anything, as it matches through regex, and only looks at the starting and ending character
     - The key should be exactly like this in the template file, including the hashtags(#).
     - The key is case sensitive.
- Reference - Format: {Reference*}
     - This indicates that Key-Value pairs from another entry should be included. Can be used for global configurations, so they can be manged in a single place.
         - e.g. access to services that are available in multiple envrionments, but requires special access.
     - The key should start with Reference, but anything goes after. It only matches the starting characters.
     - The value should be the id of another entry/credential. But are not limited in placement. As long as the accessing user, has access to it.

**Notes:**

- The paths are relative paths, with the root being where the executable is placed.
- All keys are case sensitive

## How to use PleasantPOC
The program is a console application and should be fairly simple to use.

#### Flow
 - Enter login details for Pleasant Password Server
     - Characters in password are replaced by stars(*) when typing, and are only saved in memory. Scoped to the login sequence.
 - Searching for environments through the provided folder id
 - Select environment from list
     - Should be selected through the number
 - Finds all the valid entries in the envrionment folder and creates the files
 - End of story :)